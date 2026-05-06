using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BasicEnableEvent : MonoBehaviour
{

    public UnityEvent onEnableCall;

    public UnityEvent onDisableCall;
    private void OnEnable()
    {
        onEnableCall.Invoke();
    }

    private void OnDisable()
    {
        onDisableCall.Invoke();
    }
}
