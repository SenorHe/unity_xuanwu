using UnityEngine;
using UnityEngine.UI;

namespace AegisFlowDigitalTwin.UI
{
    /// <summary>
    /// 顶部工具栏。包含放置、保存、加载、运行、停止按钮。
    /// </summary>
    public sealed class ToolbarUI : MonoBehaviour
    {
        private AegisFlow.Command.UICommandDispatcher m_Dispatcher;

        public static ToolbarUI Create(Transform parent, AegisFlow.Command.UICommandDispatcher dispatcher)
        {
            GameObject obj = new GameObject("Toolbar");
            obj.transform.SetParent(parent, false);

            RectTransform rt = obj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(0f, 50f);
            rt.anchoredPosition = Vector2.zero;

            HorizontalLayoutGroup layout = obj.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 8f;
            layout.padding = new RectOffset(10, 10, 5, 5);
            layout.childAlignment = TextAnchor.MiddleLeft;

            Image bg = obj.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.12f, 0.16f, 0.9f);

            ToolbarUI toolbar = obj.AddComponent<ToolbarUI>();
            toolbar.m_Dispatcher = dispatcher;
            toolbar.CreateButtons();

            return toolbar;
        }

        private void CreateButtons()
        {
            CreateButton("AGV", () => SelectEntityType("AGV"), new Color(0.15f, 0.45f, 0.85f));
            CreateButton("RACK", () => SelectEntityType("RACK"), new Color(0.85f, 0.45f, 0.1f));
            CreateButton("CHARGER", () => SelectEntityType("CHARGER"), new Color(0.1f, 0.6f, 0.3f));
            CreateButton("CONVEYOR", () => SelectEntityType("CONVEYOR"), new Color(0.4f, 0.4f, 0.42f));
            CreateButton("WORKSTATION", () => SelectEntityType("WORKSTATION"), new Color(0.4f, 0.2f, 0.6f));
            CreateButton("SENSOR", () => SelectEntityType("SENSOR"), new Color(0.1f, 0.6f, 0.7f));

            CreateSpacer(20f);

            CreateButton("Save", () => m_Dispatcher.Dispatch(new AegisFlow.Command.SaveModelCommand()), new Color(0.2f, 0.5f, 0.2f));
            CreateButton("Load", () => m_Dispatcher.Dispatch(new AegisFlow.Command.LoadModelCommand("default")), new Color(0.2f, 0.4f, 0.5f));
            CreateButton("Run", () => m_Dispatcher.Dispatch(new AegisFlow.Command.StartSimulationCommand("default")), new Color(0.1f, 0.6f, 0.1f));
            CreateButton("Stop", () => m_Dispatcher.Dispatch(new AegisFlow.Command.StopSimulationCommand()), new Color(0.6f, 0.1f, 0.1f));
        }

        private void SelectEntityType(string type)
        {
            string entityId = $"{type}_{System.Guid.NewGuid():N}".Substring(0, 12);
            m_Dispatcher.Dispatch(new AegisFlow.Command.PlaceEntityCommand(
                type, entityId, type, 0f, 0f, 0f, 0f));
        }

        private void CreateButton(string label, UnityEngine.Events.UnityAction onClick, Color color)
        {
            GameObject btnObj = new GameObject($"Btn_{label}");
            btnObj.transform.SetParent(transform, false);

            Image img = btnObj.AddComponent<Image>();
            img.color = color;

            Button btn = btnObj.AddComponent<Button>();
            btn.onClick.AddListener(onClick);

            LayoutElement le = btnObj.AddComponent<LayoutElement>();
            le.preferredWidth = 90f;
            le.preferredHeight = 35f;

            GameObject txtObj = new GameObject("Text");
            txtObj.transform.SetParent(btnObj.transform, false);
            Text txt = txtObj.AddComponent<Text>();
            txt.text = label;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 13;

            RectTransform txtRt = txt.GetComponent<RectTransform>();
            txtRt.anchorMin = Vector2.zero;
            txtRt.anchorMax = Vector2.one;
            txtRt.sizeDelta = Vector2.zero;
        }

        private void CreateSpacer(float width)
        {
            GameObject spacer = new GameObject("Spacer");
            spacer.transform.SetParent(transform, false);
            LayoutElement le = spacer.AddComponent<LayoutElement>();
            le.preferredWidth = width;
        }
    }
}
