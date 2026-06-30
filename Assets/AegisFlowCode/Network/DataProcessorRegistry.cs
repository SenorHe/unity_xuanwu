using System.Collections.Generic;
using UnityEngine;

namespace AegisFlow.Network
{
    /// <summary>
    /// 协议处理器注册表。替代 MrCSave 巨型 switch。
    /// </summary>
    public sealed class DataProcessorRegistry
    {
        private readonly Dictionary<int, IDataProcessor> m_ProcessorDic = new Dictionary<int, IDataProcessor>();

        public void Register(IDataProcessor processor)
        {
            if (processor == null)
            {
                return;
            }

            m_ProcessorDic[processor.ProtoCode] = processor;
        }

        public void Process(int protoCode, byte[] payload)
        {
            if (!m_ProcessorDic.TryGetValue(protoCode, out IDataProcessor processor))
            {
                Debug.LogWarning($"[AegisFlow] 未找到协议处理器：{protoCode}");
                return;
            }

            processor.Process(payload);
        }

        public void Clear()
        {
            m_ProcessorDic.Clear();
        }
    }
}
