using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pendulum : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float maxAngle = 30.0f;

    private float angle;

    void Start()
    {
        angle = maxAngle; // Inicializamos el ángulo al ángulo máximo
    }

    void Update()
    {
        angle = maxAngle * Mathf.Sin(Time.time * speed);
        transform.localRotation = Quaternion.Euler(angle, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
    }
}