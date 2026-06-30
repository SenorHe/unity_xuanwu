namespace AegisFlow.Event
{
    /// <summary>
    /// Procedure 路由失败事件。用于 UI 提示，不承载流程状态。
    /// </summary>
    public sealed class ProcedureRouteFailedEvent : DomainEvent
    {
        public string RouteName { get; private set; }
        public string Message { get; private set; }

        public ProcedureRouteFailedEvent(float createdTime, string routeName, string message) : base(createdTime)
        {
            RouteName = routeName;
            Message = message;
        }
    }
}
