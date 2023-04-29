using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BipedalController : MonoBehaviour
{
    [SerializeField] private float m_speed;

    [SerializeField] private Stepper m_leftLeg;
    [SerializeField] private Stepper m_rightLeg;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateLegMovement());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator UpdateLegMovement()
    {
        while (true)
        {
            if (!m_leftLeg.Moving)
            {
                m_rightLeg.AttemptMove();
            }

            if (!m_rightLeg.Moving)
            {
                m_leftLeg.AttemptMove();
            }

            yield return null;
        }
    }
}
