namespace AegisFlow.Event
{
    /// <summary>
    /// 登录成功领域事件。用于通知跨模块刷新，不保存登录状态。
    /// </summary>
    public sealed class LoginSucceededEvent : DomainEvent
    {
        public string AccountId { get; private set; }

        public LoginSucceededEvent(float createdTime, string accountId) : base(createdTime)
        {
            AccountId = accountId;
        }
    }
}
