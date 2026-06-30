using System;
using System.IO;
using AegisFlow.Application;
using AegisFlow.Command;
using AegisFlow.Data;
using AegisFlow.Domain;
using AegisFlow.Event;
using AegisFlow.Runtime;
using AegisFlow.Save;
using AegisFlow.UI;
using NUnit.Framework;

namespace AegisFlow.Tests
{
    public sealed class RuntimeBehaviorTests
    {
        [Test]
        public void Scheduler_DoesNotAdvanceBeforeFixedStep()
        {
            RuntimeDC runtimeDC = new RuntimeDC();
            SimulationStepScheduler scheduler = CreateScheduler(runtimeDC, 0.1f, 8);
            runtimeDC.StartRuntime("model-1");

            scheduler.Tick(0.09f);

            Assert.AreEqual(0, runtimeDC.CurrentStep);
        }

        [Test]
        public void Scheduler_AdvancesUsingAccumulatedTime()
        {
            RuntimeDC runtimeDC = new RuntimeDC();
            SimulationStepScheduler scheduler = CreateScheduler(runtimeDC, 0.1f, 8);
            runtimeDC.StartRuntime("model-1");

            scheduler.Tick(0.06f);
            scheduler.Tick(0.06f);

            Assert.AreEqual(1, runtimeDC.CurrentStep);
        }

        [Test]
        public void Scheduler_CapsCatchUpSteps()
        {
            RuntimeDC runtimeDC = new RuntimeDC();
            SimulationStepScheduler scheduler = CreateScheduler(runtimeDC, 0.1f, 3);
            runtimeDC.StartRuntime("model-1");

            scheduler.Tick(10f);

            Assert.AreEqual(3, runtimeDC.CurrentStep);
        }

        [Test]
        public void Scheduler_ResetDiscardsPartialStep()
        {
            RuntimeDC runtimeDC = new RuntimeDC();
            SimulationStepScheduler scheduler = CreateScheduler(runtimeDC, 0.1f, 8);
            runtimeDC.StartRuntime("model-1");
            scheduler.Tick(0.09f);

            scheduler.Reset();
            scheduler.Tick(0.02f);

            Assert.AreEqual(0, runtimeDC.CurrentStep);
        }

        [Test]
        public void EventProcessor_RetriesThenSucceeds()
        {
            SimulationEventProcessor processor = new SimulationEventProcessor(
                new SimulationEventExecutionPolicy(1, SimulationEventFailureAction.PauseRuntime));
            CountingHandler handler = new CountingHandler("Retry", 2, false);
            processor.Register(handler);

            SimulationEventExecutionResult result = processor.ExecuteEvent(
                new SimulationEvent("Retry", 1, "entity-1"));

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.AttemptCount);
            Assert.AreEqual(2, handler.ExecutionCount);
        }

        [Test]
        public void EventProcessor_CapturesHandlerException()
        {
            SimulationEventProcessor processor = new SimulationEventProcessor(
                new SimulationEventExecutionPolicy(1, SimulationEventFailureAction.Continue));
            processor.Register(new CountingHandler("Throws", int.MaxValue, true));

            SimulationEventExecutionResult result = null;
            Assert.DoesNotThrow(() => result = processor.ExecuteEvent(
                new SimulationEvent("Throws", 1, "entity-1")));

            Assert.AreEqual(SimulationEventExecutionStatus.HandlerException, result.Status);
            Assert.AreEqual(2, result.AttemptCount);
            Assert.AreEqual(SimulationEventFailureAction.Continue, result.FailureAction);
            StringAssert.Contains("InvalidOperationException", result.ErrorMessage);
        }

