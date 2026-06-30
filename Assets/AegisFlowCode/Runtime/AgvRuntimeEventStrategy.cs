using AegisFlow.Data;

namespace AegisFlow.Runtime
{
    /// <summary>
    /// AGV 实体运行态事件策略示例。
    /// </summary>
    public sealed class AgvRuntimeEventStrategy : IRuntimeEventStrategy
    {
        public bool CanHandle(EntityData entityData)
        {
            return entityData != null && entityData.ConfigId == "AGV";
        }

        public void Build(EntityData entityData, int baseStep, SimulationEventQueue eventQueue)
        {
            eventQueue.Enqueue(new SimulationEvent("Creating", baseStep, entityData.EntityId));
            eventQueue.Enqueue(new SimulationEvent("Activating", baseStep + 1, entityData.EntityId));
            eventQueue.Enqueue(new SimulationEvent("Completed", baseStep + 3, entityData.EntityId));
        }
    }
}
