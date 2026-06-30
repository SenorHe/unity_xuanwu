using System.Collections.Generic;

namespace AegisFlow.Data
{
    /// <summary>
    /// 数字孪生遥测数据中心。运行态实时记录实体位置、电量、速度等遥测信息。
    /// </summary>
    public sealed class TwinDC : DataCenterBase
    {
        private readonly Dictionary<string, EntityTelemetry> m_Telemetries = new Dictionary<string, EntityTelemetry>();

        public IReadOnlyDictionary<string, EntityTelemetry> Telemetries => m_Telemetries;

        public void UpdateTelemetry(
            string entityId,
            float posX, float posY, float posZ,
            float battery,
            string status,
            string targetEntityId,
            float speed)
        {
            if (string.IsNullOrEmpty(entityId))
            {
                return;
            }

            if (!m_Telemetries.TryGetValue(entityId, out EntityTelemetry telemetry))
            {
                telemetry = new EntityTelemetry(entityId);
                m_Telemetries[entityId] = telemetry;
            }

            telemetry.SetPosition(posX, posY, posZ);
            telemetry.Battery = battery;
            telemetry.Status = status;
            telemetry.TargetEntityId = targetEntityId;
            telemetry.Speed = speed;
            Save();
        }

        public void UpdateStatus(string entityId, string status)
        {
            if (!m_Telemetries.TryGetValue(entityId, out EntityTelemetry telemetry))
            {
                return;
            }

            telemetry.Status = status;
            Save();
        }

        public void UpdatePosition(string entityId, float x, float y, float z)
        {
            if (!m_Telemetries.TryGetValue(entityId, out EntityTelemetry telemetry))
            {
                return;
            }

            telemetry.SetPosition(x, y, z);
            Save();
        }

        public bool TryGetTelemetry(string entityId, out EntityTelemetry telemetry)
        {
            return m_Telemetries.TryGetValue(entityId, out telemetry);
        }

        public void RemoveTelemetry(string entityId)
        {
            if (m_Telemetries.Remove(entityId))
            {
                Save();
            }
        }

        public void Clear()
        {
            m_Telemetries.Clear();
            Save();
        }
    }

    /// <summary>
    /// 单个实体的运行态遥测数据。
    /// </summary>
    public sealed class EntityTelemetry
    {
        public string EntityId { get; private set; }
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }
        public float Battery { get; internal set; }
        public string Status { get; internal set; }
        public string TargetEntityId { get; internal set; }
        public float Speed { get; internal set; }

        public EntityTelemetry(string entityId)
        {
            EntityId = entityId;
            Battery = 100f;
            Status = "Idle";
            TargetEntityId = null;
            Speed = 0f;
        }

        internal void SetPosition(float x, float y, float z)
        {
            PosX = x;
            PosY = y;
            PosZ = z;
        }
    }
}
