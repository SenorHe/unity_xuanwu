using System.Collections.Generic;
using AegisFlow.Data;
using AegisFlow.Event;
using UnityEngine;

namespace AegisFlowDigitalTwin.UI
{
    /// <summary>
    /// UI 总根节点。管理所有面板的创建、绑定和生命周期。
    /// </summary>
    public sealed class UIRoot : MonoBehaviour
    {
        private AegisFlow.Bootstrap.AegisFlowContext m_Context;
        private DomainEventBus m_EventBus;
        private TwinDC m_TwinDC;
        private AegisFlow.Command.UICommandDispatcher m_Dispatcher;
        private EntityRepository m_EntityRepository;

        private Canvas m_Canvas;
        private ToolbarUI m_Toolbar;
        private EntityListPanel m_EntityList;
        private EntityDetailPanel m_DetailPanel;
        private RuntimeDashboardPanel m_RuntimePanel;
        private EventLogPanel m_EventLog;
        private RecoveryPanel m_RecoveryPanel;
        private ToastUI m_ToastUI;

        private readonly List<string> m_LogEntries = new List<string>();
        private const int m_MaxLogEntries = 50;

        public void Initialize(AegisFlow.Bootstrap.AegisFlowContext context)
        {
            m_Context = context;
            m_EventBus = context.DomainEventBus;
            m_TwinDC = context.TwinDC;
            m_Dispatcher = context.UICommandDispatcher;
            m_EntityRepository = context.EntityRepository;

            CreateCanvas();
            CreateToolbar();
            CreateEntityListPanel();
            CreateDetailPanel();
            CreateRuntimeDashboard();
            CreateEventLogPanel();
            CreateRecoveryPanel();
            CreateToastUI();
            BindEvents();
        }

        private void CreateCanvas()
        {
            GameObject canvasObj = new GameObject("UICanvas");
            canvasObj.transform.SetParent(transform);
            m_Canvas = canvasObj.AddComponent<Canvas>();
            m_Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            m_Canvas.sortingOrder = 10;

            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        private void CreateToolbar()
        {
            m_Toolbar = ToolbarUI.Create(m_Canvas.transform, m_Dispatcher);
        }

        private void CreateEntityListPanel()
        {
            m_EntityList = EntityListPanel.Create(m_Canvas.transform, m_EntityRepository);
        }

        private void CreateDetailPanel()
        {
            m_DetailPanel = EntityDetailPanel.Create(m_Canvas.transform);
        }

        private void CreateRuntimeDashboard()
        {
            m_RuntimePanel = RuntimeDashboardPanel.Create(m_Canvas.transform);
        }

        private void CreateEventLogPanel()
        {
            m_EventLog = EventLogPanel.Create(m_Canvas.transform);
        }

        private void CreateRecoveryPanel()
        {
            m_RecoveryPanel = RecoveryPanel.Create(m_Canvas.transform, m_Dispatcher);
        }

        private void CreateToastUI()
        {
            m_ToastUI = ToastUI.Create(m_Canvas.transform);
        }

        private void BindEvents()
        {
            m_EventBus.Subscribe<UICommandExecutedEvent>(OnCommandExecuted);
            m_EventBus.Subscribe<EntityStatusChangedEvent>(OnEntityStatusChanged);
            m_EventBus.Subscribe<AegisFlow.Event.RuntimeStepChangedEvent>(OnRuntimeStepChanged);
            m_EventBus.Subscribe<AegisFlow.Event.ProcedureRouteFailedEvent>(OnRouteFailed);
        }

        public void Unbind()
        {
            if (m_EventBus == null)
            {
                return;
            }

            m_EventBus.Unsubscribe<UICommandExecutedEvent>(OnCommandExecuted);
            m_EventBus.Unsubscribe<EntityStatusChangedEvent>(OnEntityStatusChanged);
            m_EventBus.Unsubscribe<AegisFlow.Event.RuntimeStepChangedEvent>(OnRuntimeStepChanged);
            m_EventBus.Unsubscribe<AegisFlow.Event.ProcedureRouteFailedEvent>(OnRouteFailed);
        }

        private void OnCommandExecuted(UICommandExecutedEvent evt)
        {
            string log = $"[{evt.CreatedTime:F1}] {evt.CommandId}: {(evt.IsSuccess ? "OK" : "FAIL")} - {evt.Message}";
            m_EventLog?.AddEntry(log, evt.IsSuccess);

            if (!evt.IsSuccess)
            {
                m_ToastUI?.ShowToast(evt.Message, Color.red);
            }
            else
            {
                m_ToastUI?.ShowToast(evt.Message, Color.green);
            }

            if (evt.CommandId == "PlaceEntity" || evt.CommandId == "MoveEntity")
            {
                m_EntityList?.Refresh();
            }

            if (evt.CommandId == "StartSimulation")
            {
                m_RuntimePanel?.SetRuntimeActive(true);
            }

            if (evt.CommandId == "StopSimulation")
            {
                m_RuntimePanel?.SetRuntimeActive(false);
            }
        }

        private void OnEntityStatusChanged(EntityStatusChangedEvent evt)
        {
            string log = $"[{evt.CreatedTime:F1}] {evt.EntityId} ({evt.EntityType}) -> {evt.Status} @ ({evt.PosX:F1}, {evt.PosZ:F1})";
            m_EventLog?.AddEntry(log, true);
            m_EntityList?.Refresh();
        }

        private void OnRuntimeStepChanged(AegisFlow.Event.RuntimeStepChangedEvent evt)
        {
            m_RuntimePanel?.UpdateStep(evt.Step);
        }

        private void OnRouteFailed(AegisFlow.Event.ProcedureRouteFailedEvent evt)
        {
            m_ToastUI?.ShowToast(evt.Message, Color.red);
        }

        private void OnDestroy()
        {
            Unbind();
        }
    }
}
