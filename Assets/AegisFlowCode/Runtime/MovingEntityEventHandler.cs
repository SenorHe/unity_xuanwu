using UnityEngine;

namespace AegisFlow.Runtime
{
    /// <summary>
    /// AGV 移动事件处理器。运行态 AGV 开始移动时触发。
    /// </summary>
    public sealed class MovingEntityEventHandler : ISimulationEventHandler
    {
        public string EventType => "Moving";

        public bool Execute(SimulationEvent simulationEvent)
        {
            Debug.Log($"[AegisFlow] AGV Moving, EntityId: {simulationEvent.EntityId}, Step: {simulationEvent.TimeStep}");
            return true;
        }
    }
}
