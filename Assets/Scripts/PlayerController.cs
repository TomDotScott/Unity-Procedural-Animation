using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_movementSpeed = 5.0f;
    [SerializeField] private float m_mouseSensitivity = 3.0f;

    [SerializeField] private Transform m_stickHoldTransform;

    private float m_verticalRotation;
    private float m_horizontalRotation;

    [SerializeField] private GameObject m_stick;
    public bool HasStick = true;
    [SerializeField] private DogController m_doggy;

    [Header("Parabola visualisation")]
    private LineRenderer m_lineRenderer;
    [SerializeField] private Vector3 m_maxStickVelocity;
    private Vector3 m_currentStickVelocity;
    [SerializeField] private float m_stickChargeSpeed;
    [SerializeField] private float m_stickLimitY;
    private float GRAVITY;
    [SerializeField] private int m_resolution = 30;
    [SerializeField] private float m_linecastResolution;
    [SerializeField] private float m_stickThrowForce;

    private void Start()
    {
        GRAVITY = Mathf.Abs(Physics.gravity.y);
        m_lineRenderer = GetComponent<LineRenderer>();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && HasStick)
        {
            m_doggy.SetTarget(m_stick.transform);
        }
        else if (Input.GetKey(KeyCode.Mouse0) && HasStick)
        {
            m_currentStickVelocity.y += m_stickChargeSpeed * Time.deltaTime;
            m_currentStickVelocity.z += m_stickChargeSpeed * Time.deltaTime;

            m_currentStickVelocity.y = Mathf.Min(m_currentStickVelocity.y, m_maxStickVelocity.y);
            m_currentStickVelocity.z = Mathf.Min(m_currentStickVelocity.z, m_maxStickVelocity.z);

            RenderParabola();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            ThrowStick();

            m_currentStickVelocity = new Vector3(0, 5, 3);
        }

        if (HasStick)
        {
            Rigidbody stick = m_stick.GetComponent<Rigidbody>();
            stick.useGravity = false;
            stick.velocity = Vector3.zero;

            m_stick.transform.SetPositionAndRotation(m_stickHoldTransform.position, m_stickHoldTransform.rotation);
        }
    }

    private void LateUpdate()
    {
        Move();
    }

    private void ThrowStick()
    {
        HasStick = false;

        Rigidbody stickRb = m_stick.GetComponent<Rigidbody>();

        stickRb.useGravity = true;

        Vector3 force = Camera.main.transform.forward * (m_currentStickVelocity.z * m_stickThrowForce) +
                        transform.up * (m_currentStickVelocity.y * m_stickThrowForce);

        stickRb.AddForce(force, ForceMode.Impulse);
    }

    private void RenderParabola()
    {
        m_lineRenderer.positionCount = m_resolution + 1;
        m_lineRenderer.SetPositions(CalculateLineArray());
    }

    private Vector3[] CalculateLineArray()
    {
        Vector3[] lineArray = new Vector3[m_resolution + 1];

        float lowestTimeValueX = MaxTimeX() / m_resolution;
        float lowestTimeValueZ = MaxTimeZ() / m_resolution;
        float lowestTimeValue = lowestTimeValueX > lowestTimeValueZ ? lowestTimeValueZ : lowestTimeValueX;

        for (int i = 0; i < lineArray.Length; i++)
        {
            float t = lowestTimeValue * i;
            lineArray[i] = CalculateLinePoint(t);
        }

        return lineArray;
    }

    private Vector3 CalculateLinePoint(float t)
    {
        float x = m_currentStickVelocity.x * t;
        float z = m_currentStickVelocity.z * t;
        float y = m_currentStickVelocity.y * t - (GRAVITY * Mathf.Pow(t, 2) / 2);
        return new Vector3(x + transform.position.x, y + transform.position.y, z + transform.position.z);
    }

    private float MaxTimeY()
    {
        float v = m_currentStickVelocity.y;
        float vv = v * v;

        float t = (v + Mathf.Sqrt(vv + 2 * GRAVITY * (transform.position.y - m_stickLimitY))) / GRAVITY;
        return t;
    }

    private float MaxTimeX()
    {
        if (MathUtils.IsValueAlmostZero(m_currentStickVelocity.x))
        {
            MathUtils.SetValueToAlmostZero(ref m_currentStickVelocity.x);
        }

        float x = m_currentStickVelocity.x;

        float t = (HitPosition().x - transform.position.x) / x;
        return t;
    }

    private float MaxTimeZ()
    {
        if (MathUtils.IsValueAlmostZero(m_currentStickVelocity.z))
        {
            MathUtils.SetValueToAlmostZero(ref m_currentStickVelocity.z);
        }

        float z = m_currentStickVelocity.z;

        float t = (HitPosition().z - transform.position.z) / z;
        return t;
    }

    private Vector3 HitPosition()
    {
        float lowestTimeValue = MaxTimeY() / m_linecastResolution;

        for (int i = 0; i < m_linecastResolution + 1; i++)
        {
            float t = lowestTimeValue * i;
            float tt = lowestTimeValue * (i + 1);

            if (Physics.Linecast(CalculateLinePoint(t), CalculateLinePoint(tt), out RaycastHit rayHit))
            {
                return rayHit.point;
            }
        }

        return CalculateLinePoint(MaxTimeY());
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