using UnityEngine;
using UnityEngine.AI;

namespace AegisFlowDigitalTwin.Scene
{
    /// <summary>
    /// 运行时 NavMesh 烘焙器。编辑态环境搭建后动态烘焙导航网格。
    /// </summary>
    public sealed class NavMeshBaker : MonoBehaviour
    {
        [SerializeField] private Vector3 m_Size = new Vector3(60f, 10f, 60f);
        [SerializeField] private int m_AgentTypeID = 0;

        private NavMeshData m_NavMeshData;
        private NavMeshSurface m_Surface;

        private void Awake()
        {
            m_Surface = gameObject.AddComponent<NavMeshSurface>();
            m_Surface.agentTypeID = m_AgentTypeID;
            m_Surface.collectObjects = CollectObjects.All;
            m_Surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            m_Surface.layerMask = ~0;
            m_Surface.defaultArea = 0;
        }

        public void BuildNavMesh()
        {
            if (m_Surface == null)
            {
                return;
            }

            m_Surface.BuildNavMesh();
            Debug.Log("[DigitalTwin] NavMesh baked successfully.");
        }

        public void ClearNavMesh()
        {
            if (m_NavMeshData != null)
            {
                NavMesh.RemoveNavMeshData(m_NavMeshData);
                m_NavMeshData = null;
            }
        }
    }
}
