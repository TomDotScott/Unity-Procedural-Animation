using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

class TerrainMeshData : MonoBehaviour
{
    public Texture2D Heightmap;
    [SerializeField] private int m_meshResolution;
    [SerializeField] private float m_heightmapScale;
    [SerializeField] private float m_width;

    public int GetResolution()
    {
        return m_meshResolution;
    }

    public float GetWidth()
    {
        return m_width;
    }

    public float GetScale()
    {
        return m_heightmapScale;
    }
}
