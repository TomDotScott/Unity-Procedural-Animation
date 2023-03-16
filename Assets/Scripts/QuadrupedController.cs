using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadrupedController : MonoBehaviour
{
    [SerializeField] private Transform m_eyeTarget;
    [SerializeField] private Transform m_headBone;

    [SerializeField] private float m_headTurnSpeed;
    [SerializeField] private float m_maxHeadAngle;

    private void LateUpdate()
    {
        Quaternion currentRotation = m_headBone.localRotation;

        // reset the headbone rotation 
        m_headBone.localRotation = Quaternion.identity;

        Vector3 towardTargetWorldSpace = m_eyeTarget.position - m_headBone.position;
        Vector3 towardTargetLocalSpace = m_headBone.InverseTransformDirection(towardTargetWorldSpace);

        // Apply the rotation limit to the head
        towardTargetLocalSpace = Vector3.RotateTowards(Vector3.forward, towardTargetLocalSpace, Mathf.Deg2Rad * m_maxHeadAngle, 0f);

        Quaternion targetRotation = Quaternion.LookRotation(towardTargetLocalSpace, transform.up);

        // Smooth the rotation
        m_headBone.rotation = Quaternion.Slerp(
            currentRotation,
            targetRotation,
            1 - Mathf.Exp(-m_headTurnSpeed * Time.deltaTime)
       );
    }
}
