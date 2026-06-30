namespace AegisFlow.UI
{
    /// <summary>
    /// 玩家视图数据。UI 只消费 ViewData，不直接读取 PlayerDC。
    /// </summary>
    public sealed class PlayerViewData
    {
        public string DisplayName { get; private set; }
        public int Level { get; private set; }
        public bool CanEnterBattle { get; private set; }

        public PlayerViewData(string displayName, int level, bool canEnterBattle)
        {
            DisplayName = displayName;
            Level = level;
            CanEnterBattle = canEnterBattle;
        }
    }
}
