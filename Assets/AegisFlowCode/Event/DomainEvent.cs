namespace AegisFlow.Event
{
    /// <summary>
    /// 领域事件基类。跨领域核心事件必须强类型化。
    /// </summary>
    public abstract class DomainEvent
    {
        public float CreatedTime { get; private set; }

        protected DomainEvent(float createdTime)
        {
            CreatedTime = createdTime;
        }
    }
}
