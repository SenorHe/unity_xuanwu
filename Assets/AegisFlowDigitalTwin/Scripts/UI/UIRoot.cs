using AegisFlow.Command;
using AegisFlow.Data;
using AegisFlow.Event;
using AegisFlow.Bootstrap;
using UnityEngine;
using AegisFlowDigitalTwin.Editor;
using AegisFlowDigitalTwin.Visual;

namespace AegisFlowDigitalTwin.UI
{
    /// <summary>
    /// UI 总根节点。管理所有面板的创建、绑定和生命周期。
    /// 订阅 DomainEvent 驱动 UI 刷新，连接 EditModeController 选择事件。
    /// </summary>
    public sealed class UIRoot : MonoBehaviour
    {
        private AegisFlowContext m_Context;
        private DomainEventBus m_EventBus;
        private TwinDC m_TwinDC;
        private UICommandDispatcher m_Dispatcher;
        private EntityRepository m_EntityRepository;
        private SimulationDC m_SimDC;
        private EditModeController m_EditMode;

        private Canvas m_Canvas;
        private ToolbarUI m_Toolbar;
        private EntityListPanel m_EntityList;
        private EntityDetailPanel m_DetailPanel;
        private RuntimeDashboardPanel m_RuntimePanel;
        private EventLogPanel m_EventLog;
        private RecoveryPanel m_RecoveryPanel;
        private ToastUI m_ToastUI;

        public void Initialize(AegisFlowContext context, EditModeController editMode)
        {
            m_Context = context;
            m_EventBus = context.DomainEventBus;
            m_TwinDC = context.TwinDC;
            m_Dispatcher = context.UICommandDispatcher;
            m_EntityRepository = context.EntityRepository;
            m_SimDC = context.SimulationDC;
            m_EditMode = editMode;

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
            m_Toolbar = ToolbarUI.Create(m_Canvas.transform, m_Dispatcher, m_EditMode, m_SimDC);
        }

        private void CreateEntityListPanel()
        {
            m_EntityList = EntityListPanel.Create(m_Canvas.transform, m_EntityRepository);
        }

        private void CreateDetailPanel()
        {
            m_DetailPanel = EntityDetailPanel.Create(m_Canvas.transform);
            m_DetailPanel.Clear();
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
            m_EventBus.Subscribe<RuntimeStepChangedEvent>(OnRuntimeStepChanged);
            m_EventBus.Subscribe<ProcedureRouteFailedEvent>(OnRouteFailed);
            m_EventBus.Subscribe<SimulationEventExecutedEvent>(OnSimulationEventExecuted);

            if (m_EditMode != null)
            {
                m_EditMode.OnEntitySelected += OnEditModeEntitySelected;
                m_EditMode.OnEntityDeselected += OnEditModeEntityDeselected;
                m_EditMode.OnEntityDeleted += OnEditModeEntityDeleted;
                m_EditMode.OnStateChanged += OnEditModeStateChanged;
            }
        }

        public void Unbind()
        {
            if (m_EventBus != null)
            {
                m_EventBus.Unsubscribe<UICommandExecutedEvent>(OnCommandExecuted);
                m_EventBus.Unsubscribe<EntityStatusChangedEvent>(OnEntityStatusChanged);
                m_EventBus.Unsubscribe<RuntimeStepChangedEvent>(OnRuntimeStepChanged);
                m_EventBus.Unsubscribe<ProcedureRouteFailedEvent>(OnRouteFailed);
                m_EventBus.Unsubscribe<SimulationEventExecutedEvent>(OnSimulationEventExecuted);
            }

            if (m_EditMode != null)
            {
                m_EditMode.OnEntitySelected -= OnEditModeEntitySelected;
                m_EditMode.OnEntityDeselected -= OnEditModeEntityDeselected;
                m_EditMode.OnEntityDeleted -= OnEditModeEntityDeleted;
                m_EditMode.OnStateChanged -= OnEditModeStateChanged;
            }
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

            if (evt.CommandId == "StartSimulation" && evt.IsSuccess)
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

            if (evt.EntityId == m_EditMode?.SelectedEntityId)
            {
                float battery = 100f;
                if (m_TwinDC.TryGetTelemetry(evt.EntityId, out EntityTelemetry telemetry))
                {
                    battery = telemetry.Battery;
                }
                m_DetailPanel?.ShowEntity(evt.EntityId, evt.EntityType, evt.Status, evt.PosX, evt.PosZ, battery);
            }
        }

        private void OnRuntimeStepChanged(RuntimeStepChangedEvent evt)
        {
            m_RuntimePanel?.UpdateStep(evt.CurrentStep);
        }

        private void OnRouteFailed(ProcedureRouteFailedEvent evt)
        {
            m_ToastUI?.ShowToast(evt.Message, Color.red);
        }

        private void OnSimulationEventExecuted(SimulationEventExecutedEvent evt)
        {
            bool isSuccess = evt.Status == SimulationEventExecutionStatus.Succeeded;

            if (!isSuccess)
            {
                m_RecoveryPanel?.Show(
                    $"Event Failed: {evt.EventType}\nEntity: {evt.EntityId}\nStep: {evt.TimeStep}\n{evt.ErrorMessage}");
            }
            else if (m_RecoveryPanel != null && m_RecoveryPanel.gameObject.activeSelf)
            {
                m_RecoveryPanel.Hide();
            }
        }

        private void OnEditModeEntitySelected(
            string entityId, string entityType, string status,
            float posX, float posZ, float battery)
        {
            m_DetailPanel?.ShowEntity(entityId, entityType, status, posX, posZ, battery);
        }

        private void OnEditModeEntityDeselected()
        {
            m_DetailPanel?.Clear();
        }

        private void OnEditModeEntityDeleted(string entityId)
        {
            m_DetailPanel?.Clear();
            m_EntityList?.Refresh();
            m_ToastUI?.ShowToast($"Deleted: {entityId}", Color.yellow);
        }

        private void OnEditModeStateChanged(EditModeState state)
        {
            string log = $"[EditMode] State -> {state}";
            m_EventLog?.AddEntry(log, true);
        }

        private void OnDestroy()
        {
            Unbind();
        }
    }
}
