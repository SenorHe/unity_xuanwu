namespace AegisFlow.Data
{
    /// <summary>
    /// 编辑态实体数据。这里只保留架构骨架字段，业务字段按项目扩展。
    /// </summary>
    public sealed class EntityData
    {
        public string EntityId { get; private set; }
        public string ConfigId { get; private set; }
        public string DisplayName { get; private set; }

        public EntityData(string entityId, string configId, string displayName)
        {
            EntityId = entityId;
            ConfigId = configId;
            DisplayName = displayName;
        }

        public EntityData Clone()
        {
            return new EntityData(EntityId, ConfigId, DisplayName);
        }
    }
}
