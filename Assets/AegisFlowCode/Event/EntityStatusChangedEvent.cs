namespace AegisFlow.Event
{
    /// <summary>
    /// 实体状态变更事件。运行态实体状态变化时触发。
    /// </summary>
    public sealed class EntityStatusChangedEvent : DomainEvent
    {
        public string EntityId { get; private set; }
        public string EntityType { get; private set; }
        public string Status { get; private set; }
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }

        public EntityStatusChangedEvent(
            float createdTime,
            string entityId,
            string entityType,
            string status,
            float posX, float posY, float posZ) : base(createdTime)
        {
            EntityId = entityId;
            EntityType = entityType;
            Status = status;
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
        }
    }
}
