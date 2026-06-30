using AegisFlow.Procedure;
using AegisFlow.Resource;
using AegisFlow.Save;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace AegisFlow.Bootstrap
{
    /// <summary>
    /// 玄盾流架构启动器。负责注册基础设施与启动首个 Procedure。
    /// </summary>
    public sealed class AppBootstrap
    {
        private ProcedureController m_ProcedureController;
        private ProcedureDependencies m_ProcedureDependencies;
        private AegisFlowContext m_Context;

        public AegisFlowContext Context => m_Context;

        public void Initialize()
        {
            Debug.Log("[AegisFlow] AppBootstrap Initialize.");

            ResourceComponent resourceComponent = UnityGameFramework.Runtime.GameEntry.GetComponent<ResourceComponent>();
            AssetService.Initialize(resourceComponent != null
                ? (IAssetLoader)new UGFResourceAdapter(resourceComponent)
                : new ResourcesLegacyAdapter());
            SaveAppService.Initialize(new LocalSaveRepository());

            m_Context = new AegisFlowContext();
            m_Context.Build();

            m_ProcedureController = new ProcedureController();
            m_ProcedureDependencies = new ProcedureDependencies(
                m_Context.SimulationDC,
                m_Context.PlayerDomainService,
                m_Context.ModelDomainService,
                m_Context.SimulationAppService,
                m_Context.SimulationModelAppService,
                m_Context.SimulationRuntimeController,
                m_Context.DomainEventBus);
            m_ProcedureController.ChangeProcedure(new ProcedureLaunch(m_ProcedureController, m_ProcedureDependencies));
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            m_ProcedureController?.Update(elapseSeconds, realElapseSeconds);
        }

        public void Shutdown()
        {
            m_ProcedureController?.Shutdown();
            m_ProcedureDependencies = null;
            m_Context?.Dispose();
            m_Context = null;

            SaveAppService.Shutdown();
            AssetService.Shutdown();

            Debug.Log("[AegisFlow] AppBootstrap Shutdown.");
        }
    }
}
