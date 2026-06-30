using AegisFlow.Data;

namespace AegisFlow.Factory
{
    /// <summary>
    /// 实体恢复服务。负责 JSON / AI / 服务端数据恢复，不放在 EntityFactory 内。
    /// </summary>
    public sealed class EntityRestoreService
    {
        private readonly EntityRepository m_EntityRepository;

        public EntityRestoreService(EntityRepository entityRepository)
        {
            m_EntityRepository = entityRepository;
        }

        public void Restore(EntityData entityData)
        {
            if (entityData == null)
            {
                return;
            }

            if (!m_EntityRepository.Exists(entityData.EntityId))
            {
                m_EntityRepository.Add(entityData);
            }
        }
    }
}
