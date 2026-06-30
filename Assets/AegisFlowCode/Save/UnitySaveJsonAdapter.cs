using System;
using System.Collections.Generic;
using AegisFlow.Data;
using UnityEngine;

namespace AegisFlow.Save
{
    /// <summary>
    /// 基于 Unity JsonUtility 的版本化存档适配器。
    /// </summary>
    public sealed class UnitySaveJsonAdapter : ISaveJsonAdapter
    {
        public string ToJson(SimulationModelSaveData saveData)
        {
            if (saveData == null || string.IsNullOrEmpty(saveData.ModelId))
            {
                return null;
            }

            SaveDocument document = new SaveDocument
            {
                schemaVersion = SimulationModelSaveData.CurrentSchemaVersion,
                modelId = saveData.ModelId,
                isDirty = saveData.IsDirty,
                entities = BuildEntityDocuments(saveData.Entities)
            };

            return JsonUtility.ToJson(document);
        }

        public bool TryFromJson(string json, out SimulationModelSaveData saveData)
        {
            saveData = null;

            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            try
            {
                SaveDocument document = JsonUtility.FromJson<SaveDocument>(json);

                if (document == null || string.IsNullOrEmpty(document.modelId))
                {
                    return false;
                }

                int schemaVersion = document.schemaVersion == 0 ? 1 : document.schemaVersion;

                if (schemaVersion > SimulationModelSaveData.CurrentSchemaVersion)
                {
                    return false;
                }

                saveData = new SimulationModelSaveData(
                    document.modelId,
                    document.isDirty,
                    BuildEntities(document.entities),
                    schemaVersion);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private static EntityDocument[] BuildEntityDocuments(IReadOnlyList<EntityData> entities)
        {
            EntityDocument[] documents = new EntityDocument[entities.Count];

            for (int i = 0; i < entities.Count; i++)
            {
                EntityData entity = entities[i];
                documents[i] = new EntityDocument
                {
                    entityId = entity.EntityId,
                    configId = entity.ConfigId,
                    displayName = entity.DisplayName,
                    posX = entity.PosX,
                    posY = entity.PosY,
                    posZ = entity.PosZ,
                    rotY = entity.RotY,
                    entityType = entity.EntityType,
                    status = entity.Status
                };
            }

            return documents;
        }

        private static List<EntityData> BuildEntities(EntityDocument[] documents)
        {
            List<EntityData> entities = new List<EntityData>();

            if (documents == null)
            {
                return entities;
            }

            for (int i = 0; i < documents.Length; i++)
            {
                EntityDocument document = documents[i];

                if (document != null && !string.IsNullOrEmpty(document.entityId))
                {
                    entities.Add(new EntityData(
                        document.entityId,
                        document.configId,
                        document.displayName,
                        document.posX,
                        document.posY,
                        document.posZ,
                        document.rotY,
                        document.entityType,
                        document.status));
                }
            }

            return entities;
        }

        [Serializable]
        private sealed class SaveDocument
        {
            public int schemaVersion;
            public string modelId;
            public bool isDirty;
            public EntityDocument[] entities;
        }

        [Serializable]
        private sealed class EntityDocument
        {
            public string entityId;
            public string configId;
            public string displayName;
            public float posX;
            public float posY;
            public float posZ;
            public float rotY;
            public string entityType;
            public string status;
        }
    }
}
