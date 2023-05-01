using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogNose : MonoBehaviour
{
    [SerializeField] private DogController m_dog;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("STICK"))
        {
            m_dog.SetStick(collider.gameObject);
        }
    }
}
