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
        public const int CurrentSchemaVersion = 1;

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

            string modelId = ReadValue(json, "modelId");

            if (string.IsNullOrEmpty(modelId))
            {
                return false;
            }

            List<EntityData> entities = ParseEntities(json);
            saveData = new SimulationModelSaveData(modelId, false, entities);
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

                builder.Append($"{{\"entityId\":\"{SaveJsonUtility.Escape(entityData.EntityId)}\",\"configId\":\"{SaveJsonUtility.Escape(entityData.ConfigId)}\",\"displayName\":\"{SaveJsonUtility.Escape(entityData.DisplayName)}\"}}");
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

                if (!string.IsNullOrEmpty(entityId))
                {
                    entities.Add(new EntityData(entityId, configId, displayName));
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
    }
}
