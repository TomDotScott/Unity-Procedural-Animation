using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BipedalController : MonoBehaviour
{
    [SerializeField] private float m_speed;

    [SerializeField] private Stepper m_leftLeg;
    [SerializeField] private Stepper m_rightLeg;

    [SerializeField] private float m_balancingMajorRadius;
    [SerializeField] private float m_balancingMinorRadius;

    [SerializeField] private LineRenderer m_lineRenderer;

    private Vector3 m_lastBodyPosition;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateLegMovement());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private bool IsBalanced()
    {
        // Project our targets onto an ellipse and then work out if we are inside or outside of it
        Vector3 ellipseCentre = (m_leftLeg.transform.position + m_rightLeg.transform.position) / 2f;
        ellipseCentre = Vector3.ProjectOnPlane(ellipseCentre, Vector3.up);

        Vector2 ellipseCentre2D = new Vector2(ellipseCentre.x, ellipseCentre.z);

        Vector3 point = Vector3.ProjectOnPlane(transform.position, Vector3.up);
        Vector3 point2D = new Vector2(point.x, point.z);

        Vector3 footAxis = Vector3.ProjectOnPlane(m_rightLeg.transform.position - m_leftLeg.transform.position, transform.up).normalized;
        Vector2 footAxis2D = new Vector2(footAxis.x, footAxis.z);

        Vector2 rotatedPoint = MathUtils.Rotate(point2D, MathUtils.GetAngle2D(footAxis2D, Vector2.right));

        m_lineRenderer.SetPositions(CalculateEllipse());
        m_lineRenderer.gameObject.transform.rotation = new Quaternion(0, MathUtils.GetAngle2D(point, Vector2.right), 0, 0);

        return MathUtils.IsPointInEllipse(rotatedPoint, ellipseCentre2D, m_balancingMinorRadius, m_balancingMajorRadius);
    }


    private Vector3[] CalculateEllipse()
    {
        Vector3[] ellipsePoints = new Vector3[100];

        for (int i = 0; i < ellipsePoints.Length; i++)
        {
            float angle = (i / 100f) * 360f * Mathf.Deg2Rad;

            float x = Mathf.Sin(angle) * m_balancingMinorRadius;
            float y = Mathf.Cos(angle) * m_balancingMajorRadius;

            ellipsePoints[i] = new Vector3(x, m_leftLeg.transform.position.y, y);
        }

        return ellipsePoints;
    }


    private IEnumerator UpdateLegMovement()
    {
        bool moving = false;
        bool moveLeft = false;
        while (true)
        {
            if (!moving)
            {
                if (!IsBalanced())
                {
                    Vector3 velocity = transform.position - m_lastBodyPosition;
                    Vector3 centreOfMass = transform.position;

                    float leftDistance = Vector3.Distance(m_leftLeg.transform.position, centreOfMass);
                    float rightDistance = Vector3.Distance(m_rightLeg.transform.position, centreOfMass);

                    if (leftDistance >= rightDistance)
                    {
                        moving = true;
                        moveLeft = true;
                    }
                    else
                    {
                        moving = true;
                        moveLeft = false;
                    }
                }
            }
            else
            {
                if (moveLeft)
                {
                    if (!m_rightLeg.Moving)
                    {
                        m_leftLeg.AttemptMove();
                        moving = m_leftLeg.Moving;
                    }
                }
                else
                {
                    if (!m_leftLeg.Moving)
                    {
                        m_rightLeg.AttemptMove();
                        moving = m_rightLeg.Moving;
                    }
                }
            }

            yield return null;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = IsBalanced() ? Color.green : Color.red;


        Gizmos.DrawWireSphere(m_leftLeg.transform.position, 0.2f);
        Gizmos.DrawWireSphere(m_rightLeg.transform.position, 0.2f);

        Debug.DrawLine(transform.position, transform.position + transform.up * 2f);
    }
}
