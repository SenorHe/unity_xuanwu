using AegisFlow.Data;

namespace AegisFlow.Domain
{
    /// <summary>
    /// 实体领域服务。负责实体新增、删除、重命名等业务规则。
    /// </summary>
    public sealed class EntityDomainService
    {
        private readonly EntityRepository m_EntityRepository;

        public EntityDomainService(EntityRepository entityRepository)
        {
            m_EntityRepository = entityRepository;
        }

        public bool CreateEntity(EntityData entityData)
        {
            if (entityData == null || string.IsNullOrEmpty(entityData.EntityId))
            {
                return false;
            }

            if (m_EntityRepository.Exists(entityData.EntityId))
            {
                return false;
            }

            m_EntityRepository.Add(entityData);
            return true;
        }

        public bool RemoveEntity(string entityId)
        {
            if (string.IsNullOrEmpty(entityId))
            {
                return false;
            }

            return m_EntityRepository.Remove(entityId);
        }
    }
}
