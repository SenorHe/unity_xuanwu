using UnityEngine;

namespace AegisFlowDigitalTwin.Environment
{
    /// <summary>
    /// 程序化生成仓储环境：地面、围墙、网格标记。
    /// </summary>
    public sealed class WarehouseBuilder : MonoBehaviour
    {
        [SerializeField] private float m_FloorSize = 60f;
        [SerializeField] private float m_WallHeight = 4f;
        [SerializeField] private float m_GridSpacing = 3f;

        private void Start()
        {
            BuildFloor();
            BuildWalls();
            BuildGridMarkers();
        }

        private void BuildFloor()
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "WarehouseFloor";
            floor.transform.SetParent(transform);
            floor.transform.localScale = new Vector3(m_FloorSize / 10f, 1f, m_FloorSize / 10f);
            floor.transform.position = Vector3.zero;

            MeshRenderer renderer = floor.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Material mat = CreateFloorMaterial();
                renderer.material = mat;
            }

            floor.layer = LayerMask.NameToLayer("Default");
        }

        private void BuildWalls()
        {
            float half = m_FloorSize / 2f;
            float wallThickness = 0.3f;

            CreateWall("Wall_North", new Vector3(0f, m_WallHeight / 2f, half), new Vector3(m_FloorSize, m_WallHeight, wallThickness));
            CreateWall("Wall_South", new Vector3(0f, m_WallHeight / 2f, -half), new Vector3(m_FloorSize, m_WallHeight, wallThickness));
            CreateWall("Wall_East", new Vector3(half, m_WallHeight / 2f, 0f), new Vector3(wallThickness, m_WallHeight, m_FloorSize));
            CreateWall("Wall_West", new Vector3(-half, m_WallHeight / 2f, 0f), new Vector3(wallThickness, m_WallHeight, m_FloorSize));
        }

        private void CreateWall(string name, Vector3 position, Vector3 scale)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(transform);
            wall.transform.position = position;
            wall.transform.localScale = scale;

            MeshRenderer renderer = wall.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = CreateWallMaterial();
            }
        }

        private void BuildGridMarkers()
        {
            GameObject gridParent = new GameObject("GridMarkers");
            gridParent.transform.SetParent(transform);

            float half = m_FloorSize / 2f - m_GridSpacing;
            Material gridMat = CreateGridMaterial();

            for (float x = -half; x <= half; x += m_GridSpacing)
            {
                for (float z = -half; z <= half; z += m_GridSpacing)
                {
                    GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    marker.name = $"Grid_{x:F0}_{z:F0}";
                    marker.transform.SetParent(gridParent.transform);
                    marker.transform.position = new Vector3(x, 0.02f, z);
                    marker.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                    marker.transform.localScale = new Vector3(0.15f, 0.15f, 1f);

                    MeshRenderer renderer = marker.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        renderer.material = gridMat;
                    }

                    Collider col = marker.GetComponent<Collider>();
                    if (col != null)
                    {
                        Destroy(col);
                    }
                }
            }
        }

        private Material CreateFloorMaterial()
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = new Color(0.18f, 0.2f, 0.24f, 1f);
            mat.SetFloat("_Smoothness", 0.8f);
            return mat;
        }

        private Material CreateWallMaterial()
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = new Color(0.35f, 0.38f, 0.42f, 1f);
            mat.SetFloat("_Smoothness", 0.5f);
            return mat;
        }

        private Material CreateGridMaterial()
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            mat.color = new Color(0.1f, 0.6f, 0.9f, 0.4f);
            return mat;
        }
    }
}
