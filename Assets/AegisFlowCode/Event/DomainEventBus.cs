using System;
using System.Collections.Generic;

namespace AegisFlow.Event
{
    /// <summary>
    /// 领域事件总线。用于跨领域通知，不承载业务状态。
    /// </summary>
    public sealed class DomainEventBus
    {
        private readonly Dictionary<Type, Delegate> m_HandlerDic = new Dictionary<Type, Delegate>();

        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : DomainEvent
        {
            if (handler == null)
            {
                return;
            }

            Type eventType = typeof(TEvent);

            if (m_HandlerDic.TryGetValue(eventType, out Delegate existed))
            {
                existed = Delegate.Remove(existed, handler);
                existed = Delegate.Combine(existed, handler);
                m_HandlerDic[eventType] = existed;
                return;
            }

            m_HandlerDic.Add(eventType, handler);
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : DomainEvent
        {
            if (handler == null)
            {
                return;
            }

            Type eventType = typeof(TEvent);

            if (!m_HandlerDic.TryGetValue(eventType, out Delegate existed))
            {
                return;
            }

            existed = Delegate.Remove(existed, handler);

            if (existed == null)
            {
                m_HandlerDic.Remove(eventType);
            }
            else
            {
                m_HandlerDic[eventType] = existed;
            }
        }

        public void Fire<TEvent>(TEvent eventData) where TEvent : DomainEvent
        {
            if (eventData == null)
            {
                return;
            }

            if (m_HandlerDic.TryGetValue(typeof(TEvent), out Delegate handler))
            {
                ((Action<TEvent>)handler)?.Invoke(eventData);
            }
        }

        public void Clear()
        {
            m_HandlerDic.Clear();
        }
    }
}
