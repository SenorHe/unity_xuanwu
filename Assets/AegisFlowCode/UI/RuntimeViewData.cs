namespace AegisFlow.UI
{
    /// <summary>
    /// 运行态视图数据。用于显示当前播放模型和 step。
    /// </summary>
    public sealed class RuntimeViewData
    {
        public string RunningModelId { get; private set; }
        public int CurrentStep { get; private set; }
        public bool IsPlaying { get; private set; }

        public RuntimeViewData(string runningModelId, int currentStep, bool isPlaying)
        {
            RunningModelId = runningModelId;
            CurrentStep = currentStep;
            IsPlaying = isPlaying;
        }
    }
}
