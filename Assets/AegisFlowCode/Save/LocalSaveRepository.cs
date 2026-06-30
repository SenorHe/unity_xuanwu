using System.IO;
using UnityEngine;

namespace AegisFlow.Save
{
    /// <summary>
    /// 本地保存仓储。采用 tmp + bak 思路，具体项目可替换为 DataSaver.SaveDataToJsonSafe。
    /// </summary>
    public sealed class LocalSaveRepository : ISaveRepository
    {
        private readonly string m_RootPath;

        public LocalSaveRepository()
            : this(Application.persistentDataPath)
        {
        }

        public LocalSaveRepository(string rootPath)
        {
            m_RootPath = rootPath;
        }

        public bool Save(string key, string json)
        {
            if (!TryBuildPath(key, out string path) || json == null)
            {
                return false;
            }

            Directory.CreateDirectory(m_RootPath);
            string tmpPath = path + ".tmp";
            string bakPath = path + ".bak";

            File.WriteAllText(tmpPath, json);

            if (File.Exists(path))
            {
                File.Copy(path, bakPath, true);
            }

            File.Copy(tmpPath, path, true);
            File.Delete(tmpPath);
            return true;
        }

        public bool TryLoad(string key, out string json)
        {
            json = null;

            if (!TryBuildPath(key, out string path))
            {
                return false;
            }

            if (!File.Exists(path))
            {
                return false;
            }

            json = File.ReadAllText(path);
            return true;
        }

        public bool TryLoadBackup(string key, out string json)
        {
            json = null;

            if (!TryBuildPath(key, out string path))
            {
                return false;
            }

            string backupPath = path + ".bak";

            if (!File.Exists(backupPath))
            {
                return false;
            }

            json = File.ReadAllText(backupPath);
            return true;
        }

        private bool TryBuildPath(string key, out string path)
        {
            path = null;

            if (string.IsNullOrWhiteSpace(key)
                || key.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
                || key.Contains("/")
                || key.Contains("\\"))
            {
                return false;
            }

            path = Path.Combine(m_RootPath, key + ".json");
            return true;
        }
    }
}
