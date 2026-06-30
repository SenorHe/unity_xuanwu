namespace AegisFlow.Data
{
    /// <summary>
    /// 玩家数据中心。保存玩家运行期状态。
    /// </summary>
    public sealed class PlayerDC : DataCenterBase
    {
        public string PlayerId { get; private set; }
        public string DisplayName { get; private set; }
        public int Level { get; private set; }

        public void AttachPlayerInfo(string playerId, string displayName, int level)
        {
            PlayerId = playerId;
            DisplayName = displayName;
            Level = level;
            Save();
        }
    }
}
