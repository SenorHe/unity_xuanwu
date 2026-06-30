using System;
using System.Collections.Generic;

namespace AegisFlow.Data
{
    /// <summary>
    /// DC 基类。DC 是单一事实来源，只负责保存状态和通知变化。
    /// </summary>
    public abstract class DataCenterBase
    {
        private readonly List<Action<DataCenterBase>> m_ChangedHandlers = new List<Action<DataCenterBase>>();

        public void AddChangedListener(Action<DataCenterBase> handler)
        {
            if (handler == null || m_ChangedHandlers.Contains(handler))
            {
                return;
            }

            m_ChangedHandlers.Add(handler);
        }

        public void RemoveChangedListener(Action<DataCenterBase> handler)
        {
            if (handler == null)
            {
                return;
            }

            m_ChangedHandlers.Remove(handler);
        }

        protected void Save()
        {
            for (int i = 0; i < m_ChangedHandlers.Count; i++)
            {
                m_ChangedHandlers[i]?.Invoke(this);
            }
        }
    }
}
