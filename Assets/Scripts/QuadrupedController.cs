using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadrupedController : MonoBehaviour
{
    [SerializeField] private Transform m_eyeTarget;
    [SerializeField] private Transform m_headBone;

    [SerializeField] private float m_headTurnSpeed;

    private void LateUpdate()
    {
        Vector3 towardTarget = m_eyeTarget.position - m_headBone.position;

        Quaternion targetRotation = Quaternion.LookRotation(towardTarget, transform.up);

        // Smooth the rotation
        m_headBone.rotation = Quaternion.Slerp(
            targetRotation,
            m_headBone.rotation,
            1 - Mathf.Exp(-m_headTurnSpeed * Time.deltaTime)
       );
    }
}
