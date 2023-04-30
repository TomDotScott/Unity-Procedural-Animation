using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameObjectAboveGround : MonoBehaviour
{
    [SerializeField] private float m_minimumDistanceToGround;
    [SerializeField] private Transform m_front;
    [SerializeField] private Transform m_back;
    [SerializeField] private List<Foot> m_feet;
    [SerializeField] private float m_rotationSpeed;
    [SerializeField] private float m_heightAdjustmentSpeed;
    [SerializeField] private float m_footOffset;

    [SerializeField] private bool m_orientateBody = true;

    [System.Serializable]
    public class Foot
    {
        public Stepper Stepper;
        public float MinDistanceToGround;
        public bool IsFront;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Physics.Raycast(m_front.position, Vector3.down, out RaycastHit frontHit, Mathf.Infinity);
        Physics.Raycast(m_back.position, Vector3.down, out RaycastHit backHit, Mathf.Infinity);

        if (m_orientateBody)
        {
            // Calculate the average normal of the two raycasts
            Vector3 averageNormal = ((frontHit.normal + backHit.normal) * 0.5f).normalized;

            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, averageNormal) * transform.rotation;

            // rotate towards the new rotation
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                m_rotationSpeed * Time.deltaTime
            );
        }

        transform.position = Vector3.Slerp(
                transform.position,
                CalculateNewPosition(transform, m_minimumDistanceToGround,
                    Mathf.Max(frontHit.distance, backHit.distance)),
                m_heightAdjustmentSpeed * Time.deltaTime
            );


        foreach (var foot in m_feet.Where(foot => !foot.Stepper.Moving))
        {
            float newY = foot.IsFront ? frontHit.point.y : backHit.point.y;


            foot.Stepper.transform.position = new Vector3(
                foot.Stepper.transform.position.x,
                newY + (foot.MinDistanceToGround * m_footOffset),
                foot.Stepper.transform.position.z
            );


            GroundCheck(foot.Stepper.transform, foot.MinDistanceToGround, out RaycastHit hit);

            foot.Stepper.SetHomeRotation(hit.normal);

            foot.Stepper.transform.position = CalculateNewPosition(foot.Stepper.transform, foot.MinDistanceToGround, hit.distance);
        }

    }

    private void GroundCheck(Transform inTransform, float minDistToGround, out RaycastHit hit)
    {
        // Move to be correct orientation relative to the ground
        Debug.DrawRay(inTransform.position, Vector3.down * minDistToGround, Color.yellow, 0.016f);

        Physics.Raycast(inTransform.position, Vector3.down, out hit, Mathf.Infinity);
    }

    private Vector3 CalculateNewPosition(Transform inTransform, float minDistToGround, float distanceToGround)
    {
        float amountToMove;

        if (distanceToGround < minDistToGround)
        {
            amountToMove = minDistToGround - distanceToGround;
            return inTransform.position + new Vector3(0, amountToMove, 0);
        }


        amountToMove = distanceToGround - minDistToGround;
        return inTransform.position - new Vector3(0, amountToMove, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, Vector3.down * m_minimumDistanceToGround);
        Gizmos.DrawRay(m_front.position, Vector3.down * 100f);
        Gizmos.DrawRay(m_back.position, Vector3.down * 100f);

        //Gizmos.color = Color.cyan;
        //foreach (var foot in m_feet)
        //{
        //    Gizmos.DrawRay(foot.Stepper.position, Vector3.down * foot.MinDistanceToGround);
        //}
    }
}
