using System.Collections.Generic;

namespace AegisFlow.UI
{
    /// <summary>
    /// Toast 队列。负责排队和去重，不直接操作 Unity UI。
    /// </summary>
    public sealed class ToastQueue
    {
        private readonly List<ToastViewData> m_Queue = new List<ToastViewData>();
        private readonly ToastPolicy m_Policy;
        private ToastViewData m_LastToast;

        public ToastQueue(ToastPolicy policy)
        {
            m_Policy = policy;
        }

        public void Enqueue(ToastViewData viewData)
        {
            if (viewData == null)
            {
                return;
            }

            if (m_Policy != null && m_Policy.MergeSameMessage && m_LastToast != null && m_LastToast.Message == viewData.Message)
            {
                return;
            }

            InsertByPriority(viewData);
            m_LastToast = viewData;
        }

        public bool TryDequeue(out ToastViewData viewData)
        {
            if (m_Queue.Count <= 0)
            {
                viewData = null;
                return false;
            }

            viewData = m_Queue[0];
            m_Queue.RemoveAt(0);
            return true;
        }

        private void InsertByPriority(ToastViewData viewData)
        {
            for (int i = 0; i < m_Queue.Count; i++)
            {
                if (viewData.Priority > m_Queue[i].Priority)
                {
                    m_Queue.Insert(i, viewData);
                    return;
                }
            }

            m_Queue.Add(viewData);
        }

        public void Clear()
        {
            m_Queue.Clear();
            m_LastToast = null;
        }
    }
}
