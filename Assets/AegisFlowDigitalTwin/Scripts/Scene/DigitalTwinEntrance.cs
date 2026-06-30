using AegisFlow.Bootstrap;
using AegisFlowDigitalTwin.UI;
using AegisFlowDigitalTwin.Visual;
using UnityEngine;

namespace AegisFlowDigitalTwin.Scene
{
    /// <summary>
    /// 一键启动入口。在场景中放置此 MonoBehaviour 即可启动整个数字孪生系统。
    /// </summary>
    public sealed class DigitalTwinEntrance : MonoBehaviour
    {
        private SceneBootstrap m_SceneBootstrap;
        private UIRoot m_UIRoot;
        private SimulationBridge m_SimBridge;
        private EntityVisualRegistry m_VisualRegistry;
        private Transform m_EntityRoot;
        private AegisFlowContext m_Context;

        private void Start()
        {
            m_SceneBootstrap = gameObject.GetComponent<SceneBootstrap>();
            if (m_SceneBootstrap == null)
            {
                m_SceneBootstrap = gameObject.AddComponent<SceneBootstrap>();
            }

            m_EntityRoot = new GameObject("EntityRoot").transform;

            m_VisualRegistry = new EntityVisualRegistry(m_EntityRoot);

            GameObject bridgeObj = new GameObject("SimulationBridge");
            m_SimBridge = bridgeObj.AddComponent<SimulationBridge>();

            GameObject uiObj = new GameObject("UIRoot");
            m_UIRoot = uiObj.AddComponent<UIRoot>();

            Invoke(nameof(LateInitialize), 0.1f);
        }

        private void LateInitialize()
        {
            m_Context = m_SceneBootstrap?.Context;
            if (m_Context == null)
            {
                Debug.LogError("[DigitalTwin] AegisFlowContext not available.");
                return;
            }

            m_SimBridge.Initialize(m_Context, m_VisualRegistry, m_EntityRoot);
            m_SimBridge.Bind();
            m_SimBridge.SyncAllVisuals();

            m_UIRoot.Initialize(m_Context);

            Debug.Log("[DigitalTwin] Full system initialized.");
        }

        private void OnDestroy()
        {
            m_SimBridge?.Unbind();
            m_UIRoot?.Unbind();
            m_VisualRegistry?.Clear();
        }
    }
}
