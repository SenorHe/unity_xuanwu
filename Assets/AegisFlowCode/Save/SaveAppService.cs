namespace AegisFlow.Save
{
    /// <summary>
    /// 保存应用服务。统一保存入口，避免业务层直接写文件。
    /// </summary>
    public static class SaveAppService
    {
        private static ISaveRepository s_SaveRepository;

        public static void Initialize(ISaveRepository saveRepository)
        {
            s_SaveRepository = saveRepository;
        }

        public static bool Save(string key, string json)
        {
            if (s_SaveRepository == null)
            {
                return false;
            }

            return s_SaveRepository.Save(key, json);
        }

        public static bool TryLoad(string key, out string json)
        {
            json = null;

            if (s_SaveRepository == null)
            {
                return false;
            }

            return s_SaveRepository.TryLoad(key, out json);
        }

        public static bool TryLoadBackup(string key, out string json)
        {
            json = null;

            if (s_SaveRepository == null)
            {
                return false;
            }

            return s_SaveRepository.TryLoadBackup(key, out json);
        }

        public static void Shutdown()
        {
            s_SaveRepository = null;
        }
    }
}
