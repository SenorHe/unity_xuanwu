namespace AegisFlow.Event
{
    /// <summary>
    /// 仿真失败恢复审计事件。
    /// </summary>
    public sealed class SimulationRecoveryEvent : DomainEvent
    {
        public SimulationRecoveryAction Action { get; private set; }
        public bool IsSuccess { get; private set; }
        public string EventType { get; private set; }
        public string EntityId { get; private set; }
        public int TimeStep { get; private set; }
        public string Message { get; private set; }

        public SimulationRecoveryEvent(
            float createdTime,
            SimulationRecoveryAction action,
            bool isSuccess,
            string eventType,
            string entityId,
            int timeStep,
            string message) : base(createdTime)
        {
            Action = action;
            IsSuccess = isSuccess;
            EventType = eventType;
            EntityId = entityId;
            TimeStep = timeStep;
            Message = message;
        }
    }
}
