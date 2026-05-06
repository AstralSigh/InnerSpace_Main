using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientObjectDownwards : MonoBehaviour
{
    void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(this.transform.forward, Vector3.up);
    }
}
