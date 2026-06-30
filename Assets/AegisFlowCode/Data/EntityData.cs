using System;

namespace AegisFlow.Data
{
    /// <summary>
    /// 编辑态实体数据。包含空间位置、类型与运行态状态。
    /// </summary>
    public sealed class EntityData
    {
        public string EntityId { get; private set; }
        public string ConfigId { get; private set; }
        public string DisplayName { get; private set; }

        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }
        public float RotY { get; private set; }

        public string EntityType { get; private set; }
        public string Status { get; private set; }

        public EntityData(string entityId, string configId, string displayName)
            : this(entityId, configId, displayName, 0f, 0f, 0f, 0f, configId, "Idle")
        {
        }

        public EntityData(
            string entityId,
            string configId,
            string displayName,
            float posX, float posY, float posZ,
            float rotY,
            string entityType,
            string status)
        {
            EntityId = entityId;
            ConfigId = configId;
            DisplayName = displayName;
            PosX = posX;
            PosY = posY;
            PosZ = posZ;
            RotY = rotY;
            EntityType = string.IsNullOrEmpty(entityType) ? configId : entityType;
            Status = string.IsNullOrEmpty(status) ? "Idle" : status;
        }

        public void SetPosition(float x, float y, float z)
        {
            PosX = x;
            PosY = y;
            PosZ = z;
        }

        public void SetRotationY(float rotY)
        {
            RotY = rotY;
        }

        public void SetStatus(string status)
        {
            Status = string.IsNullOrEmpty(status) ? "Idle" : status;
        }

        public EntityData Clone()
        {
            return new EntityData(
                EntityId, ConfigId, DisplayName,
                PosX, PosY, PosZ, RotY,
                EntityType, Status);
        }
    }
}
