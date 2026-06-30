using AegisFlow.Data;

namespace AegisFlow.Runtime
{
    /// <summary>
    /// AGV 拾取事件处理器。
    /// </summary>
    public sealed class PickupEntityEventHandler : ISimulationEventHandler
    {
        public string EventType => "Pickup";

        public bool Execute(SimulationEvent simulationEvent)
        {
            return true;
        }
    }

    /// <summary>
    /// AGV 放下事件处理器。
    /// </summary>
    public sealed class DropEntityEventHandler : ISimulationEventHandler
    {
        public string EventType => "Drop";

        public bool Execute(SimulationEvent simulationEvent)
        {
            return true;
        }
    }

    /// <summary>
    /// AGV 充电事件处理器。
    /// </summary>
    public sealed class ChargeEntityEventHandler : ISimulationEventHandler
    {
        public string EventType => "Charge";

        public bool Execute(SimulationEvent simulationEvent)
        {
            return true;
        }
    }

    /// <summary>
    /// 传感器扫描事件处理器。
    /// </summary>
    public sealed class SensorScanEventHandler : ISimulationEventHandler
    {
        public string EventType => "Scanning";

        public bool Execute(SimulationEvent simulationEvent)
        {
            return true;
        }
    }
}
