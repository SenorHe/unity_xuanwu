using UnityEngine;

namespace AegisFlowDigitalTwin.Performance
{
    /// <summary>
    /// 性能监控。实时显示 FPS 和内存使用。
    /// </summary>
    public sealed class PerformanceMonitor : MonoBehaviour
    {
        [SerializeField] private float m_UpdateInterval = 0.5f;

        private float m_AccumulatedTime;
        private int m_FrameCount;
        private float m_CurrentFPS;

        public float CurrentFPS => m_CurrentFPS;
        public float UsedMemoryMB => System.GC.GetTotalMemory(false) / (1024f * 1024f);

        public static PerformanceMonitor Create(Transform parent)
        {
            GameObject obj = new GameObject("PerformanceMonitor");
            obj.transform.SetParent(parent, false);

            RectTransform rt = obj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(1f, 0f);
            rt.anchorMax = new Vector2(1f, 0f);
            rt.pivot = new Vector2(1f, 0f);
            rt.sizeDelta = new Vector2(200f, 30f);
            rt.anchoredPosition = new Vector2(-10f, 140f);

            UnityEngine.UI.Text text = obj.AddComponent<UnityEngine.UI.Text>();
            text.color = new Color(0.5f, 0.6f, 0.5f);
            text.fontSize = 11;
            text.alignment = TextAnchor.LowerRight;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.raycastTarget = false;

            PerformanceMonitor monitor = obj.AddComponent<PerformanceMonitor>();
            return monitor;
        }

        private void Update()
        {
            m_AccumulatedTime += Time.deltaTime;
            m_FrameCount++;

            if (m_AccumulatedTime >= m_UpdateInterval)
            {
                m_CurrentFPS = m_FrameCount / m_AccumulatedTime;
                m_AccumulatedTime = 0f;
                m_FrameCount = 0;

                UnityEngine.UI.Text text = GetComponent<UnityEngine.UI.Text>();
                if (text != null)
                {
                    text.text = $"FPS: {m_CurrentFPS:F0} | Mem: {UsedMemoryMB:F0}MB";
                }
            }
        }
    }
}
