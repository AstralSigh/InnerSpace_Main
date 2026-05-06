using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Transform lookAt;
    // Start is called before the first frame update
    void Start()
    {
        lookAt = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (lookAt)
        {
            transform.LookAt(Camera.main.transform);
        }
        else if (Camera.main != null)
        {
            lookAt = Camera.main.transform;
        }
        else if (Camera.main == null)
        {
            return;
        }
    }
}
