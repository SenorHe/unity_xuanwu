using AegisFlow.Event;

namespace AegisFlow.UI
{
    /// <summary>
    /// UI 生命周期绑定器示例。负责把通用 UIForm 接入事件总线。
    /// </summary>
    public sealed class UIFormLifecycleBinder
    {
        private readonly DomainEventBus m_EventBus;
        private readonly ToastPresenter m_ToastPresenter;
        private readonly ToastQueue m_ToastQueue;
        private readonly ToastPolicy m_ToastPolicy;

        public UIFormLifecycleBinder(DomainEventBus eventBus, ToastPresenter toastPresenter, ToastQueue toastQueue, ToastPolicy toastPolicy)
        {
            m_EventBus = eventBus;
            m_ToastPresenter = toastPresenter;
            m_ToastQueue = toastQueue;
            m_ToastPolicy = toastPolicy;
        }

        public void BindCommandToast(CommandToastForm form)
        {
            if (form == null)
            {
                return;
            }

            form.Initialize(m_EventBus, m_ToastPresenter, m_ToastQueue, m_ToastPolicy);
            form.Open();
        }

        public void UnbindCommandToast(CommandToastForm form)
        {
            if (form == null)
            {
                return;
            }

            form.Close();
        }
    }
}
