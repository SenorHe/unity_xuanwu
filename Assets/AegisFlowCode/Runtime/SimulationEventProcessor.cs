using System;
using System.Collections.Generic;
using AegisFlow.Event;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;
using Debug = UnityEngine.Debug;

namespace AegisFlow.Runtime
{
    /// <summary>
    /// Executes simulation events and reports structured results according to the retry policy.
    /// </summary>
    public sealed class SimulationEventProcessor
    {
        private readonly Dictionary<string, ISimulationEventHandler> m_HandlerDic =
            new Dictionary<string, ISimulationEventHandler>();
        private readonly SimulationEventExecutionPolicy m_ExecutionPolicy;

        public SimulationEventProcessor(SimulationEventExecutionPolicy executionPolicy = null)
        {
            m_ExecutionPolicy = executionPolicy ?? new SimulationEventExecutionPolicy();
        }

        public void Register(ISimulationEventHandler handler)
        {
            if (handler == null || string.IsNullOrEmpty(handler.EventType))
            {
                return;
            }

            m_HandlerDic[handler.EventType] = handler;
        }

        public SimulationEventExecutionResult ExecuteEvent(SimulationEvent simulationEvent)
        {
            if (simulationEvent == null)
            {
                return CreateResult(null, SimulationEventExecutionStatus.InvalidEvent, 0, 0d, null);
            }

            if (!m_HandlerDic.TryGetValue(simulationEvent.EventType, out ISimulationEventHandler handler))
            {
                Debug.LogWarning($"[AegisFlow] Simulation event handler not found: {simulationEvent.EventType}");
                return CreateResult(
                    simulationEvent,
                    SimulationEventExecutionStatus.HandlerNotFound,
                    0,
                    0d,
                    null);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            int attemptCount = 0;
            SimulationEventExecutionStatus status = SimulationEventExecutionStatus.HandlerReturnedFailure;
            string errorMessage = null;

            for (int attempt = 0; attempt <= m_ExecutionPolicy.MaxRetryCount; attempt++)
            {
                attemptCount++;

                try
                {
                    if (handler.Execute(simulationEvent))
                    {
                        status = SimulationEventExecutionStatus.Succeeded;
                        errorMessage = null;
                        break;
                    }

                    status = SimulationEventExecutionStatus.HandlerReturnedFailure;
                }
                catch (Exception exception)
                {
                    status = SimulationEventExecutionStatus.HandlerException;
                    errorMessage = $"{exception.GetType().Name}: {exception.Message}";
                }
            }

            stopwatch.Stop();
            return CreateResult(
                simulationEvent,
                status,
                attemptCount,
                stopwatch.Elapsed.TotalMilliseconds,
                errorMessage);
        }

        public void Clear()
        {
            m_HandlerDic.Clear();
        }

        private SimulationEventExecutionResult CreateResult(
            SimulationEvent simulationEvent,
            SimulationEventExecutionStatus status,
            int attemptCount,
            double durationMilliseconds,
            string errorMessage)
        {
            return new SimulationEventExecutionResult(
                simulationEvent,
                status,
                attemptCount,
                durationMilliseconds,
                errorMessage,
                m_ExecutionPolicy.FailureAction);
        }
    }
}
