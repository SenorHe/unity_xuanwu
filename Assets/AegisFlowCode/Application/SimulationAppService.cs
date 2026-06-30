using AegisFlow.Data;
using AegisFlow.Domain;
using AegisFlow.Runtime;

namespace AegisFlow.Application
{
    /// <summary>
    /// 仿真用例编排服务。负责协调领域层与运行态，不直接操作 UI。
    /// </summary>
    public sealed class SimulationAppService
    {
        private readonly ModelDomainService m_ModelDomainService;
        private readonly RuntimeSnapshotService m_RuntimeSnapshotService;
        private readonly SimulationRuntimeController m_RuntimeController;

        public SimulationAppService(
            ModelDomainService modelDomainService,
            RuntimeSnapshotService runtimeSnapshotService,
            SimulationRuntimeController runtimeController)
        {
            m_ModelDomainService = modelDomainService;
            m_RuntimeSnapshotService = runtimeSnapshotService;
            m_RuntimeController = runtimeController;
        }

        public bool StartRuntime(string modelId)
        {
            if (!m_ModelDomainService.CanRun(modelId))
            {
                return false;
            }

            RuntimeSnapshot snapshot = m_RuntimeSnapshotService.BuildSnapshot(modelId);
            m_RuntimeController.Load(snapshot);
            return true;
        }

        public void StopRuntime()
        {
            m_RuntimeController.Stop();
        }

        public bool RetryFailedEvent()
        {
            return m_RuntimeController.RetryFailedEvent();
        }

        public bool SkipFailedEvent()
        {
            return m_RuntimeController.SkipFailedEvent();
        }

        public bool TerminateFailedRuntime()
        {
            return m_RuntimeController.TerminateFailedRuntime();
        }
    }
}
