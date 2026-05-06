using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

[ExecuteInEditMode]
public class RadialChild : MonoBehaviour
{
    public float fDistance;
    [Range(0f, 360f)]
    public float fAngle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vPos = new Vector3(Mathf.Cos(fAngle * Mathf.Deg2Rad), Mathf.Sin(fAngle * Mathf.Deg2Rad), 0);
        this.transform.localPosition = vPos * fDistance;
    }
}
