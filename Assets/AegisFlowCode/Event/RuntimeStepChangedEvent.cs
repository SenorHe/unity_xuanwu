namespace AegisFlow.Event
{
    /// <summary>
    /// 运行态步进变化事件。用于通知表现层刷新播放进度。
    /// </summary>
    public sealed class RuntimeStepChangedEvent : DomainEvent
    {
        public string ModelId { get; private set; }
        public int CurrentStep { get; private set; }

        public RuntimeStepChangedEvent(float createdTime, string modelId, int currentStep) : base(createdTime)
        {
            ModelId = modelId;
            CurrentStep = currentStep;
        }
    }
}
