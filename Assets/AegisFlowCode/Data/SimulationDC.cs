namespace AegisFlow.Data
{
    /// <summary>
    /// 仿真编辑态数据中心。保存当前编辑模型状态。
    /// </summary>
    public sealed class SimulationDC : DataCenterBase
    {
        public string CurrentModelId { get; private set; }
        public bool IsDirty { get; private set; }

        public void SwitchModel(string modelId)
        {
            CurrentModelId = modelId;
            IsDirty = false;
            Save();
        }

        public void MarkDirty()
        {
            IsDirty = true;
            Save();
        }

        public void MarkSaved()
        {
            IsDirty = false;
            Save();
        }
    }
}
