using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTweenTo : MonoBehaviour
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

    public bool onEnable;

    private void OnEnable()
    {
        if (onEnable)
            TweenTo();
    }

    public void TweenTo()
    {
        if (!tweenTarget)
            tweenTarget = this.gameObject;
        if (!tweenDest)
            tweenDest = this.gameObject.transform;
        if (moveTo)
            iTween.MoveTo(tweenTarget, iTween.Hash("position", tweenDest.position, "time", tweenTime, "easetype", tweenEasing, "looptype", tweenLoop, "delay", delayTime));

        if (scaleTo)
            iTween.ScaleTo(tweenTarget, iTween.Hash("scale", tweenDest.localScale, "time", tweenTime, "easetype", tweenEasing, "looptype", tweenLoop, "delay", delayTime));

        if (rotateTo)
            iTween.RotateTo(tweenTarget, iTween.Hash("rotation", tweenDest.rotation.eulerAngles, "time", tweenTime, "easetype", tweenEasing, "looptype", tweenLoop, "delay", delayTime));
    }

    public void TweenToTransform(Transform targetTrans)
    {
        tweenDest = targetTrans;
        
        if (!tweenTarget)
            tweenTarget = this.gameObject;
        if (moveTo)
            iTween.MoveTo(tweenTarget, iTween.Hash("position", tweenDest.position, "time", tweenTime, "easetype", tweenEasing, "looptype", tweenLoop, "delay", delayTime));

        if (scaleTo)
            iTween.ScaleTo(tweenTarget, iTween.Hash("scale", tweenDest.localScale, "time", tweenTime, "easetype", tweenEasing, "looptype", tweenLoop, "delay", delayTime));

        if (rotateTo)
            iTween.RotateTo(tweenTarget, iTween.Hash("rotation", tweenDest.rotation.eulerAngles, "time", tweenTime, "easetype", tweenEasing, "looptype", tweenLoop, "delay", delayTime));
    }
}
