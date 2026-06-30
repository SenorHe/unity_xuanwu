using System.Text;
using AegisFlow.Data;
using AegisFlow.Event;
using UnityEngine;

namespace AegisFlow.Network
{
    /// <summary>
    /// 登录回包处理器。网络回包只通过 Processor 写入 AccountDC。
    /// </summary>
    public sealed class LoginDataProcessor : IDataProcessor
    {
        public const int PROTO_LOGIN_ACK = 10002;

        private readonly AccountDC m_AccountDC;
        private readonly DomainEventBus m_EventBus;

        public int ProtoCode => PROTO_LOGIN_ACK;

        public LoginDataProcessor(AccountDC accountDC, DomainEventBus eventBus)
        {
            m_AccountDC = accountDC;
            m_EventBus = eventBus;
        }

        public void Process(byte[] payload)
        {
            string text = Encoding.UTF8.GetString(payload ?? new byte[0]);
            string[] parts = text.Split(':');

            if (parts.Length < 2)
            {
                return;
            }

            m_AccountDC.AttachLoginResult(parts[0], parts[1]);
            m_EventBus.Fire(new LoginSucceededEvent(Time.time, parts[0]));
        }
    }
}
