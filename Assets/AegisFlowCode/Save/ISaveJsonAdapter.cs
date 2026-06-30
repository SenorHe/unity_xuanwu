namespace AegisFlow.Save
{
    /// <summary>
    /// 存档 JSON 适配器。正式项目可用 NewtonsoftJson / System.Text.Json / Unity JsonUtility 实现。
    /// </summary>
    public interface ISaveJsonAdapter
    {
        string ToJson(SimulationModelSaveData saveData);

        bool TryFromJson(string json, out SimulationModelSaveData saveData);
    }
}
