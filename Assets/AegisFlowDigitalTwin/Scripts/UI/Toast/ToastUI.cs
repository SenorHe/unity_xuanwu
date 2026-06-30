using UnityEngine;
using UnityEngine.UI;

namespace AegisFlowDigitalTwin.UI
{
    /// <summary>
    /// Toast 通知 UI。短暂显示操作反馈消息。
    /// </summary>
    public sealed class ToastUI : MonoBehaviour
    {
        private Text m_Text;
        private Image m_Bg;
        private float m_DisplayTimer;
        private float m_DisplayDuration = 2.5f;

        public static ToastUI Create(Transform parent)
        {
            GameObject toast = new GameObject("ToastUI");
            toast.transform.SetParent(parent, false);

            RectTransform rt = toast.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 0f);
            rt.pivot = new Vector2(0.5f, 0f);
            rt.sizeDelta = new Vector2(400f, 40f);
            rt.anchoredPosition = new Vector2(0f, 140f);

            Image bg = toast.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.12f, 0f);

            HorizontalLayoutGroup hlg = toast.AddComponent<HorizontalLayoutGroup>();
            hlg.padding = new RectOffset(15, 15, 5, 5);
            hlg.childAlignment = TextAnchor.MiddleCenter;

            GameObject txtObj = new GameObject("Text");
            txtObj.transform.SetParent(toast.transform, false);
            Text txt = txtObj.AddComponent<Text>();
            txt.color = Color.white;
            txt.fontSize = 13;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.horizontalOverflow = HorizontalWrapMode.Wrap;

            RectTransform txtRt = txtObj.GetComponent<RectTransform>();
            txtRt.anchorMin = Vector2.zero;
            txtRt.anchorMax = Vector2.one;
            txtRt.sizeDelta = Vector2.zero;

            ToastUI toastUI = toast.AddComponent<ToastUI>();
            toastUI.m_Text = txt;
            toastUI.m_Bg = bg;
            toastUI.m_Text.gameObject.SetActive(false);
            return toastUI;
        }

        public void ShowToast(string message, Color color)
        {
            if (m_Text == null) return;

            m_Text.text = message;
            m_Text.color = color;
            m_Text.gameObject.SetActive(true);

            if (m_Bg != null)
            {
                m_Bg.color = new Color(0.1f, 0.1f, 0.12f, 0.9f);
            }

            m_DisplayTimer = m_DisplayDuration;
        }

        private void Update()
        {
            if (m_DisplayTimer <= 0f) return;

            m_DisplayTimer -= Time.deltaTime;

            if (m_DisplayTimer <= 0f)
            {
                if (m_Text != null) m_Text.gameObject.SetActive(false);
                if (m_Bg != null) m_Bg.color = new Color(0.1f, 0.1f, 0.12f, 0f);
            }
            else if (m_DisplayTimer < 0.5f)
            {
                float alpha = m_DisplayTimer / 0.5f;
                if (m_Bg != null)
                {
                    Color c = m_Bg.color;
                    c.a = alpha * 0.9f;
                    m_Bg.color = c;
                }
                if (m_Text != null)
                {
                    Color c = m_Text.color;
                    c.a = alpha;
                    m_Text.color = c;
                }
            }
        }
    }
}
