using UnityEngine;

namespace AegisFlowDigitalTwin.Visual
{
    /// <summary>
    /// 实体视觉基类。负责将 EntityData 映射到 Unity GameObject。
    /// 挂载在实体 GameObject 上，由 EntityVisualFactory 创建。
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class EntityVisualBase : MonoBehaviour
    {
        public string EntityId { get; private set; }
        public string EntityType { get; private set; }
        public string Status { get; private set; }

        protected Material m_MainMaterial;
        protected Material m_SelectionMaterial;
        protected bool m_IsSelected;

        public void Initialize(string entityId, string entityType)
        {
            EntityId = entityId;
            EntityType = entityType;
            Status = "Idle";
            OnInitialize();
        }

        public void SetStatus(string status)
        {
            Status = status;
            OnStatusChanged(status);
        }

        public void SetSelected(bool selected)
        {
            m_IsSelected = selected;
            OnSelectionChanged(selected);
        }

        public void SetPosition(float x, float y, float z)
        {
            transform.position = new Vector3(x, y, z);
        }

        public void SetRotationY(float rotY)
        {
            transform.rotation = Quaternion.Euler(0f, rotY, 0f);
        }

        protected abstract void OnInitialize();
        protected abstract void OnStatusChanged(string status);
        protected abstract void OnSelectionChanged(bool selected);

        protected static Material CreateMaterial(Color color, float smoothness = 0.5f)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = color;
            mat.SetFloat("_Smoothness", smoothness);
            return mat;
        }

        protected static GameObject CreateChild(string name, PrimitiveType type, Vector3 localPos, Vector3 scale, Material mat, Transform parent)
        {
            GameObject obj = GameObject.CreatePrimitive(type);
            obj.name = name;
            obj.transform.SetParent(parent);
            obj.transform.localPosition = localPos;
            obj.transform.localScale = scale;

            MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
            if (renderer != null && mat != null)
            {
                renderer.material = mat;
            }

            return obj;
        }
    }
}
