namespace AegisFlow.Domain
{
    /// <summary>
    /// 战斗准备领域服务。治理 Player / Table / BattleField 之间的输入边界。
    /// </summary>
    public sealed class BattlePrepareDomainService
    {
        private readonly PlayerDomainService m_PlayerDomainService;

        public BattlePrepareDomainService(PlayerDomainService playerDomainService)
        {
            m_PlayerDomainService = playerDomainService;
        }

        public bool TryBuildBattleInput(out BattleInput battleInput)
        {
            battleInput = null;

            if (!m_PlayerDomainService.CanEnterBattle())
            {
                return false;
            }

            battleInput = new BattleInput("DefaultBattle");
            return true;
        }
    }
}
