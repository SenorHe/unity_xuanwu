namespace AegisFlow.Legacy
{
    /// <summary>
    /// 旧 MrCSave 防腐适配器。新代码不直接调用旧协议巨型分发入口。
    /// </summary>
    public sealed class LegacyMrCSaveAdapter
    {
        public void SendLegacyRequest(int protoCode, byte[] payload)
        {
            // [扩展点] 在真实项目中转发到 MrCSave / 旧网络发送入口。
        }
    }
}
