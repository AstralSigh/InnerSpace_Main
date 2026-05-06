using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnableEvent : MonoBehaviour
{
    public UnityEvent OnObjEnabled;
    public UnityEvent OnObjDisabled;

    private void OnEnable()
    {
        OnObjEnabled.Invoke();
    }

    private void OnDisable()
    {
        OnObjDisabled.Invoke();
    }


}
