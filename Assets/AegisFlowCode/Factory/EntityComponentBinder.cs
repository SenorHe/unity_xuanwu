using AegisFlow.Data;
using UnityEngine;

namespace AegisFlow.Factory
{
    /// <summary>
    /// 实体组件绑定器。负责组件挂载和默认值注入。
    /// </summary>
    public sealed class EntityComponentBinder
    {
        public void Bind(GameObject entityObject, EntityData entityData)
        {
            if (entityObject == null || entityData == null)
            {
                return;
            }

            // [扩展点] 按 ConfigId 挂载业务组件，注入默认属性。
            // 示例：entityObject.AddComponent<SelectableEntity>();
        }
    }
}
