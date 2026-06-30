namespace AegisFlow.Runtime
{
    /// <summary>
    /// 同步仿真事件失败策略。重试发生在同一帧，不包含延时重试。
    /// </summary>
    public sealed class SimulationEventExecutionPolicy
    {
        public int MaxRetryCount { get; }
        public SimulationEventFailureAction FailureAction { get; }

        public SimulationEventExecutionPolicy(
            int maxRetryCount = 0,
            SimulationEventFailureAction failureAction = SimulationEventFailureAction.PauseRuntime)
        {
            MaxRetryCount = maxRetryCount < 0 ? 0 : maxRetryCount;
            FailureAction = failureAction;
        }
    }
}
