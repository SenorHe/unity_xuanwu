namespace AegisFlow.Save
{
    public interface ISaveRepository
    {
        bool Save(string key, string json);

        bool TryLoad(string key, out string json);

        bool TryLoadBackup(string key, out string json);
    }
}
