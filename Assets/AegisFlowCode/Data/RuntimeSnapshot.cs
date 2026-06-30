using System.Collections.Generic;
using AegisFlow.Data;

namespace AegisFlow.Data
{
    /// <summary>
    /// 运行态快照。运行态只读快照，不直接污染编辑态数据。
    /// </summary>
    public sealed class RuntimeSnapshot
    {
        private readonly List<EntityData> m_Entities = new List<EntityData>();

        public string ModelId { get; private set; }
        public IReadOnlyList<EntityData> Entities => m_Entities;

        public RuntimeSnapshot(string modelId)
        {
            ModelId = modelId;
        }

        public void AddEntity(EntityData entityData)
        {
            if (entityData != null)
            {
                m_Entities.Add(entityData);
            }
        }
    }
}
