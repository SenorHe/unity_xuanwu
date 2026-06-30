using UnityEngine;

namespace AegisFlow.Network
{
    /// <summary>
    /// 网络客户端骨架。负责发送协议和转发回包，不直接写 DC。
    /// </summary>
    public sealed class NetworkClient
    {
        private readonly DataProcessorRegistry m_DataProcessorRegistry;

        public NetworkClient(DataProcessorRegistry dataProcessorRegistry)
        {
            m_DataProcessorRegistry = dataProcessorRegistry;
        }

        public void Send(int protoCode, byte[] payload)
        {
            // [扩展点] 接入 Socket / HTTP / Protobuf 发送。
            Debug.Log($"[AegisFlow] Send proto: {protoCode}, bytes: {payload?.Length ?? 0}");
        }

        public void OnReceive(int protoCode, byte[] payload)
        {
            m_DataProcessorRegistry.Process(protoCode, payload);
        }
    }
}
