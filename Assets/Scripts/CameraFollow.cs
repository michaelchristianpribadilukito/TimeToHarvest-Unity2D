using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    Vector3 cameraOffset;

    private void Start()
    {
        cameraOffset = transform.position - target.position;
    }

    private void FixedUpdate()
    {
        transform.position = target.position + cameraOffset;
    }
}
