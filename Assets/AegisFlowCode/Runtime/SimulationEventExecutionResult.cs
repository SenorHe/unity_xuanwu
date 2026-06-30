using AegisFlow.Event;

namespace AegisFlow.Runtime
{
    /// <summary>
    /// 单个仿真事件的结构化执行结果。
    /// </summary>
    public sealed class SimulationEventExecutionResult
    {
        public SimulationEvent SimulationEvent { get; }
        public SimulationEventExecutionStatus Status { get; }
        public int AttemptCount { get; }
        public double DurationMilliseconds { get; }
        public string ErrorMessage { get; }
        public SimulationEventFailureAction FailureAction { get; }
        public bool IsSuccess => Status == SimulationEventExecutionStatus.Succeeded;

        public SimulationEventExecutionResult(
            SimulationEvent simulationEvent,
            SimulationEventExecutionStatus status,
            int attemptCount,
            double durationMilliseconds,
            string errorMessage,
            SimulationEventFailureAction failureAction)
        {
            SimulationEvent = simulationEvent;
            Status = status;
            AttemptCount = attemptCount;
            DurationMilliseconds = durationMilliseconds;
            ErrorMessage = errorMessage;
            FailureAction = failureAction;
        }
    }
}
