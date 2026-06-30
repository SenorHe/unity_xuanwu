using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AegisFlowDigitalTwin.UI
{
    /// <summary>
    /// 底部事件日志面板。滚动展示所有操作和事件日志。
    /// </summary>
    public sealed class EventLogPanel : MonoBehaviour
    {
        private Transform m_Content;
        private readonly List<GameObject> m_Items = new List<GameObject>();
        private const int m_MaxItems = 30;

        public static EventLogPanel Create(Transform parent)
        {
            GameObject panel = EntityListPanel.CreatePanel(parent, "EventLogPanel",
                new Vector2(0f, 0f), new Vector2(1f, 0f),
                new Vector2(0f, 120f), new Vector2(0f, 10f));

            RectTransform rt = panel.GetComponent<RectTransform>();
            rt.pivot = new Vector2(0.5f, 0f);
            rt.offsetMin = new Vector2(10f, 10f);
            rt.offsetMax = new Vector2(-10f, 130f);

            EntityListPanel.CreateTitle(panel.transform, "Event Log");

            GameObject scrollObj = new GameObject("ScrollView");
            scrollObj.transform.SetParent(panel.transform, false);
            RectTransform scrollRt = scrollObj.AddComponent<RectTransform>();
            scrollRt.anchorMin = new Vector2(0f, 0f);
            scrollRt.anchorMax = new Vector2(1f, 1f);
            scrollRt.pivot = new Vector2(0.5f, 0.5f);
            scrollRt.sizeDelta = new Vector2(-10f, -30f);
            scrollRt.anchoredPosition = new Vector2(0f, -10f);

            ScrollRect scroll = scrollObj.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;

            Image scrollBg = scrollObj.AddComponent<Image>();
            scrollBg.color = new Color(0.05f, 0.06f, 0.08f, 0.8f);

            GameObject content = new GameObject("Content");
            content.transform.SetParent(scrollObj.transform, false);
            RectTransform contentRt = content.AddComponent<RectTransform>();
            contentRt.anchorMin = new Vector2(0f, 1f);
            contentRt.anchorMax = new Vector2(1f, 1f);
            contentRt.pivot = new Vector2(0.5f, 1f);

            VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 2f;
            vlg.childControlWidth = true;
            vlg.childForceExpandWidth = true;

            ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scroll.content = contentRt;

            EventLogPanel log = panel.AddComponent<EventLogPanel>();
            log.m_Content = content.transform;
            return log;
        }

        public void AddEntry(string text, bool isSuccess)
        {
            GameObject item = new GameObject($"Log_{m_Items.Count}");
            item.transform.SetParent(m_Content, false);

            Text txt = item.AddComponent<Text>();
            txt.text = text;
            txt.color = isSuccess ? new Color(0.6f, 0.7f, 0.6f) : new Color(0.9f, 0.4f, 0.4f);
            txt.fontSize = 10;
            txt.alignment = TextAnchor.UpperLeft;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            LayoutElement le = item.AddComponent<LayoutElement>();
            le.preferredHeight = 16f;

            m_Items.Add(item);

            if (m_Items.Count > m_MaxItems)
            {
                GameObject oldest = m_Items[0];
                m_Items.RemoveAt(0);
                if (oldest != null) Destroy(oldest);
            }
        }

        public void Clear()
        {
            foreach (GameObject item in m_Items)
            {
                if (item != null) Destroy(item);
            }
            m_Items.Clear();
        }
    }
}
