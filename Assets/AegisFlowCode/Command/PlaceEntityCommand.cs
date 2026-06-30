using AegisFlow.Data;

namespace AegisFlow.Command
{
    /// <summary>
    /// 带位置的实体放置命令。
    /// </summary>
    public sealed class PlaceEntityCommand : UICommand
    {
        public string EntityType { get; private set; }
        public string EntityId { get; private set; }
        public string DisplayName { get; private set; }
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }
        public float RotY { get; private set; }

        public PlaceEntityCommand(
            string entityType,
            string entityId,
            string displayName,
            float posX, float posY, float posZ,
            float rotY = 0f) : base("PlaceEntity")
        {
            EntityType = entityType;
            EntityId = entityId;
            DisplayName = displayName;
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
            RotY = rotY;
        }

        public EntityData ToEntityData()
        {
            return new EntityData(
                EntityId, EntityType, DisplayName,
                PosX, PosY, PosZ, RotY,
                EntityType, "Placed");
        }
    }
}
