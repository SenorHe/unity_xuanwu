using System.Collections.Generic;
using AegisFlow.Bootstrap;
using AegisFlow.Data;
using AegisFlow.Event;
using AegisFlowDigitalTwin.Visual;
using UnityEngine;

namespace AegisFlowDigitalTwin.Simulation
{
    /// <summary>
    /// Runtime 与 Unity 视觉层的桥接器。
    /// 监听 DomainEvent，驱动 EntityVisual 和 AgvNavigator。
    /// </summary>
    public sealed class SimulationBridge : MonoBehaviour
    {
        private AegisFlowContext m_Context;
        private EntityVisualRegistry m_VisualRegistry;
        private Transform m_EntityRoot;
        private DomainEventBus m_EventBus;
        private TwinDC m_TwinDC;
        private EntityRepository m_EntityRepository;
        private bool m_IsBound;

        private readonly Dictionary<string, AgvNavigator> m_Navigators = new Dictionary<string, AgvNavigator>();
        private readonly HashSet<string> m_ArrivedAgvs = new HashSet<string>();

        public void Initialize(AegisFlowContext context, EntityVisualRegistry visualRegistry, Transform entityRoot)
        {
            m_Context = context;
            m_VisualRegistry = visualRegistry;
            m_EntityRoot = entityRoot;
            m_EventBus = context.DomainEventBus;
            m_TwinDC = context.TwinDC;
            m_EntityRepository = context.EntityRepository;
        }

        public void Bind()
        {
            if (m_IsBound || m_EventBus == null)
            {
                return;
            }

            m_EventBus.Subscribe<EntityStatusChangedEvent>(OnEntityStatusChanged);
            m_EventBus.Subscribe<SimulationEventExecutedEvent>(OnSimulationEventExecuted);
            m_IsBound = true;
        }

        public void Unbind()
        {
            if (!m_IsBound || m_EventBus == null)
            {
                return;
            }

            m_EventBus.Unsubscribe<EntityStatusChangedEvent>(OnEntityStatusChanged);
            m_EventBus.Unsubscribe<SimulationEventExecutedEvent>(OnSimulationEventExecuted);
            m_IsBound = false;
        }

        public void SyncAllVisuals()
        {
            if (m_EntityRepository == null || m_VisualRegistry == null)
            {
                return;
            }

            foreach (EntityData entity in m_EntityRepository.GetAll())
            {
                CreateVisualForEntity(entity);
            }
        }

        public void ClearAllVisuals()
        {
            m_VisualRegistry?.Clear();
            m_Navigators.Clear();
            m_ArrivedAgvs.Clear();
        }

        private void CreateVisualForEntity(EntityData entity)
        {
            if (m_VisualRegistry.TryGetVisual(entity.EntityId, out _))
            {
                return;
            }

            EntityVisualBase visual = EntityVisualFactory.Create(entity.EntityId, entity.EntityType, m_EntityRoot);
            visual.SetPosition(entity.PosX, entity.PosY, entity.PosZ);
            visual.SetRotationY(entity.RotY);
            m_VisualRegistry.Register(entity.EntityId, visual);

            if (entity.EntityType == "AGV")
            {
                AgvNavigator navigator = visual.gameObject.AddComponent<AgvNavigator>();
                m_Navigators[entity.EntityId] = navigator;
            }

            m_TwinDC.UpdateTelemetry(
                entity.EntityId,
                entity.PosX, entity.PosY, entity.PosZ,
                100f, entity.Status, null, 0f);
        }

        private void OnEntityStatusChanged(EntityStatusChangedEvent evt)
        {
            if (m_VisualRegistry.TryGetVisual(evt.EntityId, out EntityVisualBase visual))
            {
                visual.SetStatus(evt.Status);
                visual.SetPosition(evt.PosX, evt.PosY, evt.PosZ);
            }
        }

