namespace AegisFlow.Command
{
    /// <summary>
    /// 保存模型命令。由编辑态 UI 发起，由 ApplicationService 处理。
    /// </summary>
    public sealed class SaveModelCommand : UICommand
    {
        public SaveModelCommand() : base("SaveModel")
        {
        }
    }
}
