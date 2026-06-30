namespace AegisFlow.Event
{
    /// <summary>
    /// AGV 任务完成事件。
    /// </summary>
    public sealed class AgvTaskCompletedEvent : DomainEvent
    {
        public string AgvId { get; private set; }
        public string TaskType { get; private set; }
        public string TargetEntityId { get; private set; }

        public AgvTaskCompletedEvent(
            float createdTime,
            string agvId,
            string taskType,
            string targetEntityId) : base(createdTime)
        {
            AgvId = agvId;
            TaskType = taskType;
            TargetEntityId = targetEntityId;
        }
    }
}
