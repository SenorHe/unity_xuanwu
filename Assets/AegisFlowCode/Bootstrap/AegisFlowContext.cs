using AegisFlow.Application;
using AegisFlow.Command;
using AegisFlow.Data;
using AegisFlow.Domain;
using AegisFlow.Event;
using AegisFlow.Legacy;
using AegisFlow.Network;
using AegisFlow.Runtime;
using AegisFlow.Save;
using AegisFlow.UI;

namespace AegisFlow.Bootstrap
{
    /// <summary>
    /// 玄盾流组合根。只负责对象装配，不承载业务逻辑。
    /// </summary>
    public sealed class AegisFlowContext
    {
        public AccountDC AccountDC { get; private set; }
        public PlayerDC PlayerDC { get; private set; }
        public SimulationDC SimulationDC { get; private set; }
        public RuntimeDC RuntimeDC { get; private set; }
        public TwinDC TwinDC { get; private set; }
        public EntityRepository EntityRepository { get; private set; }
        public ModelRepository ModelRepository { get; private set; }
        public DomainEventBus DomainEventBus { get; private set; }

        public PlayerDomainService PlayerDomainService { get; private set; }
        public BattlePrepareDomainService BattlePrepareDomainService { get; private set; }
        public EntityDomainService EntityDomainService { get; private set; }
        public TwinDomainService TwinDomainService { get; private set; }
        public ModelDomainService ModelDomainService { get; private set; }
        public RuntimeSnapshotService RuntimeSnapshotService { get; private set; }
        public RuntimeEventStrategyConfigLoader RuntimeEventStrategyConfigLoader { get; private set; }
        public RuntimeEventStrategyConfig RuntimeEventStrategyConfig { get; private set; }
        public RuntimeEventQueueBuilder RuntimeEventQueueBuilder { get; private set; }

        public DataProcessorRegistry DataProcessorRegistry { get; private set; }
        public NetworkClient NetworkClient { get; private set; }
        public LoginReqService LoginReqService { get; private set; }

        public SimulationEventProcessor SimulationEventProcessor { get; private set; }
        public SimulationEventExecutionPolicy SimulationEventExecutionPolicy { get; private set; }
        public SimulationEventQueue SimulationEventQueue { get; private set; }
        public SimulationRuntimeController SimulationRuntimeController { get; private set; }
        public SimulationAppService SimulationAppService { get; private set; }
        public ISaveJsonAdapter SaveJsonAdapter { get; private set; }
        public SimulationModelAppService SimulationModelAppService { get; private set; }

        public UICommandDispatcher UICommandDispatcher { get; private set; }
        public UICommandBinder UICommandBinder { get; private set; }
        public UIFormLifecycleBinder UIFormLifecycleBinder { get; private set; }
        public ToastPresenter ToastPresenter { get; private set; }
        public ToastPolicy ToastPolicy { get; private set; }
        public ToastQueue ToastQueue { get; private set; }
        public PlayerPresenter PlayerPresenter { get; private set; }
        public RuntimePresenter RuntimePresenter { get; private set; }

        public LegacyGameAdapter LegacyGameAdapter { get; private set; }
        public LegacyPlayerStateAdapter LegacyPlayerStateAdapter { get; private set; }
        public LegacyMrCSaveAdapter LegacyMrCSaveAdapter { get; private set; }
        public LegacyEntityRepositoryAdapter LegacyEntityRepositoryAdapter { get; private set; }
        public LegacySaveMigrationService LegacySaveMigrationService { get; private set; }

