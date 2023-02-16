using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainMeshData))]
public class MeshGenerator : Editor
{
    private TerrainMeshData m_terrainMeshData;

    void GenerateMesh()
    {
        Mesh mesh = m_terrainMeshData.GetComponent<MeshFilter>().sharedMesh;

        mesh.vertices = GenerateVertices(m_terrainMeshData.GetResolution(), m_terrainMeshData.GetWidth());
        mesh.triangles = GenerateTriangles(m_terrainMeshData.GetResolution());
        mesh.uv = GenerateUVs(m_terrainMeshData.GetResolution());

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
    }

    private Vector3[] GenerateVertices(int meshResolution, float meshWidth)
    {
        Vector3[] vertices = new Vector3[meshResolution * meshResolution];

        // res-1 because we start counting at 0!
        // Caching aspect here because we don't wanna be calculating it
        // max 65,536 times! 
        float meshAspect = meshWidth / (meshResolution - 1);

        for (int y = 0; y < meshResolution; y++)
        {
            for (int x = 0; x < meshResolution; x++)
            {
                vertices[x * meshResolution + y] = new Vector3(
                    meshAspect * x,
                    GenerateHeightValue(x, y, meshResolution) * m_terrainMeshData.GetScale(),
                    meshAspect * y
                );
            }
        }

        return vertices;
    }

    private int[] GenerateTriangles(int meshResolution)
    {
        int[] triangles = new int[(meshResolution - 1) * (meshResolution - 1) * 6];

        int index = 0;
        for (int y = 0; y < meshResolution - 1; y++)
        {
            for (int x = 0; x < meshResolution - 1; x++)
            {
                int i0 = y * meshResolution + x;
                int i1 = y * meshResolution + x + 1;
                int i2 = (y + 1) * meshResolution + x;
                int i3 = (y + 1) * meshResolution + x + 1;

                // Counter-Clockwise triangle indices
                triangles[index++] = i0;
                triangles[index++] = i1;
                triangles[index++] = i2;

                triangles[index++] = i1;
                triangles[index++] = i3;
                triangles[index++] = i2;
            }
        }

        return triangles;
    }

    private Vector2[] GenerateUVs(int meshResolution)
    {
        Vector2[] uvs = new Vector2[meshResolution * meshResolution];

        for (int y = 0; y < meshResolution; y++)
        {
            for (int x = 0; x < meshResolution; x++)
            {
                uvs[y * meshResolution + x] = new Vector2((float)x / meshResolution, (float)y / meshResolution);
            }
        }

        return uvs;
    }

    private float GenerateHeightValue(int xPos, int yPos, int meshResolution)
    {
        // Grab the heightmap data
        Texture2D heightmap = m_terrainMeshData.Heightmap;

        // Find out how many pixels per vertex
        int heightmapAspect = heightmap.width / (meshResolution - 1);

        // Calculate the average height value for the vertex
        float averageValue = 0f;
        for (int i = 0; i < heightmapAspect; i++)
        {
            // Grab the pixel value at the x->x+4
            averageValue += heightmap.GetPixel(xPos + i, yPos).grayscale;
        }

        return averageValue / heightmapAspect;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        m_terrainMeshData = (TerrainMeshData)target;

        if (GUILayout.Button("Generate Terrain Mesh") || GUI.changed)
        {
            GenerateMesh();
        }

        Texture2D texture = AssetPreview.GetAssetPreview(m_terrainMeshData.Heightmap);
        GUILayout.Label("", GUILayout.Height(80), GUILayout.Width(80));
        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), texture);
    }
}