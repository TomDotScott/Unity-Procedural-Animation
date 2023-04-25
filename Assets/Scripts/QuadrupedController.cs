using System.Collections;
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

    [SerializeField] float m_turnSpeed;
    [SerializeField] float m_moveSpeed;
    [SerializeField] float m_turnAcceleration;
    [SerializeField] float m_moveAcceleration;
    [SerializeField] float m_minimumDistanceToTarget;
    [SerializeField] float m_maxDistToTarget;
    [SerializeField] float m_maxAngleToTarget;

    Vector3 m_currentVelocity;
    float m_currentAngularVelocity;


    [SerializeField] private Stepper m_frontLeftStepper;
    [SerializeField] private Stepper m_frontRightStepper;
    [SerializeField] private Stepper m_backLeftStepper;
    [SerializeField] private Stepper m_backRightStepper;

    [SerializeField] private bool m_canMove;

    private void Awake()
    {
        StartCoroutine(UpdateLegMovement());
    }

    private void LateUpdate()
    {
        if (m_canMove)
        {
            UpdateMotion();
        }

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
        towardTargetLocalSpace = Vector3.RotateTowards(
            Vector3.forward,
            towardTargetLocalSpace,
            Mathf.Deg2Rad * m_maxHeadAngle,
            0
        );

        Quaternion targetRotation = Quaternion.LookRotation(towardTargetLocalSpace, Vector3.up);

        // Smooth the rotation
        m_headBone.localRotation = Quaternion.Slerp(
            currentRotation,
            targetRotation,
            1 - Mathf.Exp(-m_headTurnSpeed * Time.deltaTime)
        );
    }

    private void UpdateEyeTracking()
    {
        Quaternion targetEyeRotation = Quaternion.LookRotation(
            m_eyeTarget.position - m_headBone.position, // toward target
            transform.up
        );

        m_leftEye.rotation = Quaternion.Slerp(
            m_leftEye.rotation,
            targetEyeRotation,
            1 - Mathf.Exp(-m_eyeTurnSpeed * Time.deltaTime)
        );

        m_rightEye.rotation = Quaternion.Slerp(
            m_rightEye.rotation,
            targetEyeRotation,
            1 - Mathf.Exp(-m_eyeTurnSpeed * Time.deltaTime)
        );

        // Clamp the rotations
        float leftEyeCurrentYRotation = m_leftEye.localEulerAngles.y;
        float rightEyeCurrentYRotation = m_rightEye.localEulerAngles.y;

        // Move the rotation to a -180 -> 180 range
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

    void UpdateMotion()
    {
        Vector3 towardTarget = m_eyeTarget.position - transform.position;

        Vector3 towardTargetProjected = Vector3.ProjectOnPlane(towardTarget, transform.up);

        float angleToTarget = Vector3.SignedAngle(transform.forward, towardTargetProjected, transform.up);

        float targetAngularVelocity = 0;

        // If we are within the max angle (i.e. facing the target) leave the target angular velocity at zero
        if (Mathf.Abs(angleToTarget) > m_maxAngleToTarget)
        {
            // Angles in Unity are clockwise, so a positive angle here means to our right
            if (angleToTarget > 0)
            {
                targetAngularVelocity = m_turnSpeed;
            }
            // Invert angular speed if target is to our left
            else
            {
                targetAngularVelocity = -m_turnSpeed;
            }
        }

        // Smooth the velocity over time
        m_currentAngularVelocity = Mathf.Lerp(
            m_currentAngularVelocity,
            targetAngularVelocity,
            1 - Mathf.Exp(-m_turnAcceleration * Time.deltaTime)
        );

        transform.Rotate(
            0,
            Time.deltaTime * m_currentAngularVelocity,
            0,
            Space.World
        );

        Vector3 targetVelocity = Vector3.zero;

        // If we are facing opposite to the target, spin in place
        if (Mathf.Abs(angleToTarget) < 90)
        {
            float distanceToTarget = Vector3.Distance(transform.position, m_eyeTarget.position);

            // If we're too far away, approach the target
            if (distanceToTarget > m_maxDistToTarget)
            {
                targetVelocity = m_moveSpeed * towardTargetProjected.normalized;
            }
            // If we're too close, reverse the direction and move away
            else if (distanceToTarget < m_minimumDistanceToTarget)
            {
                targetVelocity = m_moveSpeed * -towardTargetProjected.normalized;
            }
        }

        m_currentVelocity = Vector3.Lerp(
            m_currentVelocity,
            targetVelocity,
            1 - Mathf.Exp(-m_moveAcceleration * Time.deltaTime)
        );

        // Apply the velocity
        transform.position += m_currentVelocity * Time.deltaTime;
    }

    private IEnumerator UpdateLegMovement()
    {
        while (true)
        {
            // Move the legs in diagonal pairs, to give the illusion of the quadruped walking correctly
            do
            {
                m_frontLeftStepper.AttemptMove();
                m_backRightStepper.AttemptMove();

                yield return null; // waits a frame...
            } while (m_backRightStepper.Moving || m_frontLeftStepper.Moving);

            do
            {
                m_frontRightStepper.AttemptMove();
                m_backLeftStepper.AttemptMove();

                yield return null;
            } while (m_backLeftStepper.Moving || m_frontRightStepper.Moving);
        }
    }
}
