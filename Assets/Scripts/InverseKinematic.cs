using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
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

    private Vector3[] m_startDirectionSuccessors;
    private Quaternion[] m_startRotationBones;
    private Quaternion m_startRotationTarget;
    private Quaternion m_startRotationRoot;

    private void Awake()
    {
        Init();
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void LateUpdate()
    {
        ResolveIK();
    }

    private void Init()
    {
        m_bones = new Transform[m_chainLength + 1];
        m_positions = new Vector3[m_chainLength + 1];
        m_boneLengths = new float[m_chainLength];

        m_startDirectionSuccessors = new Vector3[m_chainLength + 1];
        m_startRotationBones = new Quaternion[m_chainLength + 1];


        m_startRotationTarget = m_target.rotation;
        m_completeLength = 0f;

        // Calculate the lengths between the bones 
        Transform currentTransform = transform;
        for (int i = m_chainLength; i >= 0; --i)
        {
            m_bones[i] = currentTransform;

            m_startRotationBones[i] = currentTransform.rotation;

            if (i == m_chainLength)
            {
                // We're at the root, so make it relative to the target
                m_startDirectionSuccessors[i] = m_target.position - currentTransform.position;
            }
            else
            {
                // Mid-bone so calculate as normal...
                m_startDirectionSuccessors[i] = m_bones[i + 1].position - currentTransform.position;
                m_boneLengths[i] = (m_bones[i + 1].position - currentTransform.position).magnitude;
                m_completeLength += m_boneLengths[i];
            }

            currentTransform = currentTransform.parent;
        }
    }

    private void ResolveIK()
    {
        if (m_target == null)
        {
            return;
        }

        if (m_boneLengths.Length != m_chainLength)
        {
            Init();
        }

        // Get the position for the bones in the IK
        for (int i = 0; i < m_bones.Length; ++i)
        {
            m_positions[i] = m_bones[i].position;
        }

        Quaternion rootRotation = Quaternion.identity;
        if (m_bones[0] != null)
        {
            rootRotation = m_bones[0].rotation;
        }

        Quaternion rootRotationDifference = rootRotation * Quaternion.Inverse(m_startRotationRoot);

        // If the target is further away than the complete length of the IK chain...
        if ((m_target.position - m_bones[0].position).sqrMagnitude >= m_completeLength * m_completeLength)
        {
            // Stretch it in the direction
            Vector3 dir = (m_target.position - m_positions[0]).normalized;

            for (int i = 1; i < m_positions.Length; ++i)
            {
                m_positions[i] = m_positions[i - 1] + dir * m_boneLengths[i - 1];
            }
        }
        else
        {
            int iteration = 0;
            while (iteration < m_iterations && (m_positions[^1] - m_target.position).sqrMagnitude > m_delta * m_delta)
            {
                // Propagate backwards
                for (int i = m_positions.Length - 1; i > 0; --i)
                {
                    if (i == m_positions.Length - 1)
                    {
                        m_positions[i] = m_target.position;
                    }
                    else
                    {
                        Vector3 boneDirection = (m_positions[i] - m_positions[i + 1]).normalized * m_boneLengths[i];
                        m_positions[i] = m_positions[i + 1] + boneDirection;
                    }
                }

                // Propagate forwards
                for (int i = 1; i < m_positions.Length; ++i)
                {
                    Vector3 boneDirection = (m_positions[i] - m_positions[i - 1]).normalized * m_boneLengths[i - 1];
                    m_positions[i] = m_positions[i - 1] + boneDirection;
                }

                iteration++;
            }
        }

        // Move towards the pole
        if (m_pole != null)
        {
            for (int i = 1; i < m_positions.Length - 1; ++i)
            {
                Plane plane = new Plane(m_positions[i + 1] - m_positions[i - 1], m_positions[i - 1]);
                Vector3 projectedPole = plane.ClosestPointOnPlane(m_pole.position);
                Vector3 projectedBone = plane.ClosestPointOnPlane(m_positions[i]);

                float angle = Vector3.SignedAngle(
                    projectedBone - m_positions[i - 1],
                    projectedPole - m_positions[i - 1],
                    plane.normal
                );

                m_positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (m_positions[i] - m_positions[i - 1]) + m_positions[i - 1];
            }
        }


        // Set the position and rotation for the bones in the IK
        for (int i = 0; i < m_positions.Length; ++i)
        {
            if (i == m_positions.Length - 1)
            {
                m_bones[i].rotation = m_target.rotation * Quaternion.Inverse(m_startRotationTarget) * m_startRotationBones[i];
            }
            else
            {
                m_bones[i].rotation = Quaternion.FromToRotation(m_startDirectionSuccessors[i], m_positions[i + 1] - m_positions[i]) * m_startRotationBones[i];
            }

            m_bones[i].position = m_positions[i];
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