        public void Build()
        {
            AccountDC = new AccountDC();
            PlayerDC = new PlayerDC();
            SimulationDC = new SimulationDC();
            RuntimeDC = new RuntimeDC();
            TwinDC = new TwinDC();
            EntityRepository = new EntityRepository();
            ModelRepository = new ModelRepository();
            DomainEventBus = new DomainEventBus();

            PlayerDomainService = new PlayerDomainService(PlayerDC);
            BattlePrepareDomainService = new BattlePrepareDomainService(PlayerDomainService);
            EntityDomainService = new EntityDomainService(EntityRepository);
            TwinDomainService = new TwinDomainService(EntityRepository, ModelRepository);
            ModelDomainService = new ModelDomainService(ModelRepository);
            RuntimeSnapshotService = new RuntimeSnapshotService(EntityRepository);
            RuntimeEventStrategyConfigLoader = new RuntimeEventStrategyConfigLoader();
            RuntimeEventStrategyConfig = RuntimeEventStrategyConfigLoader.LoadDefault();
            RuntimeEventQueueBuilder = new RuntimeEventQueueBuilder();
            RuntimeEventQueueBuilder.RegisterStrategy(new AgvRuntimeEventStrategy());
            RuntimeEventQueueBuilder.RegisterStrategy(new ConfigRuntimeEventStrategy(RuntimeEventStrategyConfig));

            DataProcessorRegistry = new DataProcessorRegistry();
            DataProcessorRegistry.Register(new LoginDataProcessor(AccountDC, DomainEventBus));
            DataProcessorRegistry.Register(new PlayerDataProcessor(PlayerDC, DomainEventBus));
            NetworkClient = new NetworkClient(DataProcessorRegistry);
            LoginReqService = new LoginReqService(NetworkClient);

            SimulationEventExecutionPolicy = new SimulationEventExecutionPolicy(
                0,
                SimulationEventFailureAction.PauseRuntime);
            SimulationEventProcessor = new SimulationEventProcessor(SimulationEventExecutionPolicy);
            SimulationEventProcessor.Register(new CreateEntityEventHandler());
            SimulationEventProcessor.Register(new ActivateEntityEventHandler());
            SimulationEventProcessor.Register(new CompleteEntityEventHandler());
            SimulationEventProcessor.Register(new MovingEntityEventHandler());
            SimulationEventProcessor.Register(new ArrivedEntityEventHandler());
            SimulationEventProcessor.Register(new PickupEntityEventHandler());
            SimulationEventProcessor.Register(new DropEntityEventHandler());
            SimulationEventProcessor.Register(new ChargeEntityEventHandler());
            SimulationEventProcessor.Register(new SensorScanEventHandler());
            SimulationEventQueue = new SimulationEventQueue();
            SimulationRuntimeController = new SimulationRuntimeController(RuntimeDC, SimulationEventProcessor, SimulationEventQueue, RuntimeEventQueueBuilder, DomainEventBus);
            SimulationAppService = new SimulationAppService(ModelDomainService, RuntimeSnapshotService, SimulationRuntimeController);
            SaveJsonAdapter = new UnitySaveJsonAdapter();
            SimulationModelAppService = new SimulationModelAppService(ModelRepository, EntityRepository, SimulationDC, SaveJsonAdapter);

            UICommandDispatcher = new UICommandDispatcher();
            UICommandBinder = new UICommandBinder(
                UICommandDispatcher,
                EntityDomainService,
                TwinDomainService,
                SimulationModelAppService,
                SimulationAppService,
                TwinDC,
                DomainEventBus);
            UICommandBinder.Bind();
            ToastPresenter = new ToastPresenter();
            ToastPolicy = new ToastPolicy(2.5f, true);
            ToastQueue = new ToastQueue(ToastPolicy);
            UIFormLifecycleBinder = new UIFormLifecycleBinder(DomainEventBus, ToastPresenter, ToastQueue, ToastPolicy);

            PlayerPresenter = new PlayerPresenter(PlayerDC, PlayerDomainService);
            RuntimePresenter = new RuntimePresenter(RuntimeDC);

            LegacyGameAdapter = new LegacyGameAdapter();
            LegacyPlayerStateAdapter = new LegacyPlayerStateAdapter();
            LegacyMrCSaveAdapter = new LegacyMrCSaveAdapter();
            LegacyEntityRepositoryAdapter = new LegacyEntityRepositoryAdapter();
            LegacySaveMigrationService = new LegacySaveMigrationService(LegacyEntityRepositoryAdapter);
        }

        public void Dispose()
        {
            UICommandBinder?.Unbind();
            DataProcessorRegistry?.Clear();
            SimulationEventProcessor?.Clear();
            SimulationEventQueue?.Clear();
            ToastQueue?.Clear();
            DomainEventBus?.Clear();
        }
    }
}
