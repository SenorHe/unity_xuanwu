using System.Collections.Generic;

namespace AegisFlow.Factory
{
    /// <summary>
    /// 实体分组解析器。替代 GenerateEntityFactory.GetGroup 中硬编码和不可达分支。
    /// </summary>
    public sealed class EntityGroupResolver
    {
        private readonly Dictionary<string, string> m_GroupMap = new Dictionary<string, string>();

        public EntityGroupResolver()
        {
            m_GroupMap["AGV"] = "AgvGroup";
            m_GroupMap["RACK"] = "RackGroup";
            m_GroupMap["AGV_RACK"] = "AgvRackGroup";
        }

        public string ResolveGroup(string configId)
        {
            if (string.IsNullOrEmpty(configId))
            {
                return "DefaultGroup";
            }

            return m_GroupMap.TryGetValue(configId, out string groupName) ? groupName : "DefaultGroup";
        }

        public void Register(string configId, string groupName)
        {
            if (!string.IsNullOrEmpty(configId) && !string.IsNullOrEmpty(groupName))
            {
                m_GroupMap[configId] = groupName;
            }
        }
    }
}
