using System;
using System.Collections.Generic;
using AegisFlow.Data;
using AegisFlow.Event;
using UnityEngine;

namespace AegisFlow.Runtime
{
    /// <summary>
    /// 仿真步进调度器。用非重入保护替代 async void OnUpdate 风险。
    /// </summary>
    public sealed class SimulationStepScheduler
    {
        private readonly RuntimeDC m_RuntimeDC;
        private readonly SimulationEventProcessor m_EventProcessor;
        private readonly SimulationEventQueue m_EventQueue;
        private readonly DomainEventBus m_EventBus;
        private readonly float m_StepSeconds;
        private readonly int m_MaxStepsPerTick;
        private bool m_IsTicking;
        private float m_AccumulatedSeconds;
        private SimulationEvent m_FailedEvent;
        private List<SimulationEvent> m_PendingStepEvents;
        private int m_NextPendingEventIndex;

        public bool HasPendingFailure => m_FailedEvent != null;

        public SimulationStepScheduler(
            RuntimeDC runtimeDC,
            SimulationEventProcessor eventProcessor,
            SimulationEventQueue eventQueue,
            DomainEventBus eventBus,
            float stepSeconds = 1f / 30f,
            int maxStepsPerTick = 8)
        {
            m_RuntimeDC = runtimeDC;
            m_EventProcessor = eventProcessor;
            m_EventQueue = eventQueue;
            m_EventBus = eventBus;
            m_StepSeconds = Mathf.Max(stepSeconds, 0.0001f);
            m_MaxStepsPerTick = Mathf.Max(maxStepsPerTick, 1);
        }

        public void Tick(float deltaTime)
        {
            if (m_IsTicking || !m_RuntimeDC.IsPlaying)
            {
                return;
            }

            if (deltaTime <= 0f)
            {
                return;
            }

            m_IsTicking = true;

            try
            {
                float maxAccumulatedSeconds = m_StepSeconds * m_MaxStepsPerTick;
                m_AccumulatedSeconds = Mathf.Min(m_AccumulatedSeconds + deltaTime, maxAccumulatedSeconds);

                int executedSteps = 0;

                while (m_AccumulatedSeconds >= m_StepSeconds && executedSteps < m_MaxStepsPerTick)
                {
                    m_AccumulatedSeconds -= m_StepSeconds;
                    if (!ExecuteStep())
                    {
                        break;
                    }

                    executedSteps++;
                }
            }
            finally
            {
                m_IsTicking = false;
            }
        }

        public void Reset()
        {
            m_AccumulatedSeconds = 0f;
            m_IsTicking = false;
            ClearFailureContext();
        }

        public bool RetryFailedEvent()
        {
            if (!HasPendingFailure || m_RuntimeDC.IsPlaying)
            {
                return false;
            }

            SimulationEvent recoveryTarget = m_FailedEvent;
            m_RuntimeDC.Resume();
            SimulationEventExecutionResult result = m_EventProcessor.ExecuteEvent(recoveryTarget);
            PublishExecutionResult(result);

            if (!result.IsSuccess)
            {
                m_RuntimeDC.Pause();
                PublishRecovery(
                    SimulationRecoveryAction.Retry,
                    recoveryTarget,
                    false,
                    "失败事件重试后仍未成功。");
                return false;
            }

            m_FailedEvent = null;
            bool isRecovered = ExecutePendingStepEvents();
            PublishRecovery(
                SimulationRecoveryAction.Retry,
                recoveryTarget,
                isRecovered,
                isRecovered ? "失败事件重试成功，运行态已恢复。" : "重试成功，但后续事件再次失败。");
            return isRecovered;
        }

        public bool SkipFailedEvent()
        {
            if (!HasPendingFailure || m_RuntimeDC.IsPlaying)
            {
                return false;
            }

            SimulationEvent recoveryTarget = m_FailedEvent;
            m_FailedEvent = null;
            m_RuntimeDC.Resume();
            bool isRecovered = ExecutePendingStepEvents();
            PublishRecovery(
                SimulationRecoveryAction.Skip,
                recoveryTarget,
                isRecovered,
                isRecovered ? "失败事件已跳过，运行态已恢复。" : "失败事件已跳过，但后续事件再次失败。");
            return isRecovered;
        }

