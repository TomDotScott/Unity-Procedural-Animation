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

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

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
