using System;
using System.Collections.Generic;

namespace AegisFlow.Command
{
    /// <summary>
    /// UI 命令分发器。UI 只投递命令，具体业务由 ApplicationService 处理。
    /// </summary>
    public sealed class UICommandDispatcher
    {
        private readonly Dictionary<Type, Delegate> m_HandlerDic = new Dictionary<Type, Delegate>();

        public void Register<TCommand>(Action<TCommand> handler) where TCommand : UICommand
        {
            if (handler == null)
            {
                return;
            }

            Type commandType = typeof(TCommand);

            if (m_HandlerDic.TryGetValue(commandType, out Delegate existed))
            {
                existed = Delegate.Remove(existed, handler);
                existed = Delegate.Combine(existed, handler);
                m_HandlerDic[commandType] = existed;
                return;
            }

            m_HandlerDic.Add(commandType, handler);
        }

        public void Unregister<TCommand>(Action<TCommand> handler) where TCommand : UICommand
        {
            if (handler == null)
            {
                return;
            }

            Type commandType = typeof(TCommand);

            if (!m_HandlerDic.TryGetValue(commandType, out Delegate existed))
            {
                return;
            }

            existed = Delegate.Remove(existed, handler);

            if (existed == null)
            {
                m_HandlerDic.Remove(commandType);
            }
            else
            {
                m_HandlerDic[commandType] = existed;
            }
        }

        public void Dispatch<TCommand>(TCommand command) where TCommand : UICommand
        {
            if (command == null)
            {
                return;
            }

            if (m_HandlerDic.TryGetValue(typeof(TCommand), out Delegate handler))
            {
                ((Action<TCommand>)handler)?.Invoke(command);
            }
        }
    }
}
