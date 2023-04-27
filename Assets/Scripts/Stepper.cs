using System.Collections;
using UnityEngine;
using static GameObjectAboveGround;

public class Stepper : MonoBehaviour
{
    [SerializeField] private Transform m_home;
    [SerializeField] private float m_stepDistance;
    [SerializeField] private float m_stepDuration;
    [SerializeField] private float m_stepOvershoot;

    public bool Moving { get; private set; }

    public void AttemptMove()
    {
        if (Moving)
        {
            return;
        }

        float distanceFromHome = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(m_home.position.x, m_home.position.z));

        // If we are too far away, then we should step 
        if (distanceFromHome > m_stepDistance)
        {
            StartCoroutine(MoveFootToHome());
        }
    }

    private IEnumerator MoveFootToHome()
    {
        Moving = true;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        Quaternion endRotation = m_home.rotation;

        Vector3 towardsHomePosition = (m_home.position - transform.position).normalized;

        float overstep = m_stepDistance * m_stepOvershoot;
        Vector3 overshootVector = towardsHomePosition * overstep;

        overshootVector = Vector3.ProjectOnPlane(overshootVector, Vector3.up);

        Vector3 endPosition = m_home.position + overshootVector;

        Vector3 centrePosition = (startPosition + endPosition) / 2;

        centrePosition += m_home.up * Vector3.Distance(startPosition, endPosition) / 2f;

        float timeElapsed = 0f;

        while (timeElapsed < m_stepDuration)
        {
            timeElapsed += Time.deltaTime;

            float normalisedTime = Easing.EaseInOutCubic(timeElapsed / m_stepDuration);

            // Use a quadratic bezier curve to move the foot to make it look convincing
            transform.position = Vector3.Lerp(
                Vector3.Lerp(startPosition, centrePosition, normalisedTime),
                Vector3.Lerp(centrePosition, endPosition, normalisedTime),
                normalisedTime
            );

            transform.rotation = Quaternion.Slerp(startRotation, endRotation, normalisedTime);

            yield return null;
        }

        Moving = false;
    }

    public void SetHomeRotation(Vector3 groundNormal)
    {
        Vector3 newUp = Vector3.Cross(transform.right, groundNormal);

        // If the new "up" direction is zero, it means that the ground normal and right direction are parallel,
        // so we can use the world space up direction instead
        if (newUp == Vector3.zero)
        {
            newUp = Vector3.up;
        }

        transform.rotation = Quaternion.FromToRotation(transform.up, newUp) * transform.rotation;

        // Compensate the X rotation by subbing 90
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x - 90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
}