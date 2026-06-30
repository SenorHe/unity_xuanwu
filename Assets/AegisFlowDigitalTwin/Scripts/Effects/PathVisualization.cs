using System.Collections.Generic;
using UnityEngine;

namespace AegisFlowDigitalTwin.Effects
{
    /// <summary>
    /// AGV 路径可视化。用 LineRenderer 绘制 AGV 行走轨迹。
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public sealed class PathVisualization : MonoBehaviour
    {
        [SerializeField] private int m_MaxPoints = 100;
        [SerializeField] private float m_PointLifetime = 5f;
        [SerializeField] private Color m_PathColor = new Color(0f, 0.8f, 1f, 0.5f);

        private LineRenderer m_LineRenderer;
        private readonly List<Vector3> m_Points = new List<Vector3>();
        private readonly List<float> m_Timestamps = new List<float>();

        private void Awake()
        {
            m_LineRenderer = GetComponent<LineRenderer>();
            m_LineRenderer.startWidth = 0.05f;
            m_LineRenderer.endWidth = 0.05f;
            m_LineRenderer.startColor = m_PathColor;
            m_LineRenderer.endColor = new Color(m_PathColor.r, m_PathColor.g, m_PathColor.b, 0f);
            m_LineRenderer.positionCount = 0;
            m_LineRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        }

        public void AddPoint(Vector3 position)
        {
            m_Points.Add(position);
            m_Timestamps.Add(Time.time);

            if (m_Points.Count > m_MaxPoints)
            {
                m_Points.RemoveAt(0);
                m_Timestamps.RemoveAt(0);
            }

            UpdateLineRenderer();
        }

        public void Clear()
        {
            m_Points.Clear();
            m_Timestamps.Clear();
            m_LineRenderer.positionCount = 0;
        }

        private void Update()
        {
            RemoveExpiredPoints();
        }

        private void RemoveExpiredPoints()
        {
            bool removed = false;
            while (m_Timestamps.Count > 0 && Time.time - m_Timestamps[0] > m_PointLifetime)
            {
                m_Timestamps.RemoveAt(0);
                m_Points.RemoveAt(0);
                removed = true;
            }

            if (removed)
            {
                UpdateLineRenderer();
            }
        }

        private void UpdateLineRenderer()
        {
            m_LineRenderer.positionCount = m_Points.Count;
            for (int i = 0; i < m_Points.Count; i++)
            {
                m_LineRenderer.SetPosition(i, m_Points[i] + Vector3.up * 0.05f);
            }
        }
    }
}
