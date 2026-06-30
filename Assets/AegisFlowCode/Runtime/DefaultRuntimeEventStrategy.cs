using AegisFlow.Data;

namespace AegisFlow.Runtime
{
    /// <summary>
    /// 默认运行态事件策略。无法匹配专用策略时使用。
    /// </summary>
    public sealed class DefaultRuntimeEventStrategy : IRuntimeEventStrategy
    {
        public bool CanHandle(EntityData entityData)
        {
            return entityData != null;
        }

        public void Build(EntityData entityData, int baseStep, SimulationEventQueue eventQueue)
        {
            eventQueue.Enqueue(new SimulationEvent("Creating", baseStep, entityData.EntityId));
            eventQueue.Enqueue(new SimulationEvent("Activating", baseStep + 1, entityData.EntityId));
            eventQueue.Enqueue(new SimulationEvent("Completed", baseStep + 2, entityData.EntityId));
        }
    }
}
