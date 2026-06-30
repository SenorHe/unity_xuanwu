using AegisFlow.Data;

namespace AegisFlow.Legacy
{
    /// <summary>
    /// 旧实体仓储防腐适配器。负责把旧实体数据转换为 AegisFlow EntityData。
    /// </summary>
    public sealed class LegacyEntityRepositoryAdapter
    {
        public EntityData ConvertLegacyEntity(string entityId, string configId, string displayName)
        {
            // [扩展点] 在真实项目中从旧 EntityRepository / Dao 转换字段。
            return new EntityData(entityId, configId, displayName);
        }
    }
}
