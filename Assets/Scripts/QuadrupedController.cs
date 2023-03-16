using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadrupedController : MonoBehaviour
{
    [SerializeField] private Transform m_eyeTarget;
    [SerializeField] private Transform m_headBone;

    private void LateUpdate()
    {
        Vector3 towardTarget = m_eyeTarget.position - m_headBone.position;

        m_headBone.rotation = Quaternion.LookRotation(towardTarget, transform.up);
    }
}
