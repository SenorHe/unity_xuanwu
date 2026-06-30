using System.Collections.Generic;
using AegisFlow.Data;
using UnityEngine;
using UnityEngine.UI;

namespace AegisFlowDigitalTwin.UI
{
    /// <summary>
    /// 左侧实体列表面板。展示所有已放置实体的 ID 和类型。
    /// </summary>
    public sealed class EntityListPanel : MonoBehaviour
    {
        private EntityRepository m_Repository;
        private Transform m_Content;
        private readonly List<GameObject> m_Items = new List<GameObject>();

        public static EntityListPanel Create(Transform parent, EntityRepository repository)
        {
            GameObject panel = CreatePanel(parent, "EntityListPanel", new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(250f, 350f), new Vector2(10f, -60f));
            CreateTitle(panel.transform, "Entities");

            EntityListPanel list = panel.AddComponent<EntityListPanel>();
            list.m_Repository = repository;

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
            scrollBg.color = new Color(0.08f, 0.09f, 0.12f, 0.8f);

            GameObject content = new GameObject("Content");
            content.transform.SetParent(scrollObj.transform, false);
            RectTransform contentRt = content.AddComponent<RectTransform>();
            contentRt.anchorMin = new Vector2(0f, 1f);
            contentRt.anchorMax = new Vector2(1f, 1f);
            contentRt.pivot = new Vector2(0.5f, 1f);
            contentRt.sizeDelta = new Vector2(0f, 0f);

            VerticalLayoutGroup vlg = content.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 3f;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scroll.content = contentRt;
            list.m_Content = content.transform;

            list.Refresh();
            return list;
        }

        public void Refresh()
        {
            ClearItems();
            if (m_Repository == null) return;

            foreach (EntityData entity in m_Repository.GetAll())
            {
                CreateItem(entity);
            }
        }

        private void CreateItem(EntityData entity)
        {
            GameObject item = new GameObject($"Item_{entity.EntityId}");
            item.transform.SetParent(m_Content, false);

            Image img = item.AddComponent<Image>();
            img.color = new Color(0.15f, 0.17f, 0.2f, 0.9f);

            HorizontalLayoutGroup hlg = item.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 5f;
            hlg.padding = new RectOffset(5, 5, 3, 3);
            hlg.childForceExpandHeight = false;

            LayoutElement le = item.AddComponent<LayoutElement>();
            le.preferredHeight = 24f;

            CreateText(item.transform, entity.EntityType, 80f, GetTypeColor(entity.EntityType));
            CreateText(item.transform, entity.EntityId, 120f, Color.white);
            CreateText(item.transform, entity.Status, 60f, GetStatusColor(entity.Status));

            m_Items.Add(item);
        }

        private void CreateText(Transform parent, string text, float width, Color color)
        {
            GameObject obj = new GameObject("Text");
            obj.transform.SetParent(parent, false);
            Text txt = obj.AddComponent<Text>();
            txt.text = text;
            txt.color = color;
            txt.fontSize = 11;
            txt.alignment = TextAnchor.MiddleLeft;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            LayoutElement le = obj.AddComponent<LayoutElement>();
            le.preferredWidth = width;

            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
        }

        private Color GetTypeColor(string type)
        {
            return type switch
            {
                "AGV" => new Color(0.3f, 0.6f, 1f),
                "RACK" => new Color(1f, 0.6f, 0.3f),
                "CHARGER" => new Color(0.3f, 1f, 0.5f),
                "CONVEYOR" => new Color(0.6f, 0.6f, 0.65f),
                "WORKSTATION" => new Color(0.7f, 0.4f, 1f),
                "SENSOR" => new Color(0.3f, 0.8f, 1f),
                _ => Color.white
            };
        }

        private Color GetStatusColor(string status)
        {
            return status switch
            {
                "Placed" => Color.cyan,
                "Moving" => Color.yellow,
                "Arrived" => Color.green,
                "Charging" => Color.magenta,
                "Error" or "Failed" => Color.red,
                _ => Color.gray
            };
        }

        private void ClearItems()
        {
            foreach (GameObject item in m_Items)
            {
                if (item != null) Destroy(item);
            }
            m_Items.Clear();
        }

        public static GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 size, Vector2 position)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);

            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = new Vector2(0f, 0.5f);
            rt.sizeDelta = size;
            rt.anchoredPosition = position;

            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.06f, 0.07f, 0.09f, 0.95f);

            return panel;
        }

        public static void CreateTitle(Transform parent, string title)
        {
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(parent, false);
            Text txt = titleObj.AddComponent<Text>();
            txt.text = title;
            txt.color = new Color(0.8f, 0.85f, 0.9f);
            txt.fontSize = 14;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.UpperLeft;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            RectTransform rt = titleObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(0f, 25f);
            rt.anchoredPosition = new Vector2(0f, 0f);
        }
    }
}
