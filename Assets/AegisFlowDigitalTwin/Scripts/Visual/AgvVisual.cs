using UnityEngine;

namespace AegisFlowDigitalTwin.Visual
{
    /// <summary>
    /// AGV 搬运车视觉。车身 + 4 轮 + 顶部指示灯。
    /// </summary>
    public sealed class AgvVisual : EntityVisualBase
    {
        private Material m_BodyMat;
        private Material m_WheelMat;
        private Material m_LightMat;
        private GameObject m_LightObj;

        protected override void OnInitialize()
        {
            m_BodyMat = CreateMaterial(new Color(0.15f, 0.45f, 0.85f), 0.6f);
            m_WheelMat = CreateMaterial(new Color(0.08f, 0.08f, 0.08f), 0.3f);
            m_LightMat = CreateMaterial(new Color(0f, 1f, 0f), 0.9f);

            CreateChild("Body", PrimitiveType.Cube, new Vector3(0f, 0.3f, 0f), new Vector3(0.8f, 0.4f, 1.2f), m_BodyMat, transform);
            CreateChild("Top", PrimitiveType.Cube, new Vector3(0f, 0.55f, 0f), new Vector3(0.6f, 0.1f, 0.8f), m_BodyMat, transform);

            float wheelY = 0.1f;
            float wheelX = 0.45f;
            float wheelZ = 0.4f;
            CreateChild("Wheel_FL", PrimitiveType.Cylinder, new Vector3(-wheelX, wheelY, wheelZ), new Vector3(0.15f, 0.05f, 0.15f), m_WheelMat, transform);
            CreateWheelRotation(CreateChild("Wheel_FL", PrimitiveType.Cylinder, new Vector3(-wheelX, wheelY, wheelZ), new Vector3(0.15f, 0.05f, 0.15f), m_WheelMat, transform));
            CreateWheelRotation(CreateChild("Wheel_FR", PrimitiveType.Cylinder, new Vector3(wheelX, wheelY, wheelZ), new Vector3(0.15f, 0.05f, 0.15f), m_WheelMat, transform));
            CreateWheelRotation(CreateChild("Wheel_RL", PrimitiveType.Cylinder, new Vector3(-wheelX, wheelY, -wheelZ), new Vector3(0.15f, 0.05f, 0.15f), m_WheelMat, transform));
            CreateWheelRotation(CreateChild("Wheel_RR", PrimitiveType.Cylinder, new Vector3(wheelX, wheelY, -wheelZ), new Vector3(0.15f, 0.05f, 0.15f), m_WheelMat, transform));

            m_LightObj = CreateChild("StatusLight", PrimitiveType.Sphere, new Vector3(0f, 0.7f, 0f), new Vector3(0.12f, 0.12f, 0.12f), m_LightMat, transform);
        }

        private void CreateWheelRotation(GameObject wheel)
        {
            wheel.transform.Rotate(90f, 0f, 0f);
        }

        protected override void OnStatusChanged(string status)
        {
            if (m_LightObj == null)
            {
                return;
            }

            MeshRenderer renderer = m_LightObj.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                return;
            }

            Color lightColor = Color.green;
            switch (status)
            {
                case "Moving":
                    lightColor = Color.cyan;
                    break;
                case "Arrived":
                    lightColor = Color.yellow;
                    break;
                case "Charging":
                    lightColor = Color.magenta;
                    break;
                case "Error":
                case "Failed":
                    lightColor = Color.red;
                    break;
                case "Idle":
                default:
                    lightColor = Color.green;
                    break;
            }

            renderer.material.color = lightColor;
        }

        protected override void OnSelectionChanged(bool selected)
        {
            if (m_BodyMat == null)
            {
                return;
            }

            m_BodyMat.color = selected
                ? new Color(1f, 0.85f, 0.2f)
                : new Color(0.15f, 0.45f, 0.85f);
        }
    }
}
