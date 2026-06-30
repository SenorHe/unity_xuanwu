using UnityEngine;
using AegisFlow.Command;
using AegisFlow.Data;

namespace AegisFlowDigitalTwin.Editor
{
    public enum EditModeState
    {
        Idle,
        Placing,
        Selected,
        Dragging
    }

    /// <summary>
    /// 编辑模式总控。协调放置、选择、拖拽三种操作。
    /// 通过 UICommandDispatcher 发送命令，通过事件回调通知 UI。
    /// </summary>
    public sealed class EditModeController : MonoBehaviour
    {
        [SerializeField] private float m_GridSize = 3f;
        [SerializeField] private LayerMask m_GroundMask = 1;
        [SerializeField] private LayerMask m_EntityMask = ~0;

        private Camera m_Camera;
        private UICommandDispatcher m_Dispatcher;
        private EntityRepository m_Repository;
        private TwinDC m_TwinDC;

        private EditModeState m_State = EditModeState.Idle;
        private string m_PendingEntityType;
        private string m_SelectedEntityId;
        private EntityPlacementController m_Placement;
        private EntitySelectionController m_Selection;
        private EntityDragController m_Drag;
        private PlacementGhostVisual m_Ghost;

        public EditModeState State => m_State;
        public string SelectedEntityId => m_SelectedEntityId;

        public event System.Action<EditModeState> OnStateChanged;
        public event System.Action<string, string, string, float, float, float> OnEntitySelected;
        public event System.Action OnEntityDeselected;
        public event System.Action<string> OnEntityDeleted;

        public void Initialize(
            Camera camera,
            UICommandDispatcher dispatcher,
            EntityRepository repository,
            TwinDC twinDC)
        {
            m_Camera = camera;
            m_Dispatcher = dispatcher;
            m_Repository = repository;
            m_TwinDC = twinDC;

            m_Ghost = gameObject.AddComponent<PlacementGhostVisual>();
            m_Placement = new EntityPlacementController(m_Camera, m_GridSize, m_GroundMask);
            m_Selection = new EntitySelectionController(m_Camera, m_EntityMask);
            m_Drag = new EntityDragController(m_Camera, m_GridSize, m_GroundMask);

            m_Placement.OnPlaceRequested += OnPlacementRequested;
            m_Drag.OnMoveRequested += OnMoveRequested;
        }

        private void OnPlacementRequested(string entityType, Vector3 position)
        {
            string entityId = $"{entityType}_{System.Guid.NewGuid():N}".Substring(0, 16);
            m_Dispatcher.Dispatch(new PlaceEntityCommand(
                entityType, entityId, entityType,
                position.x, position.y, position.z, 0f));
        }

        private void OnMoveRequested(string entityId, Vector3 position)
        {
            m_Dispatcher.Dispatch(new MoveEntityCommand(
                entityId, position.x, position.y, position.z, 0f));
        }

        public void SetPlacementType(string entityType)
        {
            m_PendingEntityType = entityType;
            m_State = EditModeState.Placing;
            m_Ghost.Show(entityType);
            OnStateChanged?.Invoke(m_State);
        }

        public void CancelPlacement()
        {
            if (m_State == EditModeState.Placing)
            {
                m_PendingEntityType = null;
                m_Ghost.Hide();
                m_State = EditModeState.Idle;
                OnStateChanged?.Invoke(m_State);
            }
        }

        public void Deselect()
        {
            if (m_State == EditModeState.Selected)
            {
                OnEntityDeselected?.Invoke();
                m_SelectedEntityId = null;
                m_State = EditModeState.Idle;
                OnStateChanged?.Invoke(m_State);
            }
        }

        public void DeleteSelected()
        {
            if (m_State == EditModeState.Selected && !string.IsNullOrEmpty(m_SelectedEntityId))
            {
                EntityData entity = m_Repository.Get(m_SelectedEntityId);
                m_TwinDC.RemoveTelemetry(m_SelectedEntityId);
                m_Dispatcher.Dispatch(new MoveEntityCommand(m_SelectedEntityId, 9999f, 9999f, 9999f, 0f));
                OnEntityDeleted?.Invoke(m_SelectedEntityId);
                m_SelectedEntityId = null;
                m_State = EditModeState.Idle;
                OnStateChanged?.Invoke(m_State);
            }
        }

        private void Update()
        {
            switch (m_State)
            {
                case EditModeState.Idle:
                    HandleIdle();
                    break;
                case EditModeState.Placing:
                    HandlePlacing();
                    break;
                case EditModeState.Selected:
                    HandleSelected();
                    break;
                case EditModeState.Dragging:
                    HandleDragging();
                    break;
            }
        }

        private void HandleIdle()
        {
            if (Input.GetMouseButtonDown(0))
            {
                string entityId = m_Selection.PickEntity(Input.mousePosition);
                if (entityId != null)
                {
                    SelectEntity(entityId);
                }
            }
        }

        private void SelectEntity(string entityId)
        {
            m_SelectedEntityId = entityId;
            m_State = EditModeState.Selected;
            OnStateChanged?.Invoke(m_State);

            EntityData entity = m_Repository.Get(entityId);
            if (entity != null)
            {
                float battery = 100f;
                if (m_TwinDC.TryGetTelemetry(entityId, out EntityTelemetry telemetry))
                {
                    battery = telemetry.Battery;
                }

                OnEntitySelected?.Invoke(
                    entityId,
                    entity.EntityType,
                    entity.Status,
                    entity.PosX,
                    entity.PosZ,
                    battery);
            }
        }

        private void HandlePlacing()
        {
            Vector3? pos = m_Placement.GetPlacementPosition(Input.mousePosition);
            if (pos.HasValue)
            {
                m_Ghost.UpdatePosition(pos.Value);
            }

            if (Input.GetMouseButtonDown(0) && pos.HasValue)
            {
                m_Placement.Place(m_PendingEntityType, pos.Value);
                m_Ghost.Hide();
                m_PendingEntityType = null;
                m_State = EditModeState.Idle;
                OnStateChanged?.Invoke(m_State);
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                CancelPlacement();
            }
        }

        private void HandleSelected()
        {
            if (Input.GetMouseButtonDown(0))
            {
                string entityId = m_Selection.PickEntity(Input.mousePosition);
                if (entityId == m_SelectedEntityId)
                {
                    m_State = EditModeState.Dragging;
                    OnStateChanged?.Invoke(m_State);
                }
                else if (entityId != null)
                {
                    SelectEntity(entityId);
                }
                else
                {
                    Deselect();
                }
            }

            if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
            {
                DeleteSelected();
            }
        }

        private void HandleDragging()
        {
            Vector3? pos = m_Drag.GetDragPosition(Input.mousePosition);
            if (pos.HasValue && m_SelectedEntityId != null)
            {
                m_Drag.Move(m_SelectedEntityId, pos.Value);
            }

            if (Input.GetMouseButtonUp(0))
            {
                m_State = EditModeState.Selected;
                OnStateChanged?.Invoke(m_State);

                EntityData entity = m_Repository.Get(m_SelectedEntityId);
                if (entity != null)
                {
                    float battery = 100f;
                    if (m_TwinDC.TryGetTelemetry(m_SelectedEntityId, out EntityTelemetry telemetry))
                    {
                        battery = telemetry.Battery;
                    }
                    OnEntitySelected?.Invoke(
                        m_SelectedEntityId,
                        entity.EntityType,
                        entity.Status,
                        entity.PosX,
                        entity.PosZ,
                        battery);
                }
            }
        }
    }
}
