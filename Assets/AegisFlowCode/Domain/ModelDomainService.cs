using AegisFlow.Data;

namespace AegisFlow.Domain
{
    /// <summary>
    /// 模型领域服务。负责模型级业务规则，不直接操作 UI。
    /// </summary>
    public sealed class ModelDomainService
    {
        private readonly ModelRepository m_ModelRepository;

        public ModelDomainService(ModelRepository modelRepository)
        {
            m_ModelRepository = modelRepository;
        }

        public bool CanRun(string modelId)
        {
            if (string.IsNullOrEmpty(modelId))
            {
                return false;
            }

            return m_ModelRepository.Exists(modelId);
        }

        public void Register(string modelId)
        {
            if (!string.IsNullOrEmpty(modelId))
            {
                m_ModelRepository.Add(modelId);
            }
        }
    }
}
