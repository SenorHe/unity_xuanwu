#if UNITY_EDITOR_OSX || UNITY_EDITOR_WIN
using System;
using System.Text;
using UnityTcp.Editor.Helpers;
using UnityEngine;

namespace Cn.Tuanjie.Codely.Editor
{
    /// <summary>
    /// Analytics event tracker. Sends events from Unity to the Tauri backend via IPC,
    /// which forwards them to the remote server. Tauri already holds the Unity user
    /// credentials (access token + user ID) and is the natural place to handle
    /// remote data submission.
    /// </summary>
    public static class CodelyEventTracker
    {
        public const string WelcomeWindowShown   = "welcome_window_shown";
        public const string WelcomeWindowClosed  = "welcome_window_closed";
        public const string WelcomeButtonClick   = "welcome_button_clicked";
        public const string CodelyPanelOpened    = "codely_panel_opened";
        public const string CodelyInputSubmitted = "codely_input_submitted";
        public const string CodelyPanelClosed    = "codely_panel_closed";

        private static bool? s_isTuanjie;

        public static void Track(string eventType, string data = null)
        {
            Debug.Log($"[CodelyEventTracker] Track called: {eventType}, s_isTuanjie={s_isTuanjie}");

            if (!s_isTuanjie.HasValue)
            {
                string editorType = TauriUtils.GetEditorType();
                s_isTuanjie = editorType == "tuanjie";
                Debug.Log($"[CodelyEventTracker] GetEditorType() returned '{editorType}', s_isTuanjie={s_isTuanjie}");
            }
            if (!s_isTuanjie.Value)
                return;

            try
            {
                string json = BuildPayload(eventType, data);
                CodelyIpcManager.TrySend(IpcMessageType.TrackEvent, json);
                CodelyLogger.Log($"Codely EventTracker: {eventType}");
            }
            catch (Exception ex)
            {
                CodelyLogger.LogWarning($"Codely EventTracker error: {ex.Message}");
            }
        }

        private static string BuildPayload(string eventType, string data)
        {
            var sb = new StringBuilder();
            sb.Append("{\"event_type\":\"").Append(EscapeJson(eventType)).Append("\"");
            sb.Append(",\"timestamp\":\"").Append(DateTimeOffset.UtcNow.ToString("o")).Append("\"");
            if (!string.IsNullOrEmpty(data))
            {
                sb.Append(",\"data\":\"").Append(EscapeJson(data)).Append("\"");
            }
            sb.Append("}");
            return sb.ToString();
        }

        private static string EscapeJson(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            var sb = new StringBuilder(s.Length);
            foreach (char c in s)
            {
                switch (c)
                {
                    case '"':  sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (c < 0x20) sb.AppendFormat("\\u{0:x4}", (int)c);
                        else sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }
    }
}
#endif