        [Test]
        public void Scheduler_PublishesFailureAndPausesRuntime()
        {
            RuntimeDC runtimeDC = new RuntimeDC();
            DomainEventBus eventBus = new DomainEventBus();
            SimulationEventQueue eventQueue = new SimulationEventQueue();
            SimulationEventProcessor processor = new SimulationEventProcessor(
                new SimulationEventExecutionPolicy(0, SimulationEventFailureAction.PauseRuntime));
            processor.Register(new CountingHandler("Fails", int.MaxValue, false));
            eventQueue.Enqueue(new SimulationEvent("Fails", 1, "entity-1"));
            SimulationEventExecutedEvent observedEvent = null;
            eventBus.Subscribe<SimulationEventExecutedEvent>(eventData => observedEvent = eventData);
            SimulationStepScheduler scheduler = new SimulationStepScheduler(
                runtimeDC,
                processor,
                eventQueue,
                eventBus,
                0.1f,
                8);
            runtimeDC.StartRuntime("model-1");

            scheduler.Tick(0.1f);

            Assert.IsFalse(runtimeDC.IsPlaying);
            Assert.IsNotNull(observedEvent);
            Assert.AreEqual(SimulationEventExecutionStatus.HandlerReturnedFailure, observedEvent.Status);
            Assert.AreEqual("entity-1", observedEvent.EntityId);
            Assert.GreaterOrEqual(observedEvent.DurationMilliseconds, 0d);
        }

        [Test]
        public void Scheduler_IsolatesEventSubscriberException()
        {
            RuntimeDC runtimeDC = new RuntimeDC();
            DomainEventBus eventBus = new DomainEventBus();
            eventBus.Subscribe<RuntimeStepChangedEvent>(_ => throw new InvalidOperationException("Subscriber failure"));
            SimulationStepScheduler scheduler = new SimulationStepScheduler(
                runtimeDC,
                new SimulationEventProcessor(),
                new SimulationEventQueue(),
                eventBus,
                0.1f,
                8);
            runtimeDC.StartRuntime("model-1");

            Assert.DoesNotThrow(() => scheduler.Tick(0.1f));
            Assert.IsTrue(runtimeDC.IsPlaying);
            Assert.AreEqual(1, runtimeDC.CurrentStep);
        }

        [Test]
        public void Scheduler_RetriesFailedEventWithoutAdvancingStepAgain()
        {
            RuntimeDC runtimeDC = new RuntimeDC();
            DomainEventBus eventBus = new DomainEventBus();
            SimulationEventQueue eventQueue = new SimulationEventQueue();
            SimulationEventProcessor processor = new SimulationEventProcessor(
                new SimulationEventExecutionPolicy(0, SimulationEventFailureAction.PauseRuntime));
            CountingHandler handler = new CountingHandler("RetryLater", 2, false);
            processor.Register(handler);
            eventQueue.Enqueue(new SimulationEvent("RetryLater", 1, "entity-1"));
            SimulationRecoveryEvent recoveryEvent = null;
            eventBus.Subscribe<SimulationRecoveryEvent>(eventData => recoveryEvent = eventData);
            SimulationStepScheduler scheduler = new SimulationStepScheduler(
                runtimeDC,
                processor,
                eventQueue,
                eventBus,
                0.1f,
                8);
            runtimeDC.StartRuntime("model-1");
            scheduler.Tick(0.1f);

            bool isRecovered = scheduler.RetryFailedEvent();

            Assert.IsTrue(isRecovered);
            Assert.IsTrue(runtimeDC.IsPlaying);
            Assert.AreEqual(1, runtimeDC.CurrentStep);
            Assert.AreEqual(2, handler.ExecutionCount);
            Assert.IsFalse(scheduler.HasPendingFailure);
            Assert.AreEqual(SimulationRecoveryAction.Retry, recoveryEvent.Action);
            Assert.IsTrue(recoveryEvent.IsSuccess);
        }

        [Test]
        public void Scheduler_SkipsFailureThenExecutesRemainingStepEvents()
        {
            RuntimeDC runtimeDC = new RuntimeDC();
            SimulationEventQueue eventQueue = new SimulationEventQueue();
            SimulationEventProcessor processor = new SimulationEventProcessor(
                new SimulationEventExecutionPolicy(0, SimulationEventFailureAction.PauseRuntime));
            CountingHandler failedHandler = new CountingHandler("Fails", int.MaxValue, false);
            CountingHandler remainingHandler = new CountingHandler("Remaining", 1, false);
            processor.Register(failedHandler);
            processor.Register(remainingHandler);
            eventQueue.Enqueue(new SimulationEvent("Fails", 1, "entity-1"));
            eventQueue.Enqueue(new SimulationEvent("Remaining", 1, "entity-2"));
            SimulationStepScheduler scheduler = new SimulationStepScheduler(
                runtimeDC,
                processor,
                eventQueue,
                new DomainEventBus(),
                0.1f,
                8);
            runtimeDC.StartRuntime("model-1");
            scheduler.Tick(0.1f);

            bool isRecovered = scheduler.SkipFailedEvent();

            Assert.IsTrue(isRecovered);
            Assert.IsTrue(runtimeDC.IsPlaying);
            Assert.AreEqual(1, runtimeDC.CurrentStep);
            Assert.AreEqual(1, failedHandler.ExecutionCount);
            Assert.AreEqual(1, remainingHandler.ExecutionCount);
        }

