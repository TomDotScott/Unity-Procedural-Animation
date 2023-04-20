using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectAboveGround : MonoBehaviour
{
    [SerializeField] private float m_minimumDistanceToGround;
    [SerializeField] private float m_footMinimumDistanceToGround;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck(transform, m_minimumDistanceToGround, out RaycastHit bodyHit);

        transform.rotation = Quaternion.LookRotation(
            Vector3.Cross(transform.TransformDirection(Vector3.right), bodyHit.normal),
            bodyHit.normal
        );

    }

    private void GroundCheck(Transform inTransform, float minDistToGround, out RaycastHit hit)
    {
        // Move to be correct orientation relative to the ground
        Debug.DrawRay(inTransform.position, Vector3.down * minDistToGround, Color.yellow, 0.016f);

        if (Physics.Raycast(inTransform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            float distanceToGround = hit.distance;
            if (distanceToGround < minDistToGround)
            {
                float amountToMove = minDistToGround - distanceToGround;
                inTransform.position += new Vector3(0, amountToMove, 0);
            }
            else if (distanceToGround > minDistToGround)
            {
                float amountToMove = distanceToGround - minDistToGround;
                inTransform.position -= new Vector3(0, amountToMove, 0);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, Vector3.down * m_minimumDistanceToGround);

    }
}
