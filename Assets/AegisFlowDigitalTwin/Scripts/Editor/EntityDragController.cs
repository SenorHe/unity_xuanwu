using UnityEngine;

namespace AegisFlowDigitalTwin.Editor
{
    /// <summary>
    /// 实体拖拽控制器。在 XZ 平面拖拽移动实体。
    /// </summary>
    public sealed class EntityDragController
    {
        private readonly Camera m_Camera;
        private readonly float m_GridSize;
        private readonly LayerMask m_GroundMask;

        public event System.Action<string, Vector3> OnMoveRequested;

        public EntityDragController(Camera camera, float gridSize, LayerMask groundMask)
        {
            m_Camera = camera;
            m_GridSize = gridSize;
            m_GroundMask = groundMask;
        }

        public Vector3? GetDragPosition(Vector2 screenPos)
        {
            Ray ray = m_Camera.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit, 200f, m_GroundMask))
            {
                return SnapToGrid(hit.point);
            }

            return null;
        }

        public void Move(string entityId, Vector3 position)
        {
            OnMoveRequested?.Invoke(entityId, position);
        }

        private Vector3 SnapToGrid(Vector3 pos)
        {
            float x = Mathf.Round(pos.x / m_GridSize) * m_GridSize;
            float z = Mathf.Round(pos.z / m_GridSize) * m_GridSize;
            return new Vector3(x, 0f, z);
        }
    }
}
