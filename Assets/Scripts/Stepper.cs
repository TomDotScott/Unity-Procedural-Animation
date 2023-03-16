using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stepper : MonoBehaviour
{
    [SerializeField] private Transform m_home;
    [SerializeField] private float m_stepDistance;
    [SerializeField] private float m_stepDuration;

    public bool Moving { get; private set; }

    // Update is called once per frame
    void Update()
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

        Quaternion startRotation = transform.rotation;
        Vector3 startPosition = transform.position;

        Quaternion endRotation = m_home.rotation;
        Vector3 endPosition = m_home.position;

        float timeElapsed = 0f;

        while (timeElapsed < m_stepDuration)
        {
            timeElapsed += Time.deltaTime;

            float normalisedTime = timeElapsed / m_stepDuration;

            // Interpolate between the start end end positions and rotations
            transform.position = Vector3.Lerp(startPosition, endPosition, normalisedTime);
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, normalisedTime);

            yield return null;
        }

        Moving = false;
    }
}
