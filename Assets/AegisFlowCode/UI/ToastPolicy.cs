namespace AegisFlow.UI
{
    /// <summary>
    /// Toast 展示策略。集中控制去重、展示时长等表现规则。
    /// </summary>
    public sealed class ToastPolicy
    {
        public float VisibleSeconds { get; private set; }
        public bool MergeSameMessage { get; private set; }

        public ToastPolicy(float visibleSeconds, bool mergeSameMessage)
        {
            VisibleSeconds = visibleSeconds;
            MergeSameMessage = mergeSameMessage;
        }
    }
}
