namespace AegisFlow.Domain
{
    /// <summary>
    /// 战斗输入快照。BattleField 只读取 BattleInput，不直接依赖 PlayerState / GameMgr。
    /// </summary>
    public sealed class BattleInput
    {
        public string BattleId { get; private set; }

        public BattleInput(string battleId)
        {
            BattleId = battleId;
        }
    }
}
