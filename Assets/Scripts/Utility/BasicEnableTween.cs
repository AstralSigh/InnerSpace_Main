using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnableTween : MonoBehaviour
{
    public GameObject goTarget;
    private void OnEnable()
    {
        if (!goTarget)
            goTarget = this.gameObject;
        
        iTween.ScaleFrom(goTarget, iTween.Hash("time", 2.0f, "scale", Vector3.zero, "easetype", iTween.EaseType.easeOutElastic));
    }

}
