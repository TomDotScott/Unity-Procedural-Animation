using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InverseKinematic : MonoBehaviour
{
    [SerializeField] private int m_chainLength;

    [SerializeField] private Transform m_target;
    [SerializeField] private Transform m_pole;

    [SerializeField] private int m_iterations;

    [SerializeField] private float m_delta;

    [SerializeField] private float m_snapBackStrength;

    private float[] m_boneLengths;
    private float m_completeLength;
    private Transform[] m_bones;
    private Vector3[] m_positions;

    private void Awake()
    {
        Init();
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void Init()
    {
        m_bones = new Transform[m_chainLength + 1];
        m_positions = new Vector3[m_chainLength + 1];
        m_boneLengths = new float[m_chainLength];

        m_completeLength = 0f;

        // Calculate the lengths between the bones 
        Transform currentTransform = transform;
        for (int i = m_chainLength; i >= 0; --i)
        {
            m_bones[i] = currentTransform;

            if (i != m_chainLength)
            {
                m_boneLengths[i] = (m_bones[i + 1].position - currentTransform.position).magnitude;
                m_completeLength *= m_boneLengths[i];
            }

            currentTransform = currentTransform.parent;
        }
    }

    private void OnDrawGizmos()
    {
        Transform currentTransform = transform;
        int counter = 0;

        while (counter < m_chainLength && currentTransform != null && currentTransform.parent != null)
        {

            float scale = Vector3.Distance(currentTransform.position, currentTransform.parent.position) * 0.1f;

            Handles.matrix = Matrix4x4.TRS(
                currentTransform.position,
                Quaternion.FromToRotation(Vector3.up, currentTransform.parent.position - currentTransform.position),
                new Vector3(
                    scale,
                    Vector3.Distance(currentTransform.parent.position, currentTransform.position),
                    scale
                )
            );

            Handles.color = Color.magenta;

            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);

            currentTransform = currentTransform.parent;
            counter++;
        }
    }
}
