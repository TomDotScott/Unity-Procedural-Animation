using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadrupedController : MonoBehaviour
{
    [SerializeField] private Transform m_eyeTarget;
    [SerializeField] private Transform m_headBone;

    [SerializeField] private float m_headTurnSpeed;
    [SerializeField] private float m_maxHeadAngle;

    [SerializeField] private Transform m_leftEye;
    [SerializeField] private Transform m_rightEye;

    [SerializeField] private float m_eyeTurnSpeed;

    [SerializeField] private float m_minLeftEyeAngle;
    [SerializeField] private float m_maxLeftEyeAngle;

    [SerializeField] private float m_minRightEyeAngle;
    [SerializeField] private float m_maxRightEyeAngle;

    private void LateUpdate()
    {
        UpdateHeadTracking();
        UpdateEyeTracking();
    }

    private void UpdateHeadTracking()
    {
        Quaternion currentRotation = new Quaternion(m_headBone.localRotation.x, m_headBone.localRotation.y, m_headBone.localRotation.z, m_headBone.localRotation.w);

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

    private void UpdateEyeTracking()
    {
        m_leftEye.rotation = Quaternion.Slerp(
            m_leftEye.rotation,
            Quaternion.LookRotation(m_eyeTarget.position - m_leftEye.position, transform.up),
            1 - Mathf.Exp(-m_eyeTurnSpeed * Time.deltaTime)
        );

        m_rightEye.rotation = Quaternion.Slerp(
            m_rightEye.rotation,
            Quaternion.LookRotation(m_eyeTarget.position - m_leftEye.position, transform.up),
            1 - Mathf.Exp(-m_eyeTurnSpeed * Time.deltaTime)
        );

        // Clamp the rotations
        float leftEyeCurrentYRotation = m_leftEye.localEulerAngles.y;
        float rightEyeCurrentYRotation = m_rightEye.localEulerAngles.y;

        // Move the rotation to a -180 ~ 180 range
        if (leftEyeCurrentYRotation > 180)
        {
            leftEyeCurrentYRotation -= 360;
        }
        if (rightEyeCurrentYRotation > 180)
        {
            rightEyeCurrentYRotation -= 360;
        }

        // Clamp the Y axis rotation
        float leftEyeClampedYRotation = Mathf.Clamp(
            leftEyeCurrentYRotation,
            m_minLeftEyeAngle,
            m_maxLeftEyeAngle
        );
        float rightEyeClampedYRotation = Mathf.Clamp(
            rightEyeCurrentYRotation,
            m_minRightEyeAngle,
            m_maxRightEyeAngle
        );

        // Apply the clamped Y rotation without changing the X and Z rotations
        m_leftEye.localEulerAngles = new Vector3(
            m_leftEye.localEulerAngles.x,
            leftEyeClampedYRotation,
            m_leftEye.localEulerAngles.z
        );
        m_rightEye.localEulerAngles = new Vector3(
            m_rightEye.localEulerAngles.x,
            rightEyeClampedYRotation,
            m_rightEye.localEulerAngles.z
        );
    }
}