        [Test]
        public void TerminateRecoveryCommand_StopsFailedRuntimeAndPublishesResult()
        {
            RuntimeDC runtimeDC = new RuntimeDC();
            DomainEventBus eventBus = new DomainEventBus();
            SimulationEventQueue eventQueue = new SimulationEventQueue();
            SimulationEventProcessor processor = new SimulationEventProcessor(
                new SimulationEventExecutionPolicy(0, SimulationEventFailureAction.PauseRuntime));
            processor.Register(new CountingHandler("Creating", int.MaxValue, false));
            SimulationRuntimeController runtimeController = new SimulationRuntimeController(
                runtimeDC,
                processor,
                eventQueue,
                new RuntimeEventQueueBuilder(),
                eventBus);
            RuntimeSnapshot snapshot = new RuntimeSnapshot("model-1");
            snapshot.AddEntity(new EntityData("entity-1", "AGV", "Entity"));
            runtimeController.Load(snapshot);
            runtimeController.Tick(1f);
            Assert.IsTrue(runtimeController.HasPendingFailure);

            ModelRepository modelRepository = new ModelRepository();
            EntityRepository entityRepository = new EntityRepository();
            SimulationAppService simulationAppService = new SimulationAppService(
                new ModelDomainService(modelRepository),
                new RuntimeSnapshotService(entityRepository),
                runtimeController);
            UICommandDispatcher dispatcher = new UICommandDispatcher();
            UICommandBinder binder = new UICommandBinder(
                dispatcher,
                new EntityDomainService(entityRepository),
                new TwinDomainService(entityRepository, modelRepository),
                new SimulationModelAppService(
                    modelRepository,
                    entityRepository,
                    new SimulationDC(),
                    new UnitySaveJsonAdapter()),
                simulationAppService,
                new TwinDC(),
                eventBus);
            UICommandExecutedEvent commandResult = null;
            eventBus.Subscribe<UICommandExecutedEvent>(eventData => commandResult = eventData);
            binder.Bind();

            dispatcher.Dispatch(new TerminateSimulationRuntimeCommand());
            binder.Unbind();

            Assert.IsNotNull(commandResult);
            Assert.IsTrue(commandResult.IsSuccess);
            Assert.AreEqual("TerminateSimulationRuntime", commandResult.CommandId);
            Assert.IsNull(runtimeDC.RunningModelId);
            Assert.IsFalse(runtimeController.HasPendingFailure);
        }

        [Test]
        public void ToastStateMachine_CompletesVisibleLifecycle()
        {
            ToastAnimationStateMachine stateMachine = new ToastAnimationStateMachine();

            stateMachine.BeginShow();
            stateMachine.MarkVisible();
            stateMachine.BeginHide();
            stateMachine.MarkHidden();

            Assert.AreEqual(ToastAnimationState.Hidden, stateMachine.State);
        }

        [Test]
        public void SaveJsonAdapter_RoundTripsEscapedValues()
        {
            UnitySaveJsonAdapter adapter = new UnitySaveJsonAdapter();
            EntityData sourceEntity = new EntityData(
                "entity-1", "AGV", "Line \"A\"\\North\nFloor",
                1.5f, 0f, 3.2f, 90f, "AGV", "Placed");
            SimulationModelSaveData source = new SimulationModelSaveData(
                "model-1",
                false,
                new[] { sourceEntity });

            string json = adapter.ToJson(source);
            bool success = adapter.TryFromJson(json, out SimulationModelSaveData restored);

            Assert.IsTrue(success);
            Assert.AreEqual(SimulationModelSaveData.CurrentSchemaVersion, restored.SchemaVersion);
            Assert.AreEqual(source.Entities[0].DisplayName, restored.Entities[0].DisplayName);
            Assert.AreEqual(source.Entities[0].PosX, restored.Entities[0].PosX);
            Assert.AreEqual(source.Entities[0].PosZ, restored.Entities[0].PosZ);
            Assert.AreEqual(source.Entities[0].EntityType, restored.Entities[0].EntityType);
        }

