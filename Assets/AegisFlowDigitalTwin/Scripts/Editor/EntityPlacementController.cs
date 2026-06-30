using UnityEngine;

namespace AegisFlowDigitalTwin.Editor
{
    /// <summary>
    /// 实体放置控制器。射线检测地面 + 网格对齐。
    /// </summary>
    public sealed class EntityPlacementController
    {
        private readonly Camera m_Camera;
        private readonly float m_GridSize;
        private readonly LayerMask m_GroundMask;

        public event System.Action<string, Vector3> OnPlaceRequested;

        public EntityPlacementController(Camera camera, float gridSize, LayerMask groundMask)
        {
            m_Camera = camera;
            m_GridSize = gridSize;
            m_GroundMask = groundMask;
        }

        public Vector3? GetPlacementPosition(Vector2 screenPos)
        {
            Ray ray = m_Camera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit, 200f, m_GroundMask))
            {
                return SnapToGrid(hit.point);
            }

            return null;
        }

        public void Place(string entityType, Vector3 position)
        {
            OnPlaceRequested?.Invoke(entityType, position);
        }

        private Vector3 SnapToGrid(Vector3 pos)
        {
            float x = Mathf.Round(pos.x / m_GridSize) * m_GridSize;
            float z = Mathf.Round(pos.z / m_GridSize) * m_GridSize;
            return new Vector3(x, 0f, z);
        }
    }
}
