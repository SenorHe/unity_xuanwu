using System.Collections.Generic;

namespace AegisFlow.Data
{
    /// <summary>
    /// 模型仓储。只保存模型元数据，不处理模型业务规则。
    /// </summary>
    public sealed class ModelRepository
    {
        private readonly HashSet<string> m_ModelIds = new HashSet<string>();

        public bool Exists(string modelId)
        {
            return !string.IsNullOrEmpty(modelId) && m_ModelIds.Contains(modelId);
        }

        public void Add(string modelId)
        {
            if (!string.IsNullOrEmpty(modelId))
            {
                m_ModelIds.Add(modelId);
            }
        }

        public bool Remove(string modelId)
        {
            return m_ModelIds.Remove(modelId);
        }
    }
}
