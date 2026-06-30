namespace AegisFlow.Event
{
    /// <summary>
    /// 玩家信息更新领域事件。用于通知 UI / 其他领域刷新视图。
    /// </summary>
    public sealed class PlayerInfoUpdatedEvent : DomainEvent
    {
        public string PlayerId { get; private set; }

        public PlayerInfoUpdatedEvent(float createdTime, string playerId) : base(createdTime)
        {
            PlayerId = playerId;
        }
    }
}
