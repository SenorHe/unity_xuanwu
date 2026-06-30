using AegisFlow.Data;

namespace AegisFlow.Domain
{
    /// <summary>
    /// 玩家领域服务。隔离 UI 与 PlayerDC / 旧 PlayerState。
    /// </summary>
    public sealed class PlayerDomainService
    {
        private readonly PlayerDC m_PlayerDC;

        public PlayerDomainService(PlayerDC playerDC)
        {
            m_PlayerDC = playerDC;
        }

        public bool HasPlayer()
        {
            return !string.IsNullOrEmpty(m_PlayerDC.PlayerId);
        }

        public bool CanEnterBattle()
        {
            return HasPlayer() && m_PlayerDC.Level > 0;
        }
    }
}
