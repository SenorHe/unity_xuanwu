using UnityEngine;

namespace AegisFlowDigitalTwin.Visual
{
    /// <summary>
    /// 按 EntityType 创建对应 EntityVisualBase 的工厂。
    /// </summary>
    public static class EntityVisualFactory
    {
        public static EntityVisualBase Create(string entityId, string entityType, Transform parent)
        {
            GameObject obj = new GameObject($"Entity_{entityType}_{entityId}");
            obj.transform.SetParent(parent);

            EntityVisualBase visual = AddVisualComponent(obj, entityType);
            visual.Initialize(entityId, entityType);

            return visual;
        }

        private static EntityVisualBase AddVisualComponent(GameObject obj, string entityType)
        {
            switch (entityType)
            {
                case "AGV":
                    return obj.AddComponent<AgvVisual>();
                case "RACK":
                    return obj.AddComponent<RackVisual>();
                case "CHARGER":
                    return obj.AddComponent<ChargerVisual>();
                case "CONVEYOR":
                    return obj.AddComponent<ConveyorVisual>();
                case "WORKSTATION":
                    return obj.AddComponent<WorkStationVisual>();
                case "SENSOR":
                    return obj.AddComponent<SensorVisual>();
                default:
                    return obj.AddComponent<AgvVisual>();
            }
        }
    }
}
