using UnityEngine;
using AegisFlowDigitalTwin.Visual;

namespace AegisFlowDigitalTwin.Editor
{
    /// <summary>
    /// 实体选择控制器。点击射线检测选中实体。
    /// </summary>
    public sealed class EntitySelectionController
    {
        private readonly Camera m_Camera;
        private readonly LayerMask m_EntityMask;

        public EntitySelectionController(Camera camera, LayerMask entityMask)
        {
            m_Camera = camera;
            m_EntityMask = entityMask;
        }

        public string PickEntity(Vector2 screenPos)
        {
            Ray ray = m_Camera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit, 200f, m_EntityMask))
            {
                EntityVisualBase visual = hit.collider.GetComponentInParent<EntityVisualBase>();
                if (visual != null)
                {
                    return visual.EntityId;
                }
            }

            return null;
        }
    }
}
