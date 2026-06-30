namespace AegisFlow.Data
{
    /// <summary>
    /// UI 展示用实体视图数据。只读，由 Presenter 从 EntityData 派生。
    /// </summary>
    public sealed class EntityViewData
    {
        public string EntityId { get; private set; }
        public string EntityType { get; private set; }
        public string DisplayName { get; private set; }
        public string Status { get; private set; }
        public float PosX { get; private set; }
        public float PosZ { get; private set; }
        public float Battery { get; private set; }

        public EntityViewData(
            string entityId,
            string entityType,
            string displayName,
            string status,
            float posX,
            float posZ,
            float battery)
        {
            EntityId = entityId;
            EntityType = entityType;
            DisplayName = displayName;
            Status = status;
            PosX = posX;
            PosZ = posZ;
            Battery = battery;
        }

        public static EntityViewData FromEntityData(EntityData data, float battery = 100f)
        {
            return new EntityViewData(
                data.EntityId,
                data.EntityType,
                data.DisplayName,
                data.Status,
                data.PosX,
                data.PosZ,
                battery);
        }
    }
}
