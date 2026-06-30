using UnityEngine;

namespace AegisFlow.Runtime
{
    /// <summary>
    /// AGV 到达事件处理器。运行态 AGV 到达目标时触发。
    /// </summary>
    public sealed class ArrivedEntityEventHandler : ISimulationEventHandler
    {
        public string EventType => "Arrived";

        public bool Execute(SimulationEvent simulationEvent)
        {
            Debug.Log($"[AegisFlow] AGV Arrived, EntityId: {simulationEvent.EntityId}, Step: {simulationEvent.TimeStep}");
            return true;
        }
    }
}
