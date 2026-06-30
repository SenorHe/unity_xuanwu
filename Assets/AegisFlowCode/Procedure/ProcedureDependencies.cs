using AegisFlow.Application;
using AegisFlow.Data;
using AegisFlow.Domain;
using AegisFlow.Event;
using AegisFlow.Runtime;

namespace AegisFlow.Procedure
{
    /// <summary>
    /// Procedure 所需服务的显式依赖集合，由 Bootstrap 组合根负责创建。
    /// </summary>
    public sealed class ProcedureDependencies
    {
        public SimulationDC SimulationDC { get; }
        public TwinDC TwinDC { get; }
        public PlayerDomainService PlayerDomainService { get; }
        public TwinDomainService TwinDomainService { get; }
        public ModelDomainService ModelDomainService { get; }
        public SimulationAppService SimulationAppService { get; }
        public SimulationModelAppService SimulationModelAppService { get; }
        public SimulationRuntimeController SimulationRuntimeController { get; }
        public DomainEventBus DomainEventBus { get; }

        public ProcedureDependencies(
            SimulationDC simulationDC,
            TwinDC twinDC,
            PlayerDomainService playerDomainService,
            TwinDomainService twinDomainService,
            ModelDomainService modelDomainService,
            SimulationAppService simulationAppService,
            SimulationModelAppService simulationModelAppService,
            SimulationRuntimeController simulationRuntimeController,
            DomainEventBus domainEventBus)
        {
            SimulationDC = simulationDC;
            TwinDC = twinDC;
            PlayerDomainService = playerDomainService;
            TwinDomainService = twinDomainService;
            ModelDomainService = modelDomainService;
            SimulationAppService = simulationAppService;
            SimulationModelAppService = simulationModelAppService;
            SimulationRuntimeController = simulationRuntimeController;
            DomainEventBus = domainEventBus;
        }
    }
}
