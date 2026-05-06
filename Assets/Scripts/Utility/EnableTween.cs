using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTween : MonoBehaviour
{
    public GameObject tweenTarget;
    public Transform tweenDest;
    public iTween.EaseType tweenEasing;
    public iTween.LoopType tweenLoop;

    public bool moveTo;
    public bool scaleTo;
    public bool rotateTo;

    public float tweenTime;
    public float delayTime;
    private void OnEnable()
    {
        if (!tweenTarget)
            tweenTarget = this.gameObject;
        if (!tweenDest)
            tweenDest = this.gameObject.transform;
        if(moveTo)
            iTween.MoveTo(tweenTarget, iTween.Hash("position", tweenDest.position, "time", tweenTime, "easetype", tweenEasing, "looptype", tweenLoop, "delay", delayTime));

        if(scaleTo)
            iTween.ScaleTo(tweenTarget, iTween.Hash("scale", tweenDest.localScale, "time", tweenTime, "easetype", tweenEasing, "looptype", tweenLoop, "delay", delayTime));

        if(rotateTo)
            iTween.RotateTo(tweenTarget, iTween.Hash("rotation", tweenDest.rotation.eulerAngles, "time", tweenTime, "easetype", tweenEasing, "looptype", tweenLoop, "delay", delayTime));

    }
}
