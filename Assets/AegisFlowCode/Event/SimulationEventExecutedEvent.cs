namespace AegisFlow.Event
{
    /// <summary>
    /// 仿真事件执行观测事件，只携带跨模块稳定字段。
    /// </summary>
    public sealed class SimulationEventExecutedEvent : DomainEvent
    {
        public string EventType { get; private set; }
        public string EntityId { get; private set; }
        public int TimeStep { get; private set; }
        public SimulationEventExecutionStatus Status { get; private set; }
        public int AttemptCount { get; private set; }
        public double DurationMilliseconds { get; private set; }
        public string ErrorMessage { get; private set; }

        public SimulationEventExecutedEvent(
            float createdTime,
            string eventType,
            string entityId,
            int timeStep,
            SimulationEventExecutionStatus status,
            int attemptCount,
            double durationMilliseconds,
            string errorMessage) : base(createdTime)
        {
            EventType = eventType;
            EntityId = entityId;
            TimeStep = timeStep;
            Status = status;
            AttemptCount = attemptCount;
            DurationMilliseconds = durationMilliseconds;
            ErrorMessage = errorMessage;
        }
    }
}
