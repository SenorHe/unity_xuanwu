using AegisFlow.Data;
using UnityEngine;

namespace AegisFlow.Factory
{
    /// <summary>
    /// 实体工厂。只负责创建实体 GameObject，不承担恢复、分组、业务规则。
    /// </summary>
    public sealed class EntityFactory
    {
        private readonly EntityComponentBinder m_ComponentBinder;
        private readonly EntityGroupResolver m_GroupResolver;

        public EntityFactory(EntityComponentBinder componentBinder, EntityGroupResolver groupResolver)
        {
            m_ComponentBinder = componentBinder;
            m_GroupResolver = groupResolver;
        }

        public GameObject CreateEditEntity(EntityData entityData, GameObject prefab)
        {
            if (entityData == null || prefab == null)
            {
                return null;
            }

            GameObject entityObject = Object.Instantiate(prefab);
            entityObject.name = entityData.DisplayName;

            string groupName = m_GroupResolver.ResolveGroup(entityData.ConfigId);
            m_ComponentBinder.Bind(entityObject, entityData);

            // [扩展点] 根据 groupName 挂载到对应 RTE / UI 分组节点。
            return entityObject;
        }
    }
}
