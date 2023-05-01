using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_movementSpeed = 5.0f;
    [SerializeField] private float m_mouseSensitivity = 3.0f;

    [SerializeField] private Transform m_stickHoldTransform;

    private float m_verticalRotation;
    private float m_horizontalRotation;

    [SerializeField] private GameObject m_stick;
    private bool m_hasStick = true;
    [SerializeField] private float m_throwForce;
    [SerializeField] private float m_throwUpwardForce;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && m_hasStick)
        {
            ThrowStick();
        }

        if (m_hasStick)
        {
            m_stick.transform.SetPositionAndRotation(m_stickHoldTransform.position, m_stickHoldTransform.rotation);
        }
    }

    private void LateUpdate()
    {
        Move();
    }

    private void ThrowStick()
    {
        m_hasStick = false;

        Rigidbody stickRb = m_stick.GetComponent<Rigidbody>();

        Vector3 force = Camera.main.transform.forward * m_throwForce + transform.up * m_throwUpwardForce;
        stickRb.AddForce(force, ForceMode.Impulse);
    }


    private void Move()
    {
        // Mouse rotation
        m_horizontalRotation += Input.GetAxis("Mouse X") * m_mouseSensitivity;

        transform.rotation = Quaternion.Euler(0.0f, m_horizontalRotation, 0.0f);

        // Keyboard movement
        float forwardSpeed = Input.GetAxis("Vertical") * m_movementSpeed;
        float sideSpeed = Input.GetAxis("Horizontal") * m_movementSpeed;

        Vector3 speed = new Vector3(sideSpeed, 0, forwardSpeed);
        speed = transform.rotation * speed;

        transform.position += speed * Time.deltaTime;
    }
}