using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAtForRecording : MonoBehaviour
{
    public Transform target;
    public bool rotateAround = false;
    public float time = 0;
    public float radius = 1;
    public float rotationSpeed = 1;
    public float upaxis = 0;
    void Update()
    {
        if (target != null)
        {
            transform.LookAt(target, Vector3.forward);
        }
        if (rotateAround)
        {
            transform.position = target.position + new Vector3(upaxis, Mathf.Sin(2 * Mathf.PI * time) * radius, Mathf.Cos(2 * Mathf.PI * time) * radius);
            time += rotationSpeed / 24f;
        }

    }
}
