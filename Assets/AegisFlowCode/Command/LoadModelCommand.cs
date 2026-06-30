namespace AegisFlow.Command
{
    /// <summary>
    /// 加载模型命令。由编辑态 UI 发起，由 ApplicationService 处理。
    /// </summary>
    public sealed class LoadModelCommand : UICommand
    {
        public string ModelId { get; private set; }

        public LoadModelCommand(string modelId) : base("LoadModel")
        {
            ModelId = modelId;
        }
    }
}
