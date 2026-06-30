namespace AegisFlow.UI
{
    /// <summary>
    /// Toast 视图数据。统一命令结果、路由失败等提示展示。
    /// </summary>
    public sealed class ToastViewData
    {
        public string SourceId { get; private set; }
        public string Message { get; private set; }
        public bool IsSuccess { get; private set; }
        public int Priority { get; private set; }

        public ToastViewData(string sourceId, string message, bool isSuccess, int priority)
        {
            SourceId = sourceId;
            Message = message;
            IsSuccess = isSuccess;
            Priority = priority;
        }
    }
}
