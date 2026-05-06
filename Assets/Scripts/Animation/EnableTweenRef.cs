using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTweenRef : MonoBehaviour
{
    [SerializeField] private GameObject animTarget;

    [SerializeField] private Transform refTransform1, refTransform2;

    [SerializeField] private Vector3 targetPos, targetScale, targetRot;

    [SerializeField] private bool tweenPos = true, tweenScale = true, tweenRot = true;
    [SerializeField] private iTween.EaseType posEasing, scaleEasing, rotEasing;

    [SerializeField] private float animTime;    

    private void OnEnable()
    {
        if (tweenPos)
            TweenTargetPos();

        if (tweenScale)
            TweenTargetScale();

        if (tweenRot)
            TweenTargetRot();
    }

    private void OnDisable()
    {
        if (refTransform1)
        {
            iTween.MoveTo(animTarget, iTween.Hash("time", animTime, "position", refTransform1.position, "easetype", posEasing));
            iTween.ScaleTo(animTarget, iTween.Hash("time", animTime, "scale", refTransform1.localScale, "easetype", scaleEasing));
            iTween.RotateTo(animTarget, iTween.Hash("time", animTime, "rotation", refTransform1.localRotation.eulerAngles, "easetype", rotEasing));
        }

    }

    public void TweenTargetFull()
    {
        TweenTargetPos();
        TweenTargetScale();
        TweenTargetRot();
    }

    public void TweenTargetPos()
    {
        if (refTransform2)
        {
            iTween.MoveTo(animTarget, iTween.Hash("time", animTime, "position", refTransform2.position, "easetype", posEasing));
        }
        else
        {
            iTween.MoveTo(animTarget, iTween.Hash("time", animTime, "position", targetPos, "easetype", posEasing));
        }



    }

    public void TweenTargetScale()
    {
        if (refTransform2)
        {
            iTween.ScaleTo(animTarget, iTween.Hash("time", animTime, "scale", refTransform2.localScale, "easetype", scaleEasing));
        }
        else
        {
            iTween.ScaleTo(animTarget, iTween.Hash("time", animTime, "scale", targetScale, "easetype", scaleEasing));
        }
    }

    public void TweenTargetRot()
    {
        if (refTransform2)
        {
            iTween.RotateTo(animTarget, iTween.Hash("time", animTime, "rotation", refTransform2.localRotation.eulerAngles, "easetype", rotEasing));
        }
        else
        {
            iTween.RotateTo(animTarget, iTween.Hash("time", animTime, "rotation", targetRot, "easetype", rotEasing));
        }
    }
}
