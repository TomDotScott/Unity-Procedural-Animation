using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Stepper : MonoBehaviour
{
    [SerializeField] private Transform m_home;
    [SerializeField] private float m_stepDistance;
    [SerializeField] private float m_stepDuration;
    [SerializeField] private float m_stepOvershoot;
    [SerializeField] private float m_centreHeightDenominator;

    public bool Moving { get; private set; }

    public void AttemptMove()
    {
        if (Moving)
        {
            return;
        }

        float distanceFromHome = Vector3.Distance(transform.position, m_home.position);

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

        Vector3 endPosition = m_home.transform.position + overshootVector;

        Vector3 centrePosition = (startPosition + endPosition) / 2;

        centrePosition += m_home.transform.up * Vector3.Distance(startPosition, endPosition) / m_centreHeightDenominator;

        float timeElapsed = 0f;

        while (timeElapsed < m_stepDuration)
        {
            timeElapsed += Time.deltaTime;

            float normalisedTime = EaseInOutCubic(timeElapsed / m_stepDuration);

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

    public static float EaseInOutCubic(float k)
    {
        if ((k *= 2f) < 1f)
        {
            return 0.5f * k * k * k;
        }

        return 0.5f * ((k -= 2f) * k * k + 2f);
    }
}
