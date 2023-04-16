using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogWalk : MonoBehaviour
{
    [SerializeField] private float m_speed;

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + m_speed * Time.deltaTime);
    }
}
