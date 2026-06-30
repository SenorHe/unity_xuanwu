using UnityEngine;
using UnityEngine.UI;

namespace AegisFlowDigitalTwin.UI
{
    /// <summary>
    /// 失败恢复面板。仿真事件失败时显示 Retry / Skip / Terminate 按钮。
    /// </summary>
    public sealed class RecoveryPanel : MonoBehaviour
    {
        private AegisFlow.Command.UICommandDispatcher m_Dispatcher;
        private Text m_MessageText;

        public static RecoveryPanel Create(Transform parent, AegisFlow.Command.UICommandDispatcher dispatcher)
        {
            GameObject panel = new GameObject("RecoveryPanel");
            panel.transform.SetParent(parent, false);

            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(400f, 120f);
            rt.anchoredPosition = new Vector2(0f, 50f);

            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.3f, 0.1f, 0.1f, 0.95f);

            VerticalLayoutGroup vlg = panel.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 8f;
            vlg.padding = new RectOffset(10, 10, 10, 10);
            vlg.childAlignment = TextAnchor.MiddleCenter;

            RecoveryPanel recovery = panel.AddComponent<RecoveryPanel>();
            recovery.m_Dispatcher = dispatcher;

            GameObject msgObj = new GameObject("Message");
            msgObj.transform.SetParent(panel.transform, false);
            Text msgTxt = msgObj.AddComponent<Text>();
            msgTxt.text = "";
            msgTxt.color = Color.yellow;
            msgTxt.fontSize = 13;
            msgTxt.alignment = TextAnchor.MiddleCenter;
            msgTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            msgTxt.horizontalOverflow = HorizontalWrapMode.Wrap;

            RectTransform msgRt = msgObj.GetComponent<RectTransform>();
            msgRt.anchorMin = Vector2.zero;
            msgRt.anchorMax = Vector2.one;
            recovery.m_MessageText = msgTxt;

            GameObject btnRow = new GameObject("Buttons");
            btnRow.transform.SetParent(panel.transform, false);
            HorizontalLayoutGroup hlg = btnRow.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 10f;
            hlg.childAlignment = TextAnchor.MiddleCenter;

            recovery.CreateRecoveryButton("Retry", () => dispatcher.Dispatch(new AegisFlow.Command.RetrySimulationEventCommand()), new Color(0.2f, 0.5f, 0.2f), btnRow.transform);
            recovery.CreateRecoveryButton("Skip", () => dispatcher.Dispatch(new AegisFlow.Command.SkipSimulationEventCommand()), new Color(0.5f, 0.4f, 0.1f), btnRow.transform);
            recovery.CreateRecoveryButton("Terminate", () => dispatcher.Dispatch(new AegisFlow.Command.TerminateSimulationRuntimeCommand()), new Color(0.6f, 0.1f, 0.1f), btnRow.transform);

            panel.SetActive(false);
            return recovery;
        }

        public void Show(string message)
        {
            gameObject.SetActive(true);
            if (m_MessageText != null)
            {
                m_MessageText.text = message;
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void CreateRecoveryButton(string label, UnityEngine.Events.UnityAction onClick, Color color, Transform parent)
        {
            GameObject btnObj = new GameObject($"Btn_{label}");
            btnObj.transform.SetParent(parent, false);

            Image img = btnObj.AddComponent<Image>();
            img.color = color;

            Button btn = btnObj.AddComponent<Button>();
            btn.onClick.AddListener(onClick);

            LayoutElement le = btnObj.AddComponent<LayoutElement>();
            le.preferredWidth = 100f;
            le.preferredHeight = 30f;

            GameObject txtObj = new GameObject("Text");
            txtObj.transform.SetParent(btnObj.transform, false);
            Text txt = txtObj.AddComponent<Text>();
            txt.text = label;
            txt.color = Color.white;
            txt.fontSize = 12;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            RectTransform txtRt = txtObj.GetComponent<RectTransform>();
            txtRt.anchorMin = Vector2.zero;
            txtRt.anchorMax = Vector2.one;
            txtRt.sizeDelta = Vector2.zero;
        }
    }
}
