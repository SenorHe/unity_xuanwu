using UnityEngine;

namespace AegisFlow.UI
{
    /// <summary>
    /// UI 基类。UI 只展示 ViewData，只发 Command，不直接写业务数据。
    /// </summary>
    public abstract class UIFormBase : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            SubscribeEvents();
        }

        protected virtual void OnDisable()
        {
            UnsubscribeEvents();
        }

        public virtual void Open(object openData = null)
        {
        }

        public virtual void Close()
        {
        }

        protected virtual void SubscribeEvents()
        {
        }

        protected virtual void UnsubscribeEvents()
        {
        }
    }
}
