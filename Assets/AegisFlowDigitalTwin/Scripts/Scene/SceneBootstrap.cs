using AegisFlow.Bootstrap;
using UnityEngine;
using UnityEngine.AI;

namespace AegisFlowDigitalTwin.Scene
{
    /// <summary>
    /// 数字孪生场景启动器。负责初始化 AegisFlow 架构并搭建仓储环境。
    /// </summary>
    /// <summary>
    /// 数字孪生场景启动器。负责初始化 AegisFlow 架构并搭建仓储环境。
    /// 注意: 如果场景中已有 DigitalTwinEntrance，不要同时挂载 AegisFlow.Bootstrap.GameEntry，
    /// 否则会导致 AppBootstrap 被初始化两次。
    /// </summary>
    public sealed class SceneBootstrap : MonoBehaviour
    {
        [SerializeField] private bool m_BuildEnvironmentOnStart = true;
        [SerializeField] private bool m_IsPrimaryBootstrap = true;

        private AppBootstrap m_AppBootstrap;
        private bool m_Initialized;

        public AegisFlowContext Context => m_AppBootstrap?.Context;

        private void Start()
        {
            if (m_Initialized || !m_IsPrimaryBootstrap)
            {
                return;
            }

            m_Initialized = true;

            m_AppBootstrap = new AppBootstrap();
            m_AppBootstrap.Initialize();

            if (m_BuildEnvironmentOnStart)
            {
                BuildEnvironment();
                SetupCamera();
                SetupLighting();
            }

            Debug.Log("[DigitalTwin] SceneBootstrap complete.");
        }

        private void Update()
        {
            m_AppBootstrap?.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void OnDestroy()
        {
            m_AppBootstrap?.Shutdown();
        }

        private void BuildEnvironment()
        {
            GameObject envRoot = new GameObject("WarehouseEnvironment");
            Environment.WarehouseBuilder builder = envRoot.AddComponent<Environment.WarehouseBuilder>();
        }

        private void SetupCamera()
        {
            GameObject camObj = new GameObject("TwinCamera");
            UnityEngine.Camera cam = camObj.AddComponent<UnityEngine.Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.12f, 0.14f, 0.16f, 1f);
            cam.fieldOfView = 60f;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 500f;

            Camera.TwinOrbitCamera orbit = camObj.AddComponent<Camera.TwinOrbitCamera>();
            camObj.transform.position = new Vector3(0f, 30f, -30f);

            AudioListener listener = camObj.AddComponent<AudioListener>();
        }

        private void SetupLighting()
        {
            GameObject dirLight = new GameObject("DirectionalLight");
            Light light = dirLight.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.96f, 0.88f, 1f);
            light.intensity = 1.2f;
            light.shadows = LightShadows.Soft;
            dirLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            GameObject ambient = new GameObject("AmbientLight");
            Light ambLight = ambient.AddComponent<Light>();
            ambLight.type = LightType.Point;
            ambLight.color = new Color(0.4f, 0.5f, 0.7f, 1f);
            ambLight.intensity = 0.5f;
            ambLight.range = 60f;
            ambient.transform.position = new Vector3(0f, 20f, 0f);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.2f, 0.22f, 0.28f, 1f);
        }
    }
}
