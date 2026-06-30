using UnityEngine;
using UnityEngine.UI;
using AegisFlow.Command;

namespace AegisFlowDigitalTwin.UI
{
    /// <summary>
    /// 顶部工具栏。实体按钮进入交互放置模式，Save/Load/Run 使用当前 ModelId。
    /// </summary>
    public sealed class ToolbarUI : MonoBehaviour
    {
        private UICommandDispatcher m_Dispatcher;
        private Editor.EditModeController m_EditMode;
        private Data.SimulationDC m_SimDC;

        public static ToolbarUI Create(
            Transform parent,
            UICommandDispatcher dispatcher,
            Editor.EditModeController editMode,
            Data.SimulationDC simDC)
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
            toolbar.m_EditMode = editMode;
            toolbar.m_SimDC = simDC;
            toolbar.CreateButtons();
            return toolbar;
        }

        private void CreateButtons()
        {
            CreateEntityButton("AGV", "AGV 搬运车", new Color(0.15f, 0.45f, 0.85f));
            CreateEntityButton("RACK", "货架", new Color(0.85f, 0.45f, 0.1f));
            CreateEntityButton("CHARGER", "充电站", new Color(0.1f, 0.6f, 0.3f));
            CreateEntityButton("CONVEYOR", "传送带", new Color(0.4f, 0.4f, 0.42f));
            CreateEntityButton("WORKSTATION", "工作站", new Color(0.4f, 0.2f, 0.6f));
            CreateEntityButton("SENSOR", "传感器", new Color(0.1f, 0.6f, 0.7f));

            CreateSpacer(20f);

            CreateActionButton("Save", () =>
            {
                string modelId = GetCurrentModelId();
                m_Dispatcher.Dispatch(new SaveModelCommand());
            }, new Color(0.2f, 0.5f, 0.2f));

            CreateActionButton("Load", () =>
            {
                string modelId = GetCurrentModelId();
                m_Dispatcher.Dispatch(new LoadModelCommand(modelId));
            }, new Color(0.2f, 0.4f, 0.5f));

            CreateActionButton("Run", () =>
            {
                string modelId = GetCurrentModelId();
                m_Dispatcher.Dispatch(new StartSimulationCommand(modelId));
            }, new Color(0.1f, 0.6f, 0.1f));

            CreateActionButton("Stop", () =>
            {
                m_Dispatcher.Dispatch(new StopSimulationCommand());
            }, new Color(0.6f, 0.1f, 0.1f));
        }

        private string GetCurrentModelId()
        {
            if (m_SimDC != null && !string.IsNullOrEmpty(m_SimDC.CurrentModelId))
            {
                return m_SimDC.CurrentModelId;
            }
            return "default";
        }

        private void CreateEntityButton(string type, string displayName, Color color)
        {
            CreateActionButton(displayName, () =>
            {
                if (m_EditMode != null)
                {
                    m_EditMode.SetPlacementType(type);
                }
            }, color);
        }

        private void CreateActionButton(string label, UnityEngine.Events.UnityAction onClick, Color color)
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
