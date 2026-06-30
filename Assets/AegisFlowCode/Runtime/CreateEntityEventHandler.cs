using UnityEngine;

namespace AegisFlow.Runtime
{
    /// <summary>
    /// 创建实体事件 Handler 示例。
    /// </summary>
    public sealed class CreateEntityEventHandler : ISimulationEventHandler
    {
        public string EventType => "Creating";

        public bool Execute(SimulationEvent simulationEvent)
        {
            Debug.Log($"[AegisFlow] Execute Creating Event, EntityId: {simulationEvent.EntityId}");
            return true;
        }
    }
}