        [Test]
        public void SaveJsonAdapter_RejectsFutureSchema()
        {
            UnitySaveJsonAdapter adapter = new UnitySaveJsonAdapter();

            bool success = adapter.TryFromJson(
                "{\"schemaVersion\":999,\"modelId\":\"model-1\",\"entities\":[]}",
                out _);

            Assert.IsFalse(success);
        }

        [Test]
        public void LocalSaveRepository_RejectsUnsafeKey()
        {
            string rootPath = CreateTempDirectory();

            try
            {
                LocalSaveRepository repository = new LocalSaveRepository(rootPath);
                Assert.IsFalse(repository.Save("../outside", "{}"));
            }
            finally
            {
                DeleteDirectory(rootPath);
            }
        }

        [Test]
        public void ModelService_LoadsBackupWhenPrimaryIsCorrupt()
        {
            string rootPath = CreateTempDirectory();

            try
            {
                const string saveKey = "simulation_model-1";
                UnitySaveJsonAdapter adapter = new UnitySaveJsonAdapter();
                LocalSaveRepository saveRepository = new LocalSaveRepository(rootPath);
                SimulationModelSaveData firstSave = new SimulationModelSaveData(
                    "model-1",
                    false,
                    new[] { new EntityData("entity-from-backup", "AGV", "Backup") });
                SimulationModelSaveData secondSave = new SimulationModelSaveData(
                    "model-1",
                    false,
                    new[] { new EntityData("entity-from-primary", "AGV", "Primary") });

                Assert.IsTrue(saveRepository.Save(saveKey, adapter.ToJson(firstSave)));
                Assert.IsTrue(saveRepository.Save(saveKey, adapter.ToJson(secondSave)));
                File.WriteAllText(Path.Combine(rootPath, saveKey + ".json"), "{broken-json");

                EntityRepository entityRepository = new EntityRepository();
                SaveAppService.Initialize(saveRepository);
                SimulationModelAppService service = new SimulationModelAppService(
                    new ModelRepository(),
                    entityRepository,
                    new SimulationDC(),
                    adapter);

                Assert.IsTrue(service.LoadModel("model-1"));
                Assert.IsTrue(entityRepository.Exists("entity-from-backup"));
                Assert.IsFalse(entityRepository.Exists("entity-from-primary"));
            }
            finally
            {
                SaveAppService.Shutdown();
                DeleteDirectory(rootPath);
            }
        }

        private static SimulationStepScheduler CreateScheduler(RuntimeDC runtimeDC, float stepSeconds, int maxStepsPerTick)
        {
            return new SimulationStepScheduler(
                runtimeDC,
                new SimulationEventProcessor(),
                new SimulationEventQueue(),
                new DomainEventBus(),
                stepSeconds,
                maxStepsPerTick);
        }

        private static string CreateTempDirectory()
        {
            string path = Path.Combine(Path.GetTempPath(), "AegisFlowTests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(path);
            return path;
        }

        private static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        private sealed class CountingHandler : ISimulationEventHandler
        {
            private readonly int m_SucceedOnAttempt;
            private readonly bool m_ThrowException;

            public string EventType { get; }
            public int ExecutionCount { get; private set; }

            public CountingHandler(string eventType, int succeedOnAttempt, bool throwException)
            {
                EventType = eventType;
                m_SucceedOnAttempt = succeedOnAttempt;
                m_ThrowException = throwException;
            }

            public bool Execute(SimulationEvent simulationEvent)
            {
                ExecutionCount++;

                if (m_ThrowException)
                {
                    throw new InvalidOperationException("Handler failure");
                }

                return ExecutionCount >= m_SucceedOnAttempt;
            }
        }
    }
}
