using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : MonoBehaviour
{
    [SerializeField] float m_turnSpeed;
    [SerializeField] float m_moveSpeed;
    [SerializeField] float m_turnAcceleration;
    [SerializeField] float m_moveAcceleration;
    [SerializeField] float m_minimumDistanceToTarget;
    [SerializeField] float m_maxDistToTarget;
    [SerializeField] float m_maxAngleToTarget;

    Vector3 m_currentVelocity;
    float m_currentAngularVelocity;

    private QuadrupedController m_quadrupedController;

    [SerializeField] private Transform m_ownerHead;
    private Transform m_currentTarget;

    [SerializeField] private bool m_hasStick;
    [SerializeField] private bool m_canMove = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_hasStick)
        {
            return;
        }

        m_currentTarget = m_ownerHead;
    }

    private void LateUpdate()
    {
        if (!m_canMove)
        {
            return;
        }

        UpdateMotion();
    }

    void UpdateMotion()
    {
        Vector3 towardTarget = m_currentTarget.position - transform.position;

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
            float distanceToTarget = Vector3.Distance(transform.position, m_currentTarget.position);

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
}
