namespace AegisFlow.Command
{
    /// <summary>
    /// UI 命令基类。UI 发命令，不直接写业务数据。
    /// </summary>
    public abstract class UICommand
    {
        public string CommandId { get; private set; }

        protected UICommand(string commandId)
        {
            CommandId = commandId;
        }
    }
}
