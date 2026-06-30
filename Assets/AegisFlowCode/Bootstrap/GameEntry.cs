using UnityEngine;

namespace AegisFlow.Bootstrap
{
    /// <summary>
    /// Unity 生命周期入口。只负责启动玄盾流架构，不承载业务逻辑。
    /// </summary>
    public sealed class GameEntry : MonoBehaviour
    {
        private static GameEntry m_Instance;

        public static GameEntry Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    Debug.LogError("[AegisFlow] GameEntry 尚未由 Unity 生命周期初始化，请确认场景中已挂载 GameEntry。");
                }

                return m_Instance;
            }
        }

        private AppBootstrap m_AppBootstrap;

        private void Awake()
        {
            if (m_Instance != null && m_Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            m_Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            m_AppBootstrap = new AppBootstrap();
            m_AppBootstrap.Initialize();
        }

        private void Update()
        {
            m_AppBootstrap?.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void OnDestroy()
        {
            m_AppBootstrap?.Shutdown();

            if (m_Instance == this)
            {
                m_Instance = null;
            }
        }
    }
}
