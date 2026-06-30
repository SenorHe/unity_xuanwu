using AegisFlow.Data;
using AegisFlow.Domain;

namespace AegisFlow.UI
{
    /// <summary>
    /// 玩家 Presenter。只读 PlayerDC，并将领域判断转换为 ViewData。
    /// </summary>
    public sealed class PlayerPresenter : PresenterBase<PlayerViewData>
    {
        private readonly PlayerDC m_PlayerDC;
        private readonly PlayerDomainService m_PlayerDomainService;

        public PlayerPresenter(PlayerDC playerDC, PlayerDomainService playerDomainService)
        {
            m_PlayerDC = playerDC;
            m_PlayerDomainService = playerDomainService;
        }

        public override PlayerViewData BuildViewData()
        {
            return new PlayerViewData(
                m_PlayerDC.DisplayName,
                m_PlayerDC.Level,
                m_PlayerDomainService.CanEnterBattle());
        }
    }
}
