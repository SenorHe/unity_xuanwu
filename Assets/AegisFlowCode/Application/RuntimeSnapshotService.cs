using AegisFlow.Data;

namespace AegisFlow.Application
{
    /// <summary>
    /// 运行态快照服务。负责将编辑态数据转换为运行态快照。
    /// </summary>
    public sealed class RuntimeSnapshotService
    {
        private readonly EntityRepository m_EntityRepository;

        public RuntimeSnapshotService(EntityRepository entityRepository)
        {
            m_EntityRepository = entityRepository;
        }

        public RuntimeSnapshot BuildSnapshot(string modelId)
        {
            RuntimeSnapshot snapshot = new RuntimeSnapshot(modelId);

            foreach (EntityData entityData in m_EntityRepository.GetAll())
            {
                snapshot.AddEntity(entityData.Clone());
            }

            return snapshot;
        }
    }
}
