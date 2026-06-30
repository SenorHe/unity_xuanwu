using UnityEngine;

namespace AegisFlow.Runtime
{
    /// <summary>
    /// 完成实体事件 Handler 示例。
    /// </summary>
    public sealed class CompleteEntityEventHandler : ISimulationEventHandler
    {
        public string EventType => "Completed";

        public bool Execute(SimulationEvent simulationEvent)
        {
            Debug.Log($"[AegisFlow] Execute Completed Event, EntityId: {simulationEvent.EntityId}");
            return true;
        }
    }
}
