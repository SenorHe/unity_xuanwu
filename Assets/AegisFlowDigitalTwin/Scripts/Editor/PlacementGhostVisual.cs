using UnityEngine;

namespace AegisFlowDigitalTwin.Editor
{
    /// <summary>
    /// 放置预览幽灵体。半透明显示将要放置的实体。
    /// </summary>
    public sealed class PlacementGhostVisual : MonoBehaviour
    {
        private GameObject m_GhostObj;
        private Material m_GhostMat;

        private void Awake()
        {
            m_GhostMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            m_GhostMat.color = new Color(0.5f, 0.8f, 1f, 0.4f);
        }

        public void Show(string entityType)
        {
            Hide();

            m_GhostObj = new GameObject($"Ghost_{entityType}");
            m_GhostObj.transform.SetParent(transform);

            CreateGhostMesh(entityType);
        }

        public void Hide()
        {
            if (m_GhostObj != null)
            {
                Destroy(m_GhostObj);
                m_GhostObj = null;
            }
        }

        public void UpdatePosition(Vector3 pos)
        {
            if (m_GhostObj != null)
            {
                m_GhostObj.transform.position = pos;
            }
        }

        private void CreateGhostMesh(string entityType)
        {
            GameObject prim = GameObject.CreatePrimitive(PrimitiveType.Cube);
            prim.transform.SetParent(m_GhostObj.transform);
            prim.transform.localPosition = Vector3.zero;

            Vector3 scale;
            switch (entityType)
            {
                case "AGV":
                    scale = new Vector3(0.8f, 0.4f, 1.2f);
                    break;
                case "RACK":
                    scale = new Vector3(1.2f, 1.8f, 0.8f);
                    break;
                case "CHARGER":
                    scale = new Vector3(0.3f, 2f, 0.3f);
                    break;
                case "CONVEYOR":
                    scale = new Vector3(0.8f, 0.4f, 1.5f);
                    break;
                case "WORKSTATION":
                    scale = new Vector3(1.2f, 0.8f, 1f);
                    break;
                case "SENSOR":
                    scale = new Vector3(0.1f, 1f, 0.1f);
                    break;
                default:
                    scale = Vector3.one;
                    break;
            }

            prim.transform.localScale = scale;
            prim.transform.position += new Vector3(0f, scale.y / 2f, 0f);

            MeshRenderer renderer = prim.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = m_GhostMat;
            }

            Collider col = prim.GetComponent<Collider>();
            if (col != null)
            {
                Destroy(col);
            }
        }
    }
}
