using AegisFlow.Data;

namespace AegisFlow.Domain
{
    /// <summary>
    /// 数字孪生领域服务。负责实体放置校验、移动校验等业务规则。
    /// </summary>
    public sealed class TwinDomainService
    {
        private readonly EntityRepository m_EntityRepository;
        private readonly ModelRepository m_ModelRepository;

        public TwinDomainService(
            EntityRepository entityRepository,
            ModelRepository modelRepository)
        {
            m_EntityRepository = entityRepository;
            m_ModelRepository = modelRepository;
        }

        public bool CanPlace(string entityType, float x, float z)
        {
            if (string.IsNullOrEmpty(entityType))
            {
                return false;
            }

            if (x < -100f || x > 100f || z < -100f || z > 100f)
            {
                return false;
            }

            return true;
        }

        public bool CanMove(string entityId, float x, float z)
        {
            if (string.IsNullOrEmpty(entityId))
            {
                return false;
            }

            if (!m_EntityRepository.Exists(entityId))
            {
                return false;
            }

            if (x < -100f || x > 100f || z < -100f || z > 100f)
            {
                return false;
            }

            return true;
        }

        public bool MoveEntity(string entityId, float x, float y, float z, float rotY)
        {
            if (!CanMove(entityId, x, z))
            {
                return false;
            }

            EntityData entity = m_EntityRepository.Get(entityId);
            if (entity == null)
            {
                return false;
            }

            entity.SetPosition(x, y, z);
            entity.SetRotationY(rotY);
            return true;
        }

        public EntityData GetEntity(string entityId)
        {
            if (string.IsNullOrEmpty(entityId))
            {
                return null;
            }

            return m_EntityRepository.Get(entityId);
        }

        public bool HasEntities()
        {
            return m_EntityRepository.Count > 0;
        }

        public int GetEntityCount()
        {
            return m_EntityRepository.Count;
        }

        public bool HasEntityType(string entityType)
        {
            System.Collections.Generic.IReadOnlyList<EntityData> entities = m_EntityRepository.GetAll();
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].EntityType == entityType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
