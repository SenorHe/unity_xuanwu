using AegisFlow.Data;
using AegisFlow.Save;

namespace AegisFlow.Legacy
{
    /// <summary>
    /// 旧存档迁移服务。负责把旧系统数据转换为新架构保存格式。
    /// </summary>
    public sealed class LegacySaveMigrationService
    {
        private readonly LegacyEntityRepositoryAdapter m_LegacyEntityRepositoryAdapter;

        public LegacySaveMigrationService(LegacyEntityRepositoryAdapter legacyEntityRepositoryAdapter)
        {
            m_LegacyEntityRepositoryAdapter = legacyEntityRepositoryAdapter;
        }

        public bool MigrateEntity(string entityId, string configId, string displayName)
        {
            EntityData entityData = m_LegacyEntityRepositoryAdapter.ConvertLegacyEntity(entityId, configId, displayName);

            string json = $"{{\"entityId\":\"{SaveJsonUtility.Escape(entityData.EntityId)}\",\"configId\":\"{SaveJsonUtility.Escape(entityData.ConfigId)}\",\"displayName\":\"{SaveJsonUtility.Escape(entityData.DisplayName)}\"}}";
            return SaveAppService.Save($"entity_{entityData.EntityId}", json);
        }
    }
}
