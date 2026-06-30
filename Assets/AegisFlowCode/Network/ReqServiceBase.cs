namespace AegisFlow.Network
{
    /// <summary>
    /// 请求服务基类。Procedure / UI 不直接拼协议。
    /// </summary>
    public abstract class ReqServiceBase
    {
        protected readonly NetworkClient m_NetworkClient;

        protected ReqServiceBase(NetworkClient networkClient)
        {
            m_NetworkClient = networkClient;
        }
    }
}
