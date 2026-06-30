using UnityEngine;

namespace AegisFlowDigitalTwin.Effects
{
    /// <summary>
    /// 小地图控制器。俯视投影渲染场景到 RenderTexture。
    /// </summary>
    public sealed class MinimapController : MonoBehaviour
    {
        [SerializeField] private float m_OrthographicSize = 35f;
        [SerializeField] private Vector2 m_DisplaySize = new Vector2(150f, 150f);
        [SerializeField] private Vector2 m_DisplayPosition = new Vector2(10f, -10f);

        private Camera m_MinimapCamera;
        private RenderTexture m_RenderTexture;
        private UnityEngine.UI.RawImage m_Display;

        public void Initialize(Transform parent)
        {
            CreateMinimapCamera();
            CreateDisplay(parent);
        }

        private void CreateMinimapCamera()
        {
            GameObject camObj = new GameObject("MinimapCamera");
            camObj.transform.SetParent(transform);
            camObj.transform.position = new Vector3(0f, 50f, 0f);
            camObj.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            m_MinimapCamera = camObj.AddComponent<Camera>();
            m_MinimapCamera.orthographic = true;
            m_MinimapCamera.orthographicSize = m_OrthographicSize;
            m_MinimapCamera.clearFlags = CameraClearFlags.SolidColor;
            m_MinimapCamera.backgroundColor = new Color(0.08f, 0.1f, 0.12f, 1f);
            m_MinimapCamera.cullingMask = ~0;
            m_MinimapCamera.depth = -1;

            m_RenderTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
            m_MinimapCamera.targetTexture = m_RenderTexture;
        }

        private void CreateDisplay(Transform parent)
        {
            GameObject displayObj = new GameObject("MinimapDisplay");
            displayObj.transform.SetParent(parent, false);

            RectTransform rt = displayObj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.sizeDelta = m_DisplaySize;
            rt.anchoredPosition = new Vector2(m_DisplayPosition.x, m_DisplayPosition.y);

            m_Display = displayObj.AddComponent<UnityEngine.UI.RawImage>();
            m_Display.texture = m_RenderTexture;
            m_Display.color = new Color(0.9f, 0.9f, 0.9f, 0.95f);
        }

        private void OnDestroy()
        {
            if (m_RenderTexture != null)
            {
                m_RenderTexture.Release();
                Destroy(m_RenderTexture);
            }
        }
    }
}
