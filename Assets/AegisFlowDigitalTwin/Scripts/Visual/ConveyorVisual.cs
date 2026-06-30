using UnityEngine;

namespace AegisFlowDigitalTwin.Visual
{
    /// <summary>
    /// 传送带视觉。框架 + 滚筒 + 带面。运行态带面滚动动画。
    /// </summary>
    public sealed class ConveyorVisual : EntityVisualBase
    {
        private Material m_FrameMat;
        private Material m_BeltMat;
        private float m_BeltOffset;
        private bool m_IsRunning;

        protected override void OnInitialize()
        {
            m_FrameMat = CreateMaterial(new Color(0.4f, 0.4f, 0.42f), 0.4f);
            m_BeltMat = CreateMaterial(new Color(0.12f, 0.12f, 0.14f), 0.7f);

            CreateChild("Frame_L", PrimitiveType.Cube, new Vector3(-0.4f, 0.2f, 0f), new Vector3(0.1f, 0.4f, 1.5f), m_FrameMat, transform);
            CreateChild("Frame_R", PrimitiveType.Cube, new Vector3(0.4f, 0.2f, 0f), new Vector3(0.1f, 0.4f, 1.5f), m_FrameMat, transform);

            GameObject belt = CreateChild("Belt", PrimitiveType.Cube, new Vector3(0f, 0.35f, 0f), new Vector3(0.7f, 0.04f, 1.4f), m_BeltMat, transform);

            for (int i = 0; i < 5; i++)
            {
                float z = -0.5f + i * 0.25f;
                CreateChild($"Roller_{i}", PrimitiveType.Cylinder, new Vector3(0f, 0.32f, z), new Vector3(0.35f, 0.03f, 0.35f), m_FrameMat, transform);
            }
        }

        private void Update()
        {
            if (!m_IsRunning || m_BeltMat == null)
            {
                return;
            }

            m_BeltOffset += Time.deltaTime * 0.5f;
            if (m_BeltOffset > 1f)
            {
                m_BeltOffset -= 1f;
            }

            m_BeltMat.mainTextureOffset = new Vector2(0f, m_BeltOffset);
        }

        protected override void OnStatusChanged(string status)
        {
            m_IsRunning = status == "Running" || status == "Active";
        }

        protected override void OnSelectionChanged(bool selected)
        {
            if (m_FrameMat == null)
            {
                return;
            }

            m_FrameMat.color = selected
                ? new Color(1f, 0.85f, 0.2f)
                : new Color(0.4f, 0.4f, 0.42f);
        }
    }
}
