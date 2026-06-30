using UnityEngine;

namespace AegisFlowDigitalTwin.Visual
{
    /// <summary>
    /// 传感器视觉。柱体 + 顶部脉冲环。运行态发射扫描脉冲。
    /// </summary>
    public sealed class SensorVisual : EntityVisualBase
    {
        private Material m_BodyMat;
        private Material m_RingMat;
        private GameObject m_RingObj;
        private float m_PulsePhase;

        protected override void OnInitialize()
        {
            m_BodyMat = CreateMaterial(new Color(0.1f, 0.6f, 0.7f), 0.5f);
            m_RingMat = CreateMaterial(new Color(0.2f, 0.9f, 1f, 0.5f), 0.8f);

            CreateChild("Pole", PrimitiveType.Cylinder, new Vector3(0f, 0.5f, 0f), new Vector3(0.1f, 0.5f, 0.1f), m_BodyMat, transform);
            CreateChild("Base", PrimitiveType.Cube, new Vector3(0f, 0.05f, 0f), new Vector3(0.3f, 0.1f, 0.3f), m_BodyMat, transform);
            CreateChild("Head", PrimitiveType.Sphere, new Vector3(0f, 1f, 0f), new Vector3(0.18f, 0.18f, 0.18f), m_BodyMat, transform);

            m_RingObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            m_RingObj.name = "PulseRing";
            m_RingObj.transform.SetParent(transform);
            m_RingObj.transform.localPosition = new Vector3(0f, 1f, 0f);
            m_RingObj.transform.localScale = new Vector3(0.2f, 0.02f, 0.2f);

            MeshRenderer ringRenderer = m_RingObj.GetComponent<MeshRenderer>();
            if (ringRenderer != null)
            {
                ringRenderer.material = m_RingMat;
            }
        }

        private void Update()
        {
            if (m_RingObj == null)
            {
                return;
            }

            if (Status == "Scanning")
            {
                m_PulsePhase += Time.deltaTime * 1.5f;
                if (m_PulsePhase > 1f)
                {
                    m_PulsePhase = 0f;
                }

                float scale = 0.2f + m_PulsePhase * 3f;
                m_RingObj.transform.localScale = new Vector3(scale, 0.02f, scale);

                if (m_RingMat != null)
                {
                    Color c = m_RingMat.color;
                    c.a = (1f - m_PulsePhase) * 0.5f;
                    m_RingMat.color = c;
                }
            }
            else
            {
                m_RingObj.transform.localScale = new Vector3(0.2f, 0.02f, 0.2f);
            }
        }

        protected override void OnStatusChanged(string status)
        {
        }

        protected override void OnSelectionChanged(bool selected)
        {
            if (m_BodyMat == null)
            {
                return;
            }

            m_BodyMat.color = selected
                ? new Color(1f, 0.85f, 0.2f)
                : new Color(0.1f, 0.6f, 0.7f);
        }
    }
}
