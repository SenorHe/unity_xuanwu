using System;
using System.Text;
using AegisFlow.Data;
using AegisFlow.Event;
using UnityEngine;

namespace AegisFlow.Network
{
    /// <summary>
    /// 玩家信息回包处理器。负责把协议数据转换为 PlayerDC 状态。
    /// </summary>
    public sealed class PlayerDataProcessor : IDataProcessor
    {
        public const int PROTO_PLAYER_INFO_ACK = 10012;

        private readonly PlayerDC m_PlayerDC;
        private readonly DomainEventBus m_EventBus;

        public int ProtoCode => PROTO_PLAYER_INFO_ACK;

        public PlayerDataProcessor(PlayerDC playerDC, DomainEventBus eventBus)
        {
            m_PlayerDC = playerDC;
            m_EventBus = eventBus;
        }

        public void Process(byte[] payload)
        {
            string text = Encoding.UTF8.GetString(payload ?? new byte[0]);
            string[] parts = text.Split(':');

            if (parts.Length < 3 || !int.TryParse(parts[2], out int level))
            {
                return;
            }

            m_PlayerDC.AttachPlayerInfo(parts[0], parts[1], Math.Max(0, level));
            m_EventBus.Fire(new PlayerInfoUpdatedEvent(Time.time, parts[0]));
        }
    }
}
