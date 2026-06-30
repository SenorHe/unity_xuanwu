namespace AegisFlow.Data
{
    /// <summary>
    /// 运行态数据中心。保存播放状态，不污染编辑态 SimulationDC。
    /// </summary>
    public sealed class RuntimeDC : DataCenterBase
    {
        public string RunningModelId { get; private set; }
        public int CurrentStep { get; private set; }
        public bool IsPlaying { get; private set; }

        public void StartRuntime(string modelId)
        {
            RunningModelId = modelId;
            CurrentStep = 0;
            IsPlaying = true;
            Save();
        }

        public void UpdateStep(int step)
        {
            CurrentStep = step;
            Save();
        }

        public void Pause()
        {
            IsPlaying = false;
            Save();
        }

        public void Resume()
        {
            if (string.IsNullOrEmpty(RunningModelId))
            {
                return;
            }

            IsPlaying = true;
            Save();
        }

        public void Stop()
        {
            RunningModelId = null;
            CurrentStep = 0;
            IsPlaying = false;
            Save();
        }
    }
}
