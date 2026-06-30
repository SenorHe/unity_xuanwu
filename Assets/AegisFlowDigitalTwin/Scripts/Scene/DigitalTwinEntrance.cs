using AegisFlow.Bootstrap;
using AegisFlowDigitalTwin.UI;
using AegisFlowDigitalTwin.Visual;
using AegisFlowDigitalTwin.Editor;
using UnityEngine;

namespace AegisFlowDigitalTwin.Scene
{
    /// <summary>
    /// 一键启动入口。在场景中放置此 MonoBehaviour 即可启动整个数字孪生系统。
    /// 注意: 不要同时挂载 AegisFlow.Bootstrap.GameEntry，此脚本会自行管理 AppBootstrap 生命周期。
    /// </summary>
    public sealed class DigitalTwinEntrance : MonoBehaviour
    {
        private SceneBootstrap m_SceneBootstrap;
        private UIRoot m_UIRoot;
        private SimulationBridge m_SimBridge;
        private EntityVisualRegistry m_VisualRegistry;
        private EditModeController m_EditMode;
        private NavMeshBaker m_NavMeshBaker;
        private Transform m_EntityRoot;
        private AegisFlowContext m_Context;
        private bool m_Initialized;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (m_Initialized) return;
            m_Initialized = true;

            m_SceneBootstrap = gameObject.GetComponent<SceneBootstrap>();
            if (m_SceneBootstrap == null)
            {
                m_SceneBootstrap = gameObject.AddComponent<SceneBootstrap>();
            }

            m_EntityRoot = new GameObject("EntityRoot").transform;

            m_VisualRegistry = new EntityVisualRegistry(m_EntityRoot);

            GameObject bridgeObj = new GameObject("SimulationBridge");
            m_SimBridge = bridgeObj.AddComponent<SimulationBridge>();

            m_NavMeshBaker = gameObject.AddComponent<NavMeshBaker>();

            GameObject editObj = new GameObject("EditModeController");
            m_EditMode = editObj.AddComponent<EditModeController>();

            GameObject uiObj = new GameObject("UIRoot");
            m_UIRoot = uiObj.AddComponent<UIRoot>();

            Invoke(nameof(LateInitialize), 0.2f);
        }

        private void LateInitialize()
        {
            m_Context = m_SceneBootstrap?.Context;
            if (m_Context == null)
            {
                Debug.LogError("[DigitalTwin] AegisFlowContext not available. Retrying...");
                Invoke(nameof(LateInitialize), 0.5f);
                return;
            }

            Camera mainCam = UnityEngine.Camera.main;
            if (mainCam == null)
            {
                GameObject camObj = new GameObject("MainCamera");
                camObj.tag = "MainCamera";
                mainCam = camObj.AddComponent<Camera>();
            }

            m_SimBridge.Initialize(m_Context, m_VisualRegistry, m_EntityRoot);
            m_SimBridge.Bind();

            m_EditMode.Initialize(
                mainCam,
                m_Context.UICommandDispatcher,
                m_Context.EntityRepository,
                m_Context.TwinDC);

            m_UIRoot.Initialize(m_Context, m_EditMode);

            m_SimBridge.SyncAllVisuals();

            m_NavMeshBaker.BuildNavMesh();

            Debug.Log("[DigitalTwin] Full system initialized with EditMode + NavMesh + UI.");
        }

        private void OnDestroy()
        {
            m_SimBridge?.Unbind();
            m_UIRoot?.Unbind();
            m_VisualRegistry?.Clear();
        }
    }
}
