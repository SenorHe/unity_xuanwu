using System;
using System.Collections.Generic;
using System.Text;
using AegisFlow.Data;

namespace AegisFlow.Save
{
    /// <summary>
    /// 仿真模型存档数据。集中维护存档结构，避免业务层散落拼 JSON。
    /// </summary>
    public sealed class SimulationModelSaveData
    {
        public const int CurrentSchemaVersion = 2;

        private readonly List<EntityData> m_Entities = new List<EntityData>();

        public string ModelId { get; private set; }
        public bool IsDirty { get; private set; }
        public int SchemaVersion { get; private set; }
        public IReadOnlyList<EntityData> Entities => m_Entities;

        public SimulationModelSaveData(string modelId, bool isDirty, IEnumerable<EntityData> entities)
            : this(modelId, isDirty, entities, CurrentSchemaVersion)
        {
        }

        public SimulationModelSaveData(
            string modelId,
            bool isDirty,
            IEnumerable<EntityData> entities,
            int schemaVersion)
        {
            ModelId = modelId;
            IsDirty = isDirty;
            SchemaVersion = schemaVersion <= 0 ? CurrentSchemaVersion : schemaVersion;

            if (entities == null)
            {
                return;
            }

            foreach (EntityData entityData in entities)
            {
                if (entityData != null)
                {
                    m_Entities.Add(entityData.Clone());
                }
            }
        }

        public static bool TryParse(string json, out SimulationModelSaveData saveData)
        {
            saveData = null;

            if (string.IsNullOrEmpty(json))
            {
                return false;
            }

            int schemaVersion = TryReadInt(json, "schemaVersion");
            if (schemaVersion > CurrentSchemaVersion)
            {
                return false;
            }

            string modelId = ReadValue(json, "modelId");

            if (string.IsNullOrEmpty(modelId))
            {
                return false;
            }

            List<EntityData> entities = ParseEntities(json);
            saveData = new SimulationModelSaveData(modelId, false, entities, schemaVersion);
            return true;
        }

        public string ToJson()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"{{\"schemaVersion\":{SchemaVersion},\"modelId\":\"{SaveJsonUtility.Escape(ModelId)}\",\"isDirty\":{IsDirty.ToString().ToLowerInvariant()},\"entities\":[");

            for (int i = 0; i < m_Entities.Count; i++)
            {
                EntityData entityData = m_Entities[i];

                if (i > 0)
                {
                    builder.Append(',');
                }

                builder.Append($"{{\"entityId\":\"{SaveJsonUtility.Escape(entityData.EntityId)}\"");
                builder.Append($",\"configId\":\"{SaveJsonUtility.Escape(entityData.ConfigId)}\"");
                builder.Append($",\"displayName\":\"{SaveJsonUtility.Escape(entityData.DisplayName)}\"");
                builder.Append($",\"posX\":{entityData.PosX.ToString("F3")}");
                builder.Append($",\"posY\":{entityData.PosY.ToString("F3")}");
                builder.Append($",\"posZ\":{entityData.PosZ.ToString("F3")}");
                builder.Append($",\"rotY\":{entityData.RotY.ToString("F3")}");
                builder.Append($",\"entityType\":\"{SaveJsonUtility.Escape(entityData.EntityType)}\"");
                builder.Append($",\"status\":\"{SaveJsonUtility.Escape(entityData.Status)}\"}}");
            }

            builder.Append("]}");
            return builder.ToString();
        }

        private static List<EntityData> ParseEntities(string json)
        {
            List<EntityData> entities = new List<EntityData>();
            string[] entityBlocks = json.Split(new[] { "{\"entityId\"" }, StringSplitOptions.None);

            for (int i = 1; i < entityBlocks.Length; i++)
            {
                string block = "{\"entityId\"" + entityBlocks[i];
                string entityId = ReadValue(block, "entityId");
                string configId = ReadValue(block, "configId");
                string displayName = ReadValue(block, "displayName");
                float posX = TryReadFloat(block, "posX");
                float posY = TryReadFloat(block, "posY");
                float posZ = TryReadFloat(block, "posZ");
                float rotY = TryReadFloat(block, "rotY");
                string entityType = ReadValue(block, "entityType");
                string status = ReadValue(block, "status");

                if (!string.IsNullOrEmpty(entityId))
                {
                    entities.Add(new EntityData(
                        entityId, configId, displayName,
                        posX, posY, posZ, rotY,
                        entityType, status));
                }
            }

            return entities;
        }

        private static string ReadValue(string json, string key)
        {
            string token = $"\"{key}\":\"";
            int startIndex = json.IndexOf(token, StringComparison.Ordinal);

            if (startIndex < 0)
            {
                return null;
            }

            startIndex += token.Length;
            int endIndex = json.IndexOf("\"", startIndex, StringComparison.Ordinal);

            if (endIndex < 0)
            {
                return null;
            }

            return json.Substring(startIndex, endIndex - startIndex);
        }

        private static float TryReadFloat(string json, string key)
        {
            string token = $"\"{key}\":";
            int startIndex = json.IndexOf(token, StringComparison.Ordinal);

            if (startIndex < 0)
            {
                return 0f;
            }

            startIndex += token.Length;
            int endIndex = startIndex;

            while (endIndex < json.Length && (char.IsDigit(json[endIndex]) || json[endIndex] == '.' || json[endIndex] == '-'))
            {
                endIndex++;
            }

            string numberStr = json.Substring(startIndex, endIndex - startIndex);
            float.TryParse(numberStr, out float result);
            return result;
        }

        private static int TryReadInt(string json, string key)
        {
            string token = $"\"{key}\":";
            int startIndex = json.IndexOf(token, StringComparison.Ordinal);

            if (startIndex < 0)
            {
                return 0;
            }

            startIndex += token.Length;
            int endIndex = startIndex;

            while (endIndex < json.Length && (char.IsDigit(json[endIndex]) || json[endIndex] == '-'))
            {
                endIndex++;
            }

            string numberStr = json.Substring(startIndex, endIndex - startIndex);
            int.TryParse(numberStr, out int result);
            return result;
        }
    }
}
