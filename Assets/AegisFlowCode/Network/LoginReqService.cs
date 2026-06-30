using System.Text;

namespace AegisFlow.Network
{
    /// <summary>
    /// 登录请求服务。负责协议组装和发送。
    /// </summary>
    public sealed class LoginReqService : ReqServiceBase
    {
        private const int PROTO_LOGIN_REQ = 10001;

        public LoginReqService(NetworkClient networkClient) : base(networkClient)
        {
        }

        public void SendLogin(string account, string password)
        {
            // [扩展点] 正式项目中替换为 Protobuf 序列化。
            byte[] payload = Encoding.UTF8.GetBytes($"{account}:{password}");
            m_NetworkClient.Send(PROTO_LOGIN_REQ, payload);
        }
    }
}
