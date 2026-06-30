using AegisFlow.Data;
using AegisFlow.Domain;
using AegisFlow.Event;
using AegisFlow.Runtime;
using AegisFlow.Save;
using NUnit.Framework;

namespace AegisFlow.Tests
{
    /// <summary>
    /// 数字孪生集成测试。验证 EntityData 空间扩展、TwinDC、TwinDomainService、
    /// SaveData 往返含位置字段、AGV 事件策略含 Moving/Arrived。
    /// </summary>
    public sealed class DigitalTwinIntegrationTests
    {
        [Test]
        public void EntityData_StoresSpatialFields()
        {
            EntityData entity = new EntityData(
                "agv-1", "AGV", "Test AGV",
                1.5f, 0f, 3.2f, 90f, "AGV", "Placed");

            Assert.AreEqual(1.5f, entity.PosX);
            Assert.AreEqual(3.2f, entity.PosZ);
            Assert.AreEqual(90f, entity.RotY);
            Assert.AreEqual("AGV", entity.EntityType);
            Assert.AreEqual("Placed", entity.Status);
        }

        [Test]
        public void EntityData_ClonePreservesAllFields()
        {
            EntityData original = new EntityData(
                "rack-1", "RACK", "Rack A",
                5f, 0f, 10f, 0f, "RACK", "Idle");

            EntityData clone = original.Clone();

            Assert.AreEqual(original.EntityId, clone.EntityId);
            Assert.AreEqual(original.PosX, clone.PosX);
            Assert.AreEqual(original.PosZ, clone.PosZ);
            Assert.AreEqual(original.EntityType, clone.EntityType);
            Assert.AreEqual(original.Status, clone.Status);
        }

        [Test]
        public void EntityData_SetStatus_UpdatesValue()
        {
            EntityData entity = new EntityData("e1", "AGV", "AGV 1");

            entity.SetStatus("Moving");

            Assert.AreEqual("Moving", entity.Status);
        }

        [Test]
        public void TwinDC_UpdatesAndRetrievesTelemetry()
        {
            TwinDC twinDC = new TwinDC();

            twinDC.UpdateTelemetry("agv-1", 1f, 0f, 2f, 80f, "Moving", "rack-1", 3.5f);

            Assert.IsTrue(twinDC.TryGetTelemetry("agv-1", out EntityTelemetry telemetry));
            Assert.AreEqual(1f, telemetry.PosX);
            Assert.AreEqual(2f, telemetry.PosZ);
            Assert.AreEqual(80f, telemetry.Battery);
            Assert.AreEqual("Moving", telemetry.Status);
            Assert.AreEqual("rack-1", telemetry.TargetEntityId);
            Assert.AreEqual(3.5f, telemetry.Speed);
        }

        [Test]
        public void TwinDC_UpdatesStatus()
        {
            TwinDC twinDC = new TwinDC();
            twinDC.UpdateTelemetry("agv-1", 0f, 0f, 0f, 100f, "Idle", null, 0f);

            twinDC.UpdateStatus("agv-1", "Charging");

            Assert.IsTrue(twinDC.TryGetTelemetry("agv-1", out EntityTelemetry telemetry));
            Assert.AreEqual("Charging", telemetry.Status);
        }

        [Test]
        public void TwinDC_RemovesTelemetry()
        {
            TwinDC twinDC = new TwinDC();
            twinDC.UpdateTelemetry("agv-1", 0f, 0f, 0f, 100f, "Idle", null, 0f);

            twinDC.RemoveTelemetry("agv-1");

            Assert.IsFalse(twinDC.TryGetTelemetry("agv-1", out _));
        }

        [Test]
        public void TwinDomainService_CanPlace_ValidatesBounds()
        {
            TwinDomainService service = new TwinDomainService(new EntityRepository(), new ModelRepository());

            Assert.IsTrue(service.CanPlace("AGV", 5f, 5f));
            Assert.IsFalse(service.CanPlace("AGV", 200f, 5f));
            Assert.IsFalse(service.CanPlace(null, 5f, 5f));
        }

        [Test]
        public void TwinDomainService_MoveEntity_UpdatesPosition()
        {
            EntityRepository repo = new EntityRepository();
            EntityData entity = new EntityData("agv-1", "AGV", "AGV 1", 0f, 0f, 0f, 0f, "AGV", "Idle");
            repo.Add(entity);
            TwinDomainService service = new TwinDomainService(repo, new ModelRepository());

            bool result = service.MoveEntity("agv-1", 3f, 0f, 5f, 90f);

            Assert.IsTrue(result);
            Assert.AreEqual(3f, entity.PosX);
            Assert.AreEqual(5f, entity.PosZ);
            Assert.AreEqual(90f, entity.RotY);
        }

