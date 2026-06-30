namespace AegisFlow.Legacy
{
    /// <summary>
    /// 旧 PlayerState 防腐适配器。新代码不直接依赖 PlayerState 单例。
    /// </summary>
    public sealed class LegacyPlayerStateAdapter
    {
        public string GetCurrentPlayerId()
        {
            // [扩展点] 在真实项目中读取 PlayerState / GameMgr 中的玩家 ID。
            return string.Empty;
        }

        public int GetCurrentPlayerLevel()
        {
            // [扩展点] 在真实项目中读取旧 PlayerState 等级字段。
            return 0;
        }
    }
}
