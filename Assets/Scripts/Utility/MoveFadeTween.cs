using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFadeTween : MonoBehaviour
{
    public GameObject moveTarget;
    public Transform moveDest;
    public GameObject fadeTarget;
    public float fadeValue;

    public float animTime;
    public float delayTime;
    private void OnEnable()
    {
        iTween.MoveTo(moveTarget, iTween.Hash("time", animTime, "position", moveDest.position, "looptype", iTween.LoopType.loop, "easetype", iTween.EaseType.easeInSine, "delay", delayTime));
        iTween.FadeTo(fadeTarget, iTween.Hash("time", animTime, "amount", fadeValue, "looptype", iTween.LoopType.loop, "easetype", iTween.EaseType.easeInSine, "delay", delayTime));
    }
}
