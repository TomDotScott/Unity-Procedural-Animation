using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralBipedalAnimation : MonoBehaviour
{
    private Vector3 m_initialLeftFootPosition;
    private Vector3 m_initialRightFootPosition;

    private Vector3 m_previousLeftFootPosition;
    private Vector3 m_previousRightFootPosition;
    private Vector3 m_previousBodyPosition;


    [SerializeField] private Transform m_leftFootTarget;
    [SerializeField] private Transform m_rightFootTarget;

    [SerializeField] private float m_balancingMinorRadius;
    [SerializeField] private float m_balancingMajorRadius;



    // Start is called before the first frame update
    void Start()
    {
        m_initialLeftFootPosition = m_leftFootTarget.position;
        m_initialRightFootPosition = m_rightFootTarget.position;

        m_previousLeftFootPosition = m_initialLeftFootPosition;
        m_previousRightFootPosition = m_initialRightFootPosition;

        m_previousBodyPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = transform.position - m_previousBodyPosition;
        Vector3 centreOfMass = transform.position;

        m_leftFootTarget.position = m_previousLeftFootPosition;
        m_rightFootTarget.position = m_previousRightFootPosition;

        m_previousLeftFootPosition = m_leftFootTarget.position;
        m_previousRightFootPosition = m_rightFootTarget.position;
    }

    private bool IsBalanced()
    {
        // Project our targets onto an ellipse and then work out if we are inside or outside of it
        Vector3 ellipseCentre = (m_leftFootTarget.position + m_rightFootTarget.position) / 2f;
        ellipseCentre = Vector3.ProjectOnPlane(ellipseCentre, Vector3.up);

        Vector2 ellipseCentre2D = new Vector2(ellipseCentre.x, ellipseCentre.z);

        Vector3 point = Vector3.ProjectOnPlane(transform.position, Vector3.up);
        Vector3 point2D = new Vector2(point.x, point.z);

        Vector3 footAxis = Vector3.ProjectOnPlane(m_rightFootTarget.position - m_leftFootTarget.position, transform.up).normalized;
        Vector2 footAxis2D = new Vector2(footAxis.x, footAxis.z);

        Vector2 rotatedPoint = Rotate2D(point2D, GetAngle2D(footAxis2D, Vector2.right));

        return IsInEllipse(rotatedPoint, ellipseCentre2D, m_balancingMinorRadius, m_balancingMajorRadius);
    }

    private bool IsInEllipse(Vector2 point, Vector2 ellipseCentre, float minorRadius, float majorRadius)
    {
        float major = Mathf.Pow((point.x - ellipseCentre.x) / majorRadius, 2f);

        float minor = Mathf.Pow((point.y - ellipseCentre.y) / minorRadius, 2f);

        return major + minor <= 1f;
    }

    private Vector2 Rotate2D(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    private float GetAngle2D(Vector2 a, Vector2 b)
    {
        float dot = Vector2.Dot(a, b);
        float mags = a.magnitude * b.magnitude;

        return Mathf.Acos(dot / mags);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = IsBalanced() ? Color.green : Color.red;

        Gizmos.DrawWireSphere(m_leftFootTarget.position, 0.2f);
        Gizmos.DrawWireSphere(m_rightFootTarget.position, 0.2f);

        Debug.DrawLine(transform.position, transform.position + transform.up * 2f);
    }
}
