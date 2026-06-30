using UnityEngine;

namespace AegisFlowDigitalTwin.Visual
{
    /// <summary>
    /// 货架视觉。框架 + 3 层隔板。
    /// </summary>
    public sealed class RackVisual : EntityVisualBase
    {
        private Material m_FrameMat;
        private Material m_ShelfMat;

        protected override void OnInitialize()
        {
            m_FrameMat = CreateMaterial(new Color(0.85f, 0.45f, 0.1f), 0.5f);
            m_ShelfMat = CreateMaterial(new Color(0.6f, 0.35f, 0.08f), 0.4f);

            CreateChild("Base", PrimitiveType.Cube, new Vector3(0f, 0.05f, 0f), new Vector3(1.2f, 0.1f, 0.8f), m_ShelfMat, transform);

            float postX = 0.55f;
            float postZ = 0.35f;
            float postHeight = 1.8f;
            float postY = postHeight / 2f;

            CreateChild("Post_FL", PrimitiveType.Cube, new Vector3(-postX, postY, postZ), new Vector3(0.08f, postHeight, 0.08f), m_FrameMat, transform);
            CreateChild("Post_FR", PrimitiveType.Cube, new Vector3(postX, postY, postZ), new Vector3(0.08f, postHeight, 0.08f), m_FrameMat, transform);
            CreateChild("Post_RL", PrimitiveType.Cube, new Vector3(-postX, postY, -postZ), new Vector3(0.08f, postHeight, 0.08f), m_FrameMat, transform);
            CreateChild("Post_RR", PrimitiveType.Cube, new Vector3(postX, postY, -postZ), new Vector3(0.08f, postHeight, 0.08f), m_FrameMat, transform);

            for (int i = 0; i < 3; i++)
            {
                float shelfY = 0.5f + i * 0.55f;
                CreateChild($"Shelf_{i}", PrimitiveType.Cube, new Vector3(0f, shelfY, 0f), new Vector3(1.15f, 0.05f, 0.75f), m_ShelfMat, transform);
            }
        }

        protected override void OnStatusChanged(string status)
        {
        }

        protected override void OnSelectionChanged(bool selected)
        {
            if (m_FrameMat == null)
            {
                return;
            }

            m_FrameMat.color = selected
                ? new Color(1f, 0.85f, 0.2f)
                : new Color(0.85f, 0.45f, 0.1f);
        }
    }
}
