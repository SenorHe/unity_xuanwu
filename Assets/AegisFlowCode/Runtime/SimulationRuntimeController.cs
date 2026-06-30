using AegisFlow.Data;
using AegisFlow.Event;
using UnityEngine;

namespace AegisFlow.Runtime
{
    /// <summary>
    /// 仿真运行态控制器。负责加载快照和驱动运行态调度。
    /// </summary>
    public sealed class SimulationRuntimeController
    {
        private readonly RuntimeDC m_RuntimeDC;
        private readonly SimulationEventQueue m_EventQueue;
        private readonly RuntimeEventQueueBuilder m_EventQueueBuilder;
        private readonly SimulationStepScheduler m_StepScheduler;
        private RuntimeSnapshot m_CurrentSnapshot;

        public bool HasPendingFailure => m_StepScheduler.HasPendingFailure;

        public SimulationRuntimeController(
            RuntimeDC runtimeDC,
            SimulationEventProcessor eventProcessor,
            SimulationEventQueue eventQueue,
            RuntimeEventQueueBuilder eventQueueBuilder,
            DomainEventBus eventBus)
        {
            m_RuntimeDC = runtimeDC;
            m_EventQueue = eventQueue;
            m_EventQueueBuilder = eventQueueBuilder;
            m_StepScheduler = new SimulationStepScheduler(runtimeDC, eventProcessor, eventQueue, eventBus);
        }

        public void Load(RuntimeSnapshot snapshot)
        {
            if (snapshot == null)
            {
                Debug.LogError("[AegisFlow] RuntimeSnapshot 为空，无法进入运行态。");
                return;
            }

            m_CurrentSnapshot = snapshot;
            m_StepScheduler.Reset();
            m_EventQueueBuilder.Build(snapshot, m_EventQueue);
            m_RuntimeDC.StartRuntime(snapshot.ModelId);
            Debug.Log($"[AegisFlow] Load RuntimeSnapshot: {snapshot.ModelId}, Entities: {snapshot.Entities.Count}");
        }

        public void Tick(float deltaTime)
        {
            if (m_CurrentSnapshot == null)
            {
                return;
            }

            m_StepScheduler.Tick(deltaTime);
        }

        public bool RetryFailedEvent()
        {
            return m_CurrentSnapshot != null && m_StepScheduler.RetryFailedEvent();
        }

        public bool SkipFailedEvent()
        {
            return m_CurrentSnapshot != null && m_StepScheduler.SkipFailedEvent();
        }

        public bool TerminateFailedRuntime()
        {
            if (m_CurrentSnapshot == null || !m_StepScheduler.TerminateFailedRuntime())
            {
                return false;
            }

            Stop();
            return true;
        }

        public void Stop()
        {
            m_CurrentSnapshot = null;
            m_StepScheduler.Reset();
            m_EventQueue.Clear();
            m_RuntimeDC.Stop();
        }
    }
}
