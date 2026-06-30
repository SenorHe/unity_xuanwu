using UnityEngine;

namespace AegisFlowDigitalTwin.Editor
{
    /// <summary>
    /// 编辑模式状态机。管理 Idle / Placing / Selected / Dragging 四种状态。
    /// </summary>
    public enum EditModeState
    {
        Idle,
        Placing,
        Selected,
        Dragging
    }

    /// <summary>
    /// 编辑模式总控。协调放置、选择、拖拽三种操作。
    /// </summary>
    public sealed class EditModeController : MonoBehaviour
    {
        [SerializeField] private float m_GridSize = 3f;
        [SerializeField] private LayerMask m_GroundMask = 1;
        [SerializeField] private LayerMask m_EntityMask = ~0;
        [SerializeField] private Camera m_Camera;

        private EditModeState m_State = EditModeState.Idle;
        private string m_SelectedEntityType;
        private string m_SelectedEntityId;
        private EntityPlacementController m_Placement;
        private EntitySelectionController m_Selection;
        private EntityDragController m_Drag;
        private PlacementGhostVisual m_Ghost;

        public EditModeState State => m_State;
        public string SelectedEntityId => m_SelectedEntityId;
        public string SelectedEntityType => m_SelectedEntityType;

        public event System.Action<EditModeState> OnStateChanged;
        public event System.Action<string> OnEntitySelected;
        public event System.Action<string> OnEntityDeselected;

        private void Awake()
        {
            if (m_Camera == null)
            {
                m_Camera = UnityEngine.Camera.main;
            }

            m_Ghost = gameObject.AddComponent<PlacementGhostVisual>();
            m_Placement = new EntityPlacementController(m_Camera, m_GridSize, m_GroundMask);
            m_Selection = new EntitySelectionController(m_Camera, m_EntityMask);
            m_Drag = new EntityDragController(m_Camera, m_GridSize, m_GroundMask);
        }

        public void SetPlacementType(string entityType)
        {
            m_SelectedEntityType = entityType;
            m_State = EditModeState.Placing;
            m_Ghost.Show(entityType);
            OnStateChanged?.Invoke(m_State);
        }

        public void CancelPlacement()
        {
            if (m_State == EditModeState.Placing)
            {
                m_SelectedEntityType = null;
                m_Ghost.Hide();
                m_State = EditModeState.Idle;
                OnStateChanged?.Invoke(m_State);
            }
        }

        public void Deselect()
        {
            if (m_State == EditModeState.Selected)
            {
                OnEntityDeselected?.Invoke(m_SelectedEntityId);
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
                    m_SelectedEntityId = entityId;
                    m_State = EditModeState.Selected;
                    OnStateChanged?.Invoke(m_State);
                    OnEntitySelected?.Invoke(entityId);
                }
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
                m_Placement.Place(m_SelectedEntityType, pos.Value);
                m_Ghost.Hide();
                m_SelectedEntityType = null;
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
                    OnEntityDeselected?.Invoke(m_SelectedEntityId);
                    m_SelectedEntityId = entityId;
                    OnEntitySelected?.Invoke(entityId);
                }
                else
                {
                    Deselect();
                }
            }

            if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
            {
                if (m_SelectedEntityId != null)
                {
                    OnEntityDeselected?.Invoke(m_SelectedEntityId);
                    m_SelectedEntityId = null;
                    m_State = EditModeState.Idle;
                    OnStateChanged?.Invoke(m_State);
                }
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
            }
        }
    }
}
