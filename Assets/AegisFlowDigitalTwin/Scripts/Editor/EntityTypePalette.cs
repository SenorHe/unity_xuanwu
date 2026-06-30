using UnityEngine;

namespace AegisFlowDigitalTwin.Editor
{
    /// <summary>
    /// 实体类型选择面板。提供 6 种实体类型的快速选择。
    /// </summary>
    public static class EntityTypePalette
    {
        public static readonly string[] EntityTypes =
        {
            "AGV",
            "RACK",
            "CHARGER",
            "CONVEYOR",
            "WORKSTATION",
            "SENSOR"
        };

        public static readonly string[] DisplayNames =
        {
            "AGV 搬运车",
            "货架",
            "充电站",
            "传送带",
            "工作站",
            "传感器"
        };

        public static readonly Color[] TypeColors =
        {
            new Color(0.15f, 0.45f, 0.85f),
            new Color(0.85f, 0.45f, 0.1f),
            new Color(0.1f, 0.6f, 0.3f),
            new Color(0.4f, 0.4f, 0.42f),
            new Color(0.4f, 0.2f, 0.6f),
            new Color(0.1f, 0.6f, 0.7f)
        };

        public static int TypeCount => EntityTypes.Length;

        public static string GetDisplayName(string entityType)
        {
            for (int i = 0; i < EntityTypes.Length; i++)
            {
                if (EntityTypes[i] == entityType)
                {
                    return DisplayNames[i];
                }
            }

            return entityType;
        }

        public static Color GetColor(string entityType)
        {
            for (int i = 0; i < EntityTypes.Length; i++)
            {
                if (EntityTypes[i] == entityType)
                {
                    return TypeColors[i];
                }
            }

            return Color.white;
        }
    }
}
