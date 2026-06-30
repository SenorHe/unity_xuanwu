namespace AegisFlow.Save
{
    /// <summary>
    /// 存档 JSON 工具。当前提供轻量转义，后续可替换为正式 JSON 序列化库。
    /// </summary>
    public static class SaveJsonUtility
    {
        public static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r");
        }
    }
}