        private void OnSimulationEventExecuted(SimulationEventExecutedEvent evt)
        {
            if (evt.Status != SimulationEventExecutionStatus.Succeeded || string.IsNullOrEmpty(evt.EntityId))
            {
                return;
            }

            switch (evt.EventType)
            {
                case "Moving":
                    HandleMovingEvent(evt.EntityId);
                    break;
                case "Arrived":
                    HandleArrivedEvent(evt.EntityId);
                    break;
                case "Charging":
                    HandleChargingEvent(evt.EntityId);
                    break;
            }
        }

        private void HandleMovingEvent(string entityId)
        {
            if (!m_Navigators.TryGetValue(entityId, out AgvNavigator navigator))
            {
                return;
            }

            EntityData source = m_EntityRepository.Get(entityId);
            if (source == null)
            {
                return;
            }

            EntityData target = FindNearestEntity(entityId, "RACK");
            if (target == null)
            {
                target = FindNearestEntity(entityId, "WORKSTATION");
            }
            if (target == null)
            {
                target = FindNearestEntity(entityId, "CHARGER");
            }

            if (target != null)
            {
                Vector3 dest = new Vector3(target.PosX, 0f, target.PosZ);
                navigator.MoveTo(dest, target.EntityId);
                m_TwinDC.UpdateStatus(entityId, "Moving");
            }
            else
            {
                Vector3 wander = new Vector3(
                    source.PosX + Random.Range(-6f, 6f),
                    0f,
                    source.PosZ + Random.Range(-6f, 6f));
                wander.x = Mathf.Clamp(wander.x, -25f, 25f);
                wander.z = Mathf.Clamp(wander.z, -25f, 25f);
                navigator.MoveTo(wander, null);
                m_TwinDC.UpdateStatus(entityId, "Moving");
            }
        }

        private void HandleArrivedEvent(string entityId)
        {
            if (!m_Navigators.TryGetValue(entityId, out AgvNavigator navigator))
            {
                return;
            }

            navigator.Stop();
            m_TwinDC.UpdateStatus(entityId, "Arrived");
            m_ArrivedAgvs.Add(entityId);
        }

        private void HandleChargingEvent(string entityId)
        {
            if (m_TwinDC.TryGetTelemetry(entityId, out EntityTelemetry telemetry))
            {
                m_TwinDC.UpdateTelemetry(
                    entityId,
                    telemetry.PosX, telemetry.PosY, telemetry.PosZ,
                    100f, "Charging", null, 0f);
            }
        }

        private EntityData FindNearestEntity(string sourceEntityId, string entityType)
        {
            EntityData source = m_EntityRepository.Get(sourceEntityId);
            if (source == null)
            {
                return null;
            }

            Vector3 sourcePos = new Vector3(source.PosX, source.PosY, source.PosZ);
            EntityData nearest = null;
            float nearestDist = float.MaxValue;

            foreach (EntityData entity in m_EntityRepository.GetAll())
            {
                if (entity.EntityId == sourceEntityId || entity.EntityType != entityType)
                {
                    continue;
                }

                float dist = Vector3.Distance(sourcePos, new Vector3(entity.PosX, entity.PosY, entity.PosZ));
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = entity;
                }
            }

            return nearest;
        }

        private void Update()
        {
            UpdateNavigatorTelemetry();
        }

        private void UpdateNavigatorTelemetry()
        {
            if (m_Navigators.Count == 0)
            {
                return;
            }

            foreach (var pair in m_Navigators)
            {
                string entityId = pair.Key;
                AgvNavigator navigator = pair.Value;

                if (navigator == null)
                {
                    continue;
                }

                if (m_TwinDC.TryGetTelemetry(entityId, out EntityTelemetry telemetry))
                {
                    Vector3 pos = navigator.transform.position;
                    float battery = telemetry.Battery;

                    if (navigator.IsMoving)
                    {
                        battery = Mathf.Max(0f, battery - Time.deltaTime * 0.5f);
                    }

                    string status = navigator.HasArrived ? "Arrived" : (navigator.IsMoving ? "Moving" : telemetry.Status);

                    m_TwinDC.UpdateTelemetry(
                        entityId,
                        pos.x, pos.y, pos.z,
                        battery,
                        status,
                        navigator.GetTargetEntityId(),
                        navigator.Speed);
                }
            }
        }
    }
}
