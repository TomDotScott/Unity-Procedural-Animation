using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseKinematic : MonoBehaviour
{
    [SerializeField] private int m_chainLength;

    [SerializeField] private Transform m_target;
    [SerializeField] private Transform m_pole;

    [SerializeField] private int m_iterations;

    [SerializeField] private float m_delta;

    [SerializeField] private float m_snapBackStrength;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
