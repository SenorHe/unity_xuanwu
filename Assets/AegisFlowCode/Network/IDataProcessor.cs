namespace AegisFlow.Network
{
    /// <summary>
    /// 协议回包处理器接口。网络回包只能通过 Processor 写入 DC。
    /// </summary>
    public interface IDataProcessor
    {
        int ProtoCode { get; }

        void Process(byte[] payload);
    }
}
