using UnityEngine;

namespace AegisFlowDigitalTwin.Visual
{
    /// <summary>
    /// 充电站视觉。柱体 + 屏幕 + 充电指示灯。
    /// </summary>
    public sealed class ChargerVisual : EntityVisualBase
    {
        private Material m_PillarMat;
        private Material m_ScreenMat;
        private Material m_LightMat;
        private GameObject m_LightObj;

        protected override void OnInitialize()
        {
            m_PillarMat = CreateMaterial(new Color(0.1f, 0.6f, 0.3f), 0.5f);
            m_ScreenMat = CreateMaterial(new Color(0.05f, 0.15f, 0.08f), 0.2f);
            m_LightMat = CreateMaterial(new Color(0f, 0.8f, 0.4f), 0.9f);

            CreateChild("Pillar", PrimitiveType.Cylinder, new Vector3(0f, 1f, 0f), new Vector3(0.3f, 1f, 0.3f), m_PillarMat, transform);
            CreateChild("Base", PrimitiveType.Cube, new Vector3(0f, 0.05f, 0f), new Vector3(0.8f, 0.1f, 0.8f), m_PillarMat, transform);
            CreateChild("Screen", PrimitiveType.Cube, new Vector3(0f, 1.8f, 0.15f), new Vector3(0.4f, 0.3f, 0.05f), m_ScreenMat, transform);

            m_LightObj = CreateChild("ChargeLight", PrimitiveType.Sphere, new Vector3(0f, 1.8f, 0f), new Vector3(0.08f, 0.08f, 0.08f), m_LightMat, transform);
        }

        protected override void OnStatusChanged(string status)
        {
            if (m_LightObj == null)
            {
                return;
            }

            MeshRenderer renderer = m_LightObj.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                return;
            }

            renderer.material.color = status == "Charging"
                ? new Color(0f, 1f, 0.5f)
                : new Color(0.3f, 0.3f, 0.3f);
        }

        protected override void OnSelectionChanged(bool selected)
        {
            if (m_PillarMat == null)
            {
                return;
            }

            m_PillarMat.color = selected
                ? new Color(1f, 0.85f, 0.2f)
                : new Color(0.1f, 0.6f, 0.3f);
        }
    }
}
