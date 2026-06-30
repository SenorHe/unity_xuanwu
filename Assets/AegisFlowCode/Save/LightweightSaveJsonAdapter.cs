namespace AegisFlow.Save
{
    /// <summary>
    /// 轻量存档 JSON 适配器。当前复用 SimulationModelSaveData 内部实现。
    /// </summary>
    public sealed class LightweightSaveJsonAdapter : ISaveJsonAdapter
    {
        public string ToJson(SimulationModelSaveData saveData)
        {
            return saveData == null ? null : saveData.ToJson();
        }

        public bool TryFromJson(string json, out SimulationModelSaveData saveData)
        {
            return SimulationModelSaveData.TryParse(json, out saveData);
        }
    }
}
