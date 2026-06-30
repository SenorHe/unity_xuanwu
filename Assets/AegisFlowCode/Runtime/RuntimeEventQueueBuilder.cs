using System.Collections.Generic;
using AegisFlow.Data;

namespace AegisFlow.Runtime
{
    /// <summary>
    /// 运行态事件队列构建服务。负责从 RuntimeSnapshot 派生 time_step 事件队列。
    /// </summary>
    public sealed class RuntimeEventQueueBuilder
    {
        private readonly List<IRuntimeEventStrategy> m_Strategies = new List<IRuntimeEventStrategy>();
        private readonly IRuntimeEventStrategy m_DefaultStrategy = new DefaultRuntimeEventStrategy();

        public void RegisterStrategy(IRuntimeEventStrategy strategy)
        {
            if (strategy == null || m_Strategies.Contains(strategy))
            {
                return;
            }

            m_Strategies.Add(strategy);
        }

        public void Build(RuntimeSnapshot snapshot, SimulationEventQueue eventQueue)
        {
            if (snapshot == null || eventQueue == null)
            {
                return;
            }

            eventQueue.Clear();

            for (int i = 0; i < snapshot.Entities.Count; i++)
            {
                EntityData entityData = snapshot.Entities[i];
                int baseStep = i * 3 + 1;
                ResolveStrategy(entityData).Build(entityData, baseStep, eventQueue);
            }
        }

        private IRuntimeEventStrategy ResolveStrategy(EntityData entityData)
        {
            for (int i = 0; i < m_Strategies.Count; i++)
            {
                if (m_Strategies[i].CanHandle(entityData))
                {
                    return m_Strategies[i];
                }
            }

            return m_DefaultStrategy;
        }
    }
}
