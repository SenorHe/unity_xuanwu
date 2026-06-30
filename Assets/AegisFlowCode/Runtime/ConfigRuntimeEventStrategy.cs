using AegisFlow.Data;

namespace AegisFlow.Runtime
{
    /// <summary>
    /// 配置驱动运行态事件策略。按 ConfigId 查表生成事件序列。
    /// </summary>
    public sealed class ConfigRuntimeEventStrategy : IRuntimeEventStrategy
    {
        private readonly RuntimeEventStrategyConfig m_Config;

        public ConfigRuntimeEventStrategy(RuntimeEventStrategyConfig config)
        {
            m_Config = config;
        }

        public bool CanHandle(EntityData entityData)
        {
            return entityData != null && m_Config.TryGetEventTypes(entityData.ConfigId, out _);
        }

        public void Build(EntityData entityData, int baseStep, SimulationEventQueue eventQueue)
        {
            if (!m_Config.TryGetEventTypes(entityData.ConfigId, out string[] eventTypes))
            {
                return;
            }

            for (int i = 0; i < eventTypes.Length; i++)
            {
                eventQueue.Enqueue(new SimulationEvent(eventTypes[i], baseStep + i, entityData.EntityId));
            }
        }
    }
}
