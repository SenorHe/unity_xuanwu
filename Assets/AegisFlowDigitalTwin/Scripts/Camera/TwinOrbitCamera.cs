using UnityEngine;

namespace AegisFlowDigitalTwin.Camera
{
    /// <summary>
    /// 轨道俯视相机。鼠标右键旋转，滚轮缩放，中键平移。
    /// </summary>
    public sealed class TwinOrbitCamera : MonoBehaviour
    {
        [SerializeField] private float m_MinDistance = 5f;
        [SerializeField] private float m_MaxDistance = 80f;
        [SerializeField] private float m_RotateSpeed = 120f;
        [SerializeField] private float m_ZoomSpeed = 15f;
        [SerializeField] private float m_PanSpeed = 0.3f;
        [SerializeField] private float m_MinPitch = 10f;
        [SerializeField] private float m_MaxPitch = 85f;

        private Transform m_Self;
        private float m_Distance = 30f;
        private float m_Yaw = 45f;
        private float m_Pitch = 50f;
        private Vector3 m_Target = Vector3.zero;
        private Vector3 m_LastMousePos;

        private void Awake()
        {
            m_Self = transform;
        }

        private void Start()
        {
            UpdateTransform();
        }

        private void LateUpdate()
        {
            HandleRotation();
            HandleZoom();
            HandlePan();
            UpdateTransform();
        }

        private void HandleRotation()
        {
            if (!Input.GetMouseButton(1))
            {
                return;
            }

            Vector3 delta = Input.mousePosition - m_LastMousePos;
            m_Yaw += delta.x * m_RotateSpeed * Time.deltaTime;
            m_Pitch -= delta.y * m_RotateSpeed * Time.deltaTime;
            m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);
            m_LastMousePos = Input.mousePosition;
        }

        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) < 0.001f)
            {
                return;
            }

            m_Distance -= scroll * m_ZoomSpeed * 10f * Time.deltaTime;
            m_Distance = Mathf.Clamp(m_Distance, m_MinDistance, m_MaxDistance);
        }

        private void HandlePan()
        {
            if (!Input.GetMouseButton(2))
            {
                return;
            }

            Vector3 delta = Input.mousePosition - m_LastMousePos;
            Vector3 right = m_Self.right;
            Vector3 forward = Vector3.ProjectOnPlane(m_Self.forward, Vector3.up).normalized;
            m_Target -= right * delta.x * m_PanSpeed * Time.deltaTime * m_Distance * 0.1f;
            m_Target -= forward * delta.y * m_PanSpeed * Time.deltaTime * m_Distance * 0.1f;
            m_LastMousePos = Input.mousePosition;
        }

        private void UpdateTransform()
        {
            Quaternion rotation = Quaternion.Euler(m_Pitch, m_Yaw, 0f);
            Vector3 offset = rotation * Vector3.back * m_Distance;
            m_Self.position = m_Target + offset;
            m_Self.rotation = rotation;
        }

        public void FocusOn(Vector3 position)
        {
            m_Target = position;
        }
    }
}
