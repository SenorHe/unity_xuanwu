using System.Collections.Generic;

namespace AegisFlow.Runtime
{
    /// <summary>
    /// 运行态事件策略配置。正式项目可由配置表生成。
    /// </summary>
    public sealed class RuntimeEventStrategyConfig
    {
        private readonly Dictionary<string, string[]> m_EventTypesDic = new Dictionary<string, string[]>();

        public void Register(string configId, params string[] eventTypes)
        {
            if (string.IsNullOrEmpty(configId) || eventTypes == null || eventTypes.Length == 0)
            {
                return;
            }

            m_EventTypesDic[configId] = eventTypes;
        }

        public bool TryGetEventTypes(string configId, out string[] eventTypes)
        {
            return m_EventTypesDic.TryGetValue(configId, out eventTypes);
        }
    }
}
