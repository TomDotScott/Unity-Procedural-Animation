using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_movementSpeed = 5.0f;
    [SerializeField] private float m_mouseSensitivity = 3.0f;

    private float m_verticalRotation;
    private float m_horizontalRotation;

    void Update()
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