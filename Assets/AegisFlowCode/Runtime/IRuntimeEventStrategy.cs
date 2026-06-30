using AegisFlow.Data;

namespace AegisFlow.Runtime
{
    /// <summary>
    /// 运行态事件策略。按实体类型决定如何生成仿真事件。
    /// </summary>
    public interface IRuntimeEventStrategy
    {
        bool CanHandle(EntityData entityData);

        void Build(EntityData entityData, int baseStep, SimulationEventQueue eventQueue);
    }
}
