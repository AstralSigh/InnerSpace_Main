using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderAnimator : MonoBehaviour
{
    public float holeRadius;
    void Update()
    {
        transform.GetComponent<Renderer>().sharedMaterial.SetFloat("_HoleRadius", holeRadius);
    }
}
