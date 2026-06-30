namespace AegisFlow.Runtime
{
    /// <summary>
    /// 仿真事件数据。运行态事件必须可识别类型和 time_step。
    /// </summary>
    public sealed class SimulationEvent
    {
        public string EventType { get; private set; }
        public int TimeStep { get; private set; }
        public string EntityId { get; private set; }

        public SimulationEvent(string eventType, int timeStep, string entityId)
        {
            EventType = eventType;
            TimeStep = timeStep;
            EntityId = entityId;
        }
    }
}
