using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

class TerrainMeshData : MonoBehaviour
{
    [SerializeField] private int m_meshResolution;

    public int GetResolution()
    {
        return m_meshResolution;
    }
}

[CustomEditor(typeof(TerrainMeshData))]
public class MeshGenerator : Editor
{
    TerrainMeshData m_terrainMeshData;

    void GenerateMesh()
    {
        Mesh mesh = m_terrainMeshData.GetComponent<MeshFilter>().sharedMesh;

        int resolution = m_terrainMeshData.GetResolution();

        Vector3[] vertices = new Vector3[resolution * resolution];

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                vertices[x * resolution + y] = new Vector3(x, 0, y);
            }
        }

        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int index = 0;
        for (int y = 0; y < resolution - 1; y++)
        {
            for (int x = 0; x < resolution - 1; x++)
            {
                int i0 = y * resolution + x;
                int i1 = y * resolution + x + 1;
                int i2 = (y + 1) * resolution + x;
                int i3 = (y + 1) * resolution + x + 1;

                triangles[index++] = i0;
                triangles[index++] = i1;
                triangles[index++] = i2;

                triangles[index++] = i1;
                triangles[index++] = i3;
                triangles[index++] = i2;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        m_terrainMeshData = (TerrainMeshData)target;

        if (GUILayout.Button("Generate Terrain Mesh"))
        {
            GenerateMesh();
        }
    }
}
