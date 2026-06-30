using System;

namespace AegisFlow.Runtime
{
    /// <summary>
    /// 运行态事件策略配置加载器示例。正式项目可从配置表 / 表格系统加载。
    /// </summary>
    public sealed class RuntimeEventStrategyConfigLoader
    {
        public RuntimeEventStrategyConfig LoadDefault()
        {
            const string text = "AGV=Creating,Activating,Completed\nRACK=Creating,Completed\nSTATION=Creating,Activating,Completed";
            return LoadFromText(text);
        }

        public RuntimeEventStrategyConfig LoadFromText(string text)
        {
            RuntimeEventStrategyConfig config = new RuntimeEventStrategyConfig();

            if (string.IsNullOrEmpty(text))
            {
                return config;
            }

            string[] lines = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split('=');

                if (parts.Length != 2)
                {
                    continue;
                }

                string configId = parts[0].Trim();
                string[] eventTypes = parts[1].Split(',');

                for (int j = 0; j < eventTypes.Length; j++)
                {
                    eventTypes[j] = eventTypes[j].Trim();
                }

                config.Register(configId, eventTypes);
            }

            return config;
        }
    }
}
