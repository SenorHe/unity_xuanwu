namespace AegisFlow.Command
{
    /// <summary>
    /// 启动仿真命令。
    /// </summary>
    public sealed class StartSimulationCommand : UICommand
    {
        public string ModelId { get; private set; }

        public StartSimulationCommand(string modelId) : base("StartSimulation")
        {
            ModelId = modelId;
        }
    }
}
