using System.Collections.Generic;

namespace AegisFlow.Data
{
    /// <summary>
    /// 实体仓储。只做数据容器，不做业务规则、不刷新 UI。
    /// </summary>
    public sealed class EntityRepository
    {
        private readonly Dictionary<string, EntityData> m_EntityDic = new Dictionary<string, EntityData>();

        public int Count => m_EntityDic.Count;

        public bool Exists(string entityId)
        {
            return !string.IsNullOrEmpty(entityId) && m_EntityDic.ContainsKey(entityId);
        }

        public void Add(EntityData entityData)
        {
            m_EntityDic[entityData.EntityId] = entityData;
        }

        public bool Remove(string entityId)
        {
            return m_EntityDic.Remove(entityId);
        }

        public EntityData Get(string entityId)
        {
            m_EntityDic.TryGetValue(entityId, out EntityData entityData);
            return entityData;
        }

        public IEnumerable<EntityData> GetAll()
        {
            return m_EntityDic.Values;
        }

        public void Clear()
        {
            m_EntityDic.Clear();
        }
    }
}
