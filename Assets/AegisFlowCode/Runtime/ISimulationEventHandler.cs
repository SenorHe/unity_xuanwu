namespace AegisFlow.Runtime
{
    /// <summary>
    /// 仿真事件处理器接口。Handler 返回 bool 表示执行是否成功。
    /// </summary>
    public interface ISimulationEventHandler
    {
        string EventType { get; }

        bool Execute(SimulationEvent simulationEvent);
    }
}
