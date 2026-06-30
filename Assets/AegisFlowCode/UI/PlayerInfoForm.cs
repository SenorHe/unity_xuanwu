using AegisFlow.Event;
using UnityEngine;

namespace AegisFlow.UI
{
    /// <summary>
    /// 玩家信息 UI 示例。只订阅事件、刷新 ViewData，不直接写 PlayerDC。
    /// </summary>
    public sealed class PlayerInfoForm : UIFormBase
    {
        private DomainEventBus m_EventBus;
        private PlayerPresenter m_PlayerPresenter;

        public void Initialize(DomainEventBus eventBus, PlayerPresenter playerPresenter)
        {
            m_EventBus = eventBus;
            m_PlayerPresenter = playerPresenter;
            RefreshView();
        }

        protected override void SubscribeEvents()
        {
            m_EventBus?.Subscribe<PlayerInfoUpdatedEvent>(OnPlayerInfoUpdated);
        }

        protected override void UnsubscribeEvents()
        {
            m_EventBus?.Unsubscribe<PlayerInfoUpdatedEvent>(OnPlayerInfoUpdated);
        }

        private void OnPlayerInfoUpdated(PlayerInfoUpdatedEvent eventData)
        {
            RefreshView();
        }

        private void RefreshView()
        {
            if (m_PlayerPresenter == null)
            {
                return;
            }

            PlayerViewData viewData = m_PlayerPresenter.BuildViewData();
            Debug.Log($"[AegisFlow] Player UI Refresh: {viewData.DisplayName}, Lv.{viewData.Level}, CanEnterBattle: {viewData.CanEnterBattle}");
        }
    }
}
