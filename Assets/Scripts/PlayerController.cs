using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_speed;

    private Rigidbody m_rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_rigidBody.velocity.magnitude <= m_speed)
        {
            float input = Input.GetAxis("Vertical");

            m_rigidBody.AddForce(0, 0, input * Time.fixedDeltaTime * 1000f);
        }
    }
}
