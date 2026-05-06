using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnableAnimTrigger : MonoBehaviour
{
    [SerializeField] private Animator targetAnimator;
    [SerializeField] private string triggerWords;
    private void OnEnable()
    {
        SendTrigger(triggerWords);
    }

    public void SendTrigger(string triggerName)
    {
        if (targetAnimator)
        {
            targetAnimator.SetTrigger(triggerName);
        }
    }
}
