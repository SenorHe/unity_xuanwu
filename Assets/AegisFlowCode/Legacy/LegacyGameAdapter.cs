namespace AegisFlow.Legacy
{
    /// <summary>
    /// 旧 GameMgr 防腐适配器。旧系统调用集中封装在这里，新代码不直接依赖 GameMgr。
    /// </summary>
    public sealed class LegacyGameAdapter
    {
        public bool IsAvailable()
        {
            // [扩展点] 在真实项目中检测 GameMgr.inst 是否可用。
            return true;
        }
    }
}
