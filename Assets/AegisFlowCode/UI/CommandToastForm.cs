using AegisFlow.Event;
using UnityEngine;
using UnityEngine.UI;

namespace AegisFlow.UI
{
    /// <summary>
    /// 命令结果提示 UI。只消费 UICommandExecutedEvent，不参与业务判断。
    /// </summary>
    public sealed class CommandToastForm : UIFormBase
    {
        [SerializeField] private Text varTxtMessage;

        private DomainEventBus m_EventBus;
        private ToastPresenter m_ToastPresenter;
        private ToastQueue m_ToastQueue;
        private ToastPolicy m_ToastPolicy;
        private ToastAnimationStateMachine m_AnimationStateMachine = new ToastAnimationStateMachine();
        private float m_HideTime;

        public void Initialize(DomainEventBus eventBus, ToastPresenter toastPresenter, ToastQueue toastQueue, ToastPolicy toastPolicy)
        {
            m_EventBus = eventBus;
            m_ToastPresenter = toastPresenter;
            m_ToastQueue = toastQueue;
            m_ToastPolicy = toastPolicy;
        }

        protected override void SubscribeEvents()
        {
            m_EventBus?.Subscribe<UICommandExecutedEvent>(OnCommandExecuted);
            m_EventBus?.Subscribe<ProcedureRouteFailedEvent>(OnProcedureRouteFailed);
        }

        protected override void UnsubscribeEvents()
        {
            m_EventBus?.Unsubscribe<UICommandExecutedEvent>(OnCommandExecuted);
            m_EventBus?.Unsubscribe<ProcedureRouteFailedEvent>(OnProcedureRouteFailed);
        }

        private void OnCommandExecuted(UICommandExecutedEvent eventData)
        {
            if (eventData == null || m_ToastPresenter == null)
            {
                return;
            }

            ToastViewData viewData = m_ToastPresenter.Build(eventData);
            EnqueueToast(viewData);

            if (viewData != null)
            {
                Debug.Log($"[AegisFlow] Command Result: {viewData.SourceId}, Success: {viewData.IsSuccess}, Message: {viewData.Message}");
            }
        }

        private void OnProcedureRouteFailed(ProcedureRouteFailedEvent eventData)
        {
            if (eventData == null || m_ToastPresenter == null)
            {
                return;
            }

            ToastViewData viewData = m_ToastPresenter.Build(eventData);
            EnqueueToast(viewData);

            if (viewData != null)
            {
                Debug.Log($"[AegisFlow] Route Failed: {viewData.SourceId}, Message: {viewData.Message}");
            }
        }

        private void Update()
        {
            if (varTxtMessage == null || m_AnimationStateMachine.State != ToastAnimationState.Visible)
            {
                return;
            }

            if (Time.time < m_HideTime)
            {
                return;
            }

            m_AnimationStateMachine.BeginHide();
            HideCurrent();
            m_AnimationStateMachine.MarkHidden();
            TryShowNext();
        }

        private void EnqueueToast(ToastViewData viewData)
        {
            if (viewData == null || m_ToastQueue == null)
            {
                return;
            }

            m_ToastQueue.Enqueue(viewData);
            TryShowNext();
        }

        private void TryShowNext()
        {
            if (varTxtMessage == null || m_AnimationStateMachine.IsVisible || m_ToastQueue == null)
            {
                return;
            }

            if (!m_ToastQueue.TryDequeue(out ToastViewData viewData))
            {
                return;
            }

            RefreshView(viewData);
        }

        private void RefreshView(ToastViewData viewData)
        {
            if (viewData == null || varTxtMessage == null || m_ToastPolicy == null)
            {
                return;
            }

            varTxtMessage.text = viewData.Message;
            m_HideTime = Time.time + m_ToastPolicy.VisibleSeconds;
        }

        private void HideCurrent()
        {
            if (varTxtMessage != null)
            {
                varTxtMessage.text = string.Empty;
            }
        }
    }
}
