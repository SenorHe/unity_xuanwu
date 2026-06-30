using UnityEngine;

namespace AegisFlowDigitalTwin.Visual
{
    /// <summary>
    /// 工作站视觉。台面 + 机械臂 + 指示灯。
    /// </summary>
    public sealed class WorkStationVisual : EntityVisualBase
    {
        private Material m_BodyMat;
        private Material m_ArmMat;
        private Material m_LightMat;
        private GameObject m_LightObj;
        private Transform m_ArmPivot;

        protected override void OnInitialize()
        {
            m_BodyMat = CreateMaterial(new Color(0.4f, 0.2f, 0.6f), 0.5f);
            m_ArmMat = CreateMaterial(new Color(0.25f, 0.25f, 0.28f), 0.6f);
            m_LightMat = CreateMaterial(new Color(1f, 0.5f, 0f), 0.9f);

            CreateChild("Table", PrimitiveType.Cube, new Vector3(0f, 0.4f, 0f), new Vector3(1.2f, 0.1f, 1f), m_BodyMat, transform);
            CreateChild("Leg_FL", PrimitiveType.Cube, new Vector3(-0.5f, 0.2f, 0.4f), new Vector3(0.08f, 0.4f, 0.08f), m_BodyMat, transform);
            CreateChild("Leg_FR", PrimitiveType.Cube, new Vector3(0.5f, 0.2f, 0.4f), new Vector3(0.08f, 0.4f, 0.08f), m_BodyMat, transform);
            CreateChild("Leg_RL", PrimitiveType.Cube, new Vector3(-0.5f, 0.2f, -0.4f), new Vector3(0.08f, 0.4f, 0.08f), m_BodyMat, transform);
            CreateChild("Leg_RR", PrimitiveType.Cube, new Vector3(0.5f, 0.2f, -0.4f), new Vector3(0.08f, 0.4f, 0.08f), m_BodyMat, transform);

            GameObject armBase = CreateChild("ArmBase", PrimitiveType.Cylinder, new Vector3(0f, 0.55f, -0.3f), new Vector3(0.15f, 0.1f, 0.15f), m_ArmMat, transform);
            armBase.transform.Rotate(90f, 0f, 0f);

            GameObject armPivot = new GameObject("ArmPivot");
            armPivot.transform.SetParent(transform);
            armPivot.transform.localPosition = new Vector3(0f, 0.65f, -0.3f);
            m_ArmPivot = armPivot.transform;

            CreateChild("ArmUpper", PrimitiveType.Cube, new Vector3(0f, 0.3f, 0f), new Vector3(0.06f, 0.6f, 0.06f), m_ArmMat, m_ArmPivot);

            GameObject armJoint = CreateChild("ArmJoint", PrimitiveType.Sphere, new Vector3(0f, 0.6f, 0f), new Vector3(0.1f, 0.1f, 0.1f), m_ArmMat, m_ArmPivot);

            GameObject forearmPivot = new GameObject("ForearmPivot");
            forearmPivot.transform.SetParent(m_ArmPivot);
            forearmPivot.transform.localPosition = new Vector3(0f, 0.6f, 0f);

            CreateChild("Forearm", PrimitiveType.Cube, new Vector3(0f, 0.2f, 0.25f), new Vector3(0.05f, 0.05f, 0.5f), m_ArmMat, forearmPivot.transform);
            forearmPivot.transform.localRotation = Quaternion.Euler(30f, 0f, 0f);

            m_LightObj = CreateChild("StatusLight", PrimitiveType.Sphere, new Vector3(0f, 0.85f, 0.3f), new Vector3(0.08f, 0.08f, 0.08f), m_LightMat, transform);
        }

        private void Update()
        {
            if (m_ArmPivot == null)
            {
                return;
            }

            float targetRot = Status == "Working" ? Mathf.Sin(Time.time * 2f) * 45f : 0f;
            m_ArmPivot.localRotation = Quaternion.Lerp(m_ArmPivot.localRotation, Quaternion.Euler(0f, targetRot, 0f), Time.deltaTime * 3f);
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

            renderer.material.color = status == "Working"
                ? new Color(0f, 1f, 0f)
                : new Color(1f, 0.5f, 0f);
        }

        protected override void OnSelectionChanged(bool selected)
        {
            if (m_BodyMat == null)
            {
                return;
            }

            m_BodyMat.color = selected
                ? new Color(1f, 0.85f, 0.2f)
                : new Color(0.4f, 0.2f, 0.6f);
        }
    }
}