        public bool TerminateFailedRuntime()
        {
            if (!HasPendingFailure)
            {
                return false;
            }

            SimulationEvent recoveryTarget = m_FailedEvent;
            PublishRecovery(
                SimulationRecoveryAction.Terminate,
                recoveryTarget,
                true,
                "失败运行态已终止。");
            ClearFailureContext();
            return true;
        }

        private bool ExecuteStep()
        {
            int nextStep = m_RuntimeDC.CurrentStep + 1;
            m_RuntimeDC.UpdateStep(nextStep);
            FireSafely(new RuntimeStepChangedEvent(Time.time, m_RuntimeDC.RunningModelId, nextStep));

            List<SimulationEvent> events = m_EventQueue.Dequeue(nextStep);

            if (events == null)
            {
                return true;
            }

            for (int i = 0; i < events.Count; i++)
            {
                SimulationEventExecutionResult result = m_EventProcessor.ExecuteEvent(events[i]);
                PublishExecutionResult(result);

                if (!result.IsSuccess && result.FailureAction == SimulationEventFailureAction.PauseRuntime)
                {
                    SetPendingFailure(events[i], events, i + 1);
                    m_RuntimeDC.Pause();
                    return false;
                }
            }

            return true;
        }

        private bool ExecutePendingStepEvents()
        {
            while (m_PendingStepEvents != null && m_NextPendingEventIndex < m_PendingStepEvents.Count)
            {
                SimulationEvent simulationEvent = m_PendingStepEvents[m_NextPendingEventIndex];
                m_NextPendingEventIndex++;
                SimulationEventExecutionResult result = m_EventProcessor.ExecuteEvent(simulationEvent);
                PublishExecutionResult(result);

                if (!result.IsSuccess && result.FailureAction == SimulationEventFailureAction.PauseRuntime)
                {
                    m_FailedEvent = simulationEvent;
                    m_RuntimeDC.Pause();
                    return false;
                }
            }

            ClearFailureContext();
            return true;
        }

        private void SetPendingFailure(
            SimulationEvent failedEvent,
            List<SimulationEvent> stepEvents,
            int nextEventIndex)
        {
            m_FailedEvent = failedEvent;
            m_PendingStepEvents = stepEvents;
            m_NextPendingEventIndex = nextEventIndex;
        }

        private void ClearFailureContext()
        {
            m_FailedEvent = null;
            m_PendingStepEvents = null;
            m_NextPendingEventIndex = 0;
        }

        private void PublishExecutionResult(SimulationEventExecutionResult result)
        {
            SimulationEvent simulationEvent = result.SimulationEvent;
            FireSafely(new SimulationEventExecutedEvent(
                Time.time,
                simulationEvent?.EventType,
                simulationEvent?.EntityId,
                simulationEvent?.TimeStep ?? -1,
                result.Status,
                result.AttemptCount,
                result.DurationMilliseconds,
                result.ErrorMessage));
        }

        private void PublishRecovery(
            SimulationRecoveryAction action,
            SimulationEvent simulationEvent,
            bool isSuccess,
            string message)
        {
            FireSafely(new SimulationRecoveryEvent(
                Time.time,
                action,
                isSuccess,
                simulationEvent?.EventType,
                simulationEvent?.EntityId,
                simulationEvent?.TimeStep ?? -1,
                message));
        }

        private void FireSafely<TEvent>(TEvent eventData) where TEvent : DomainEvent
        {
            try
            {
                m_EventBus.Fire(eventData);
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    $"[AegisFlow] 运行态事件通知失败。Event: {typeof(TEvent).Name}, "
                    + $"Error: {exception.GetType().Name}: {exception.Message}");
            }
        }
    }
}
