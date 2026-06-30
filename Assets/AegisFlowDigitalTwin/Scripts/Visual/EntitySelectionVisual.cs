using UnityEngine;

namespace AegisFlowDigitalTwin.Visual
{
    /// <summary>
    /// 选中高亮效果。在选中实体周围显示发光轮廓。
    /// </summary>
    public sealed class EntitySelectionVisual : MonoBehaviour
    {
        [SerializeField] private float m_OutlineScale = 1.15f;
        [SerializeField] private Color m_OutlineColor = new Color(1f, 0.85f, 0.2f, 0.4f);

        private GameObject m_OutlineObj;
        private bool m_IsActive;

        public void Show()
        {
            if (m_OutlineObj == null)
            {
                CreateOutline();
            }

            m_OutlineObj.SetActive(true);
            m_IsActive = true;
        }

        public void Hide()
        {
            if (m_OutlineObj != null)
            {
                m_OutlineObj.SetActive(false);
            }

            m_IsActive = false;
        }

        private void CreateOutline()
        {
            m_OutlineObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            m_OutlineObj.name = "SelectionOutline";
            m_OutlineObj.transform.SetParent(transform);
            m_OutlineObj.transform.localPosition = Vector3.zero;
            m_OutlineObj.transform.localScale = Vector3.one * m_OutlineScale;

            MeshRenderer renderer = m_OutlineObj.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                mat.color = m_OutlineColor;
                renderer.material = mat;
            }

            Collider col = m_OutlineObj.GetComponent<Collider>();
            if (col != null)
            {
                Destroy(col);
            }
        }

        private void Update()
        {
            if (!m_IsActive || m_OutlineObj == null)
            {
                return;
            }

            float pulse = 0.8f + Mathf.Sin(Time.time * 4f) * 0.2f;
            m_OutlineObj.transform.localScale = Vector3.one * m_OutlineScale * pulse;
        }
    }
}