        [Test]
        public void TwinDomainService_GetEntity_ReturnsCorrectData()
        {
            EntityRepository repo = new EntityRepository();
            EntityData entity = new EntityData("agv-1", "AGV", "AGV 1", 1f, 0f, 2f, 0f, "AGV", "Idle");
            repo.Add(entity);
            TwinDomainService service = new TwinDomainService(repo, new ModelRepository());

            EntityData result = service.GetEntity("agv-1");

            Assert.IsNotNull(result);
            Assert.AreEqual("agv-1", result.EntityId);
        }

        [Test]
        public void TwinDomainService_HasEntityType_DetectsCorrectType()
        {
            EntityRepository repo = new EntityRepository();
            repo.Add(new EntityData("rack-1", "RACK", "Rack 1"));
            TwinDomainService service = new TwinDomainService(repo, new ModelRepository());

            Assert.IsTrue(service.HasEntityType("RACK"));
            Assert.IsFalse(service.HasEntityType("AGV"));
        }

        [Test]
        public void AgvRuntimeEventStrategy_GeneratesMovingAndArrivedEvents()
        {
            AgvRuntimeEventStrategy strategy = new AgvRuntimeEventStrategy();
            SimulationEventQueue queue = new SimulationEventQueue();
            EntityData agv = new EntityData("agv-1", "AGV", "Test AGV");

            strategy.Build(agv, 1, queue);

            Assert.IsNotNull(queue.Dequeue(1));
            Assert.IsNotNull(queue.Dequeue(2));
            Assert.IsNotNull(queue.Dequeue(3));
            Assert.IsNotNull(queue.Dequeue(5));
            Assert.IsNotNull(queue.Dequeue(6));
        }

        [Test]
        public void AgvRuntimeEventStrategy_OnlyHandlesAgv()
        {
            AgvRuntimeEventStrategy strategy = new AgvRuntimeEventStrategy();

            Assert.IsTrue(strategy.CanHandle(new EntityData("agv-1", "AGV", "AGV")));
            Assert.IsFalse(strategy.CanHandle(new EntityData("rack-1", "RACK", "Rack")));
        }

        [Test]
        public void SaveData_RoundTripsSpatialFields()
        {
            UnitySaveJsonAdapter adapter = new UnitySaveJsonAdapter();
            EntityData entity = new EntityData(
                "agv-1", "AGV", "AGV Alpha",
                5.5f, 0f, 12.3f, 45f, "AGV", "Placed");
            SimulationModelSaveData saveData = new SimulationModelSaveData("warehouse-1", false, new[] { entity });

            string json = adapter.ToJson(saveData);
            bool success = adapter.TryFromJson(json, out SimulationModelSaveData restored);

            Assert.IsTrue(success);
            Assert.AreEqual(1, restored.Entities.Count);
            Assert.AreEqual(5.5f, restored.Entities[0].PosX);
            Assert.AreEqual(12.3f, restored.Entities[0].PosZ);
            Assert.AreEqual(45f, restored.Entities[0].RotY);
            Assert.AreEqual("AGV", restored.Entities[0].EntityType);
            Assert.AreEqual("Placed", restored.Entities[0].Status);
        }

        [Test]
        public void EntityViewData_FromEntityData_MapsCorrectly()
        {
            EntityData entity = new EntityData(
                "agv-1", "AGV", "AGV Alpha",
                3f, 0f, 7f, 0f, "AGV", "Moving");

            EntityViewData viewData = EntityViewData.FromEntityData(entity, 75f);

            Assert.AreEqual("agv-1", viewData.EntityId);
            Assert.AreEqual("AGV", viewData.EntityType);
            Assert.AreEqual("Moving", viewData.Status);
            Assert.AreEqual(3f, viewData.PosX);
            Assert.AreEqual(7f, viewData.PosZ);
            Assert.AreEqual(75f, viewData.Battery);
        }

        [Test]
        public void MovingEntityHandler_ReturnsTrue()
        {
            MovingEntityEventHandler handler = new MovingEntityEventHandler();
            SimulationEvent evt = new SimulationEvent("Moving", 1, "agv-1");

            bool result = handler.Execute(evt);

            Assert.IsTrue(result);
            Assert.AreEqual("Moving", handler.EventType);
        }

        [Test]
        public void ArrivedEntityHandler_ReturnsTrue()
        {
            ArrivedEntityEventHandler handler = new ArrivedEntityEventHandler();
            SimulationEvent evt = new SimulationEvent("Arrived", 5, "agv-1");

            bool result = handler.Execute(evt);

            Assert.IsTrue(result);
            Assert.AreEqual("Arrived", handler.EventType);
        }

        [Test]
        public void EntityStatusChangedEvent_CarriesAllData()
        {
            EntityStatusChangedEvent evt = new EntityStatusChangedEvent(
                1.5f, "agv-1", "AGV", "Moving", 3f, 0f, 7f);

            Assert.AreEqual("agv-1", evt.EntityId);
            Assert.AreEqual("AGV", evt.EntityType);
            Assert.AreEqual("Moving", evt.Status);
            Assert.AreEqual(3f, evt.PosX);
            Assert.AreEqual(7f, evt.PosZ);
        }
    }
}
