namespace AegisFlow.Event
{
    /// <summary>
    /// UI 命令执行结果事件。用于通知表现层显示成功、失败、提示，不承载业务状态。
    /// </summary>
    public sealed class UICommandExecutedEvent : DomainEvent
    {
        public string CommandId { get; private set; }
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }

        public UICommandExecutedEvent(float createdTime, string commandId, bool isSuccess, string message) : base(createdTime)
        {
            CommandId = commandId;
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}
