using AegisFlow.Event;
using UnityEngine;

namespace AegisFlow.UI
{
    /// <summary>
    /// 运行态状态 UI 示例。通过 RuntimeStepChangedEvent 刷新 ViewData。
    /// </summary>
    public sealed class RuntimeStatusForm : UIFormBase
    {
        private DomainEventBus m_EventBus;
        private RuntimePresenter m_RuntimePresenter;

        [SerializeField] private SUIRuntimeStatus varSUIRuntimeStatus;

        public void Initialize(DomainEventBus eventBus, RuntimePresenter runtimePresenter)
        {
            m_EventBus = eventBus;
            m_RuntimePresenter = runtimePresenter;
            RefreshView();
        }

        protected override void SubscribeEvents()
        {
            m_EventBus?.Subscribe<RuntimeStepChangedEvent>(OnRuntimeStepChanged);
        }

        protected override void UnsubscribeEvents()
        {
            m_EventBus?.Unsubscribe<RuntimeStepChangedEvent>(OnRuntimeStepChanged);
        }

        private void OnRuntimeStepChanged(RuntimeStepChangedEvent eventData)
        {
            RefreshView();
        }

        private void RefreshView()
        {
            if (m_RuntimePresenter == null)
            {
                return;
            }

            RuntimeViewData viewData = m_RuntimePresenter.BuildViewData();

            if (varSUIRuntimeStatus != null)
            {
                varSUIRuntimeStatus.Refresh(viewData);
            }

            Debug.Log($"[AegisFlow] Runtime UI Refresh: {viewData.RunningModelId}, Step: {viewData.CurrentStep}, Playing: {viewData.IsPlaying}");
        }
    }
}
