using UnityEngine;
using UnityEngine.UI;

namespace AegisFlowDigitalTwin.UI
{
    /// <summary>
    /// 运行态仪表盘。展示当前步数、事件数、运行状态。
    /// </summary>
    public sealed class RuntimeDashboardPanel : MonoBehaviour
    {
        private Text m_StepText;
        private Text m_StatusText;
        private bool m_IsActive;

        public static RuntimeDashboardPanel Create(Transform parent)
        {
            GameObject panel = EntityListPanel.CreatePanel(parent, "RuntimeDashboard",
                new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(300f, 60f), new Vector2(-260f, -10f));

            RuntimeDashboardPanel dashboard = panel.AddComponent<RuntimeDashboardPanel>();

            HorizontalLayoutGroup hlg = panel.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 15f;
            hlg.padding = new RectOffset(10, 10, 5, 5);
            hlg.childAlignment = TextAnchor.MiddleLeft;

            dashboard.m_StepText = CreateLabel(panel.transform, "Step: 0", 120f);
            dashboard.m_StatusText = CreateLabel(panel.transform, "Idle", 120f);

            dashboard.SetRuntimeActive(false);
            return dashboard;
        }

        private static Text CreateLabel(Transform parent, string text, float width)
        {
            GameObject obj = new GameObject($"Label_{text}");
            obj.transform.SetParent(parent, false);
            Text txt = obj.AddComponent<Text>();
            txt.text = text;
            txt.color = new Color(0.7f, 0.75f, 0.8f);
            txt.fontSize = 12;
            txt.alignment = TextAnchor.MiddleLeft;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            LayoutElement le = obj.AddComponent<LayoutElement>();
            le.preferredWidth = width;

            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            return txt;
        }

        public void UpdateStep(int step)
        {
            if (m_StepText != null)
            {
                m_StepText.text = $"Step: {step}";
            }
        }

        public void SetRuntimeActive(bool active)
        {
            m_IsActive = active;
            if (m_StatusText != null)
            {
                m_StatusText.text = active ? "Running" : "Idle";
                m_StatusText.color = active ? Color.green : Color.gray;
            }
        }
    }
}
