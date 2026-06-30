using System.Collections.Generic;
using UnityEngine;

namespace AegisFlowDigitalTwin.Visual
{
    /// <summary>
    /// EntityId 到 EntityVisualBase 的映射表。管理实体视觉生命周期。
    /// </summary>
    public sealed class EntityVisualRegistry
    {
        private readonly Dictionary<string, EntityVisualBase> m_Visuals = new Dictionary<string, EntityVisualBase>();
        private readonly Transform m_RootParent;

        public IReadOnlyCollection<EntityVisualBase> Visuals => m_Visuals.Values;

        public EntityVisualRegistry(Transform rootParent)
        {
            m_RootParent = rootParent;
        }

        public void Register(string entityId, EntityVisualBase visual)
        {
            if (string.IsNullOrEmpty(entityId) || visual == null)
            {
                return;
            }

            m_Visuals[entityId] = visual;
        }

        public bool Unregister(string entityId)
        {
            if (!m_Visuals.TryGetValue(entityId, out EntityVisualBase visual))
            {
                return false;
            }

            m_Visuals.Remove(entityId);
            if (visual != null)
            {
                Object.Destroy(visual.gameObject);
            }

            return true;
        }

        public bool TryGetVisual(string entityId, out EntityVisualBase visual)
        {
            return m_Visuals.TryGetValue(entityId, out visual);
        }

        public void Clear()
        {
            foreach (var pair in m_Visuals)
            {
                if (pair.Value != null)
                {
                    Object.Destroy(pair.Value.gameObject);
                }
            }

            m_Visuals.Clear();
        }
    }
}
