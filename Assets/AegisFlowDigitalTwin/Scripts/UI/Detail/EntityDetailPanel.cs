using UnityEngine;
using UnityEngine.UI;

namespace AegisFlowDigitalTwin.UI
{
    /// <summary>
    /// 右侧实体详情面板。展示选中实体的详细信息和遥测数据。
    /// </summary>
    public sealed class EntityDetailPanel : MonoBehaviour
    {
        private Text m_InfoText;

        public static EntityDetailPanel Create(Transform parent)
        {
            GameObject panel = EntityListPanel.CreatePanel(parent, "DetailPanel",
                new Vector2(1f, 0.5f), new Vector2(1f, 0.5f),
                new Vector2(250f, 350f), new Vector2(-260f, -60f));

            EntityListPanel.CreateTitle(panel.transform, "Details");

            EntityDetailPanel detail = panel.AddComponent<EntityDetailPanel>();

            GameObject infoObj = new GameObject("InfoText");
            infoObj.transform.SetParent(panel.transform, false);
            Text txt = infoObj.AddComponent<Text>();
            txt.color = new Color(0.7f, 0.75f, 0.8f);
            txt.fontSize = 11;
            txt.alignment = TextAnchor.UpperLeft;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.supportRichText = true;

            RectTransform rt = infoObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(-10f, -30f);
            rt.anchoredPosition = new Vector2(0f, -10f);

            ContentSizeFitter fitter = infoObj.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            detail.m_InfoText = txt;
            return detail;
        }

        public void ShowEntity(string entityId, string entityType, string status, float posX, float posZ, float battery)
        {
            if (m_InfoText == null) return;

            m_InfoText.text =
                $"<b>Entity ID:</b> {entityId}\n" +
                $"<b>Type:</b> {entityType}\n" +
                $"<b>Status:</b> {status}\n" +
                $"<b>Position:</b> ({posX:F1}, {posZ:F1})\n" +
                $"<b>Battery:</b> {battery:F0}%\n";
        }

        public void Clear()
        {
            if (m_InfoText != null)
            {
                m_InfoText.text = "Select an entity to view details.";
            }
        }
    }
}
