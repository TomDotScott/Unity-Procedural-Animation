using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float m_sensitivity = 100.0f;
    [SerializeField] private float m_maxYAngle = 80.0f;
    [SerializeField] private float m_minYAngle = -80.0f;

    private float m_mouseX;
    private float m_mouseY;
    private float m_yRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        m_mouseX = Input.GetAxis("Mouse X") * m_sensitivity * Time.deltaTime;
        m_mouseY = Input.GetAxis("Mouse Y") * m_sensitivity * Time.deltaTime;

        m_yRotation -= m_mouseY;
        m_yRotation = Mathf.Clamp(m_yRotation, m_minYAngle, m_maxYAngle);

        transform.localRotation = Quaternion.Euler(m_yRotation, 0.0f, 0.0f);
        transform.parent.Rotate(Vector3.up * m_mouseX);
    }
}