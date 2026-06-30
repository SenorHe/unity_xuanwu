using UnityEngine;

namespace AegisFlow.Runtime
{
    /// <summary>
    /// 激活实体事件 Handler 示例。
    /// </summary>
    public sealed class ActivateEntityEventHandler : ISimulationEventHandler
    {
        public string EventType => "Activating";

        public bool Execute(SimulationEvent simulationEvent)
        {
            Debug.Log($"[AegisFlow] Execute Activating Event, EntityId: {simulationEvent.EntityId}");
            return true;
        }
    }
}
