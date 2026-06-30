using UnityEngine;
using UnityEngine.AI;

namespace AegisFlowDigitalTwin.Simulation
{
    /// <summary>
    /// NavMesh 导航封装。桥接 AegisFlow Runtime 事件到 Unity NavMeshAgent。
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class AgvNavigator : MonoBehaviour
    {
        private NavMeshAgent m_Agent;
        private bool m_HasDestination;
        private string m_TargetEntityId;

        public bool HasArrived => m_Agent != null && !m_Agent.pathPending && m_Agent.remainingDistance < 0.5f;
        public bool IsMoving => m_Agent != null && m_Agent.velocity.magnitude > 0.1f;
        public float Speed => m_Agent != null ? m_Agent.velocity.magnitude : 0f;

        private void Awake()
        {
            m_Agent = GetComponent<NavMeshAgent>();
            m_Agent.speed = 3f;
            m_Agent.angularSpeed = 180f;
            m_Agent.acceleration = 8f;
            m_Agent.stoppingDistance = 0.5f;
            m_Agent.radius = 0.3f;
            m_Agent.height = 0.6f;
        }

        public void MoveTo(Vector3 destination, string targetEntityId = null)
        {
            if (m_Agent == null || !m_Agent.isOnNavMesh)
            {
                return;
            }

            m_Agent.isStopped = false;
            m_Agent.SetDestination(destination);
            m_HasDestination = true;
            m_TargetEntityId = targetEntityId;
        }

        public void Stop()
        {
            if (m_Agent != null && m_Agent.isOnNavMesh)
            {
                m_Agent.isStopped = true;
            }

            m_HasDestination = false;
            m_TargetEntityId = null;
        }

        public string GetTargetEntityId()
        {
            return m_TargetEntityId;
        }
    }
}
