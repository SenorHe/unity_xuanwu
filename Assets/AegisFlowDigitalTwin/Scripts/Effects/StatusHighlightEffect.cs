using UnityEngine;

namespace AegisFlowDigitalTwin.Effects
{
    /// <summary>
    /// 实体状态高亮。运行/空闲/故障显示不同颜色光晕。
    /// </summary>
    public sealed class StatusHighlightEffect : MonoBehaviour
    {
        [SerializeField] private float m_PulseSpeed = 2f;
        [SerializeField] private float m_MinIntensity = 0.3f;
        [SerializeField] private float m_MaxIntensity = 0.8f;

        private Material m_Material;
        private string m_Status = "Idle";
        private Color m_BaseColor = Color.white;
        private float m_PulsePhase;

        private void Awake()
        {
            MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
            if (renderer != null)
            {
                m_Material = renderer.material;
                m_BaseColor = m_Material.color;
            }
        }

        public void SetStatus(string status)
        {
            m_Status = status;
            m_BaseColor = GetStatusColor(status);
        }

        private Color GetStatusColor(string status)
        {
            return status switch
            {
                "Moving" => new Color(0f, 0.8f, 1f),
                "Arrived" => new Color(0f, 1f, 0.4f),
                "Charging" => new Color(1f, 0f, 1f),
                "Working" => new Color(1f, 0.8f, 0f),
                "Error" or "Failed" => new Color(1f, 0.1f, 0.1f),
                _ => new Color(0.5f, 0.5f, 0.5f)
            };
        }

        private void Update()
        {
            if (m_Material == null) return;

            m_PulsePhase += Time.deltaTime * m_PulseSpeed;
            float t = (Mathf.Sin(m_PulsePhase) + 1f) / 2f;
            float intensity = Mathf.Lerp(m_MinIntensity, m_MaxIntensity, t);

            m_Material.color = m_BaseColor * intensity;
            m_Material.SetColor("_EmissionColor", m_BaseColor * intensity);
        }
    }
}
