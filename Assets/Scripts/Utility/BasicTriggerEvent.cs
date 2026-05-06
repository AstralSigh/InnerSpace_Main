using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using BNG;

public class BasicTriggerEvent : MonoBehaviour
{
    [SerializeField]
    private string triggeringTag;
    
    public UnityEvent onEnteringTrigger;

    public UnityEvent onExitingTrigger;
    

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == triggeringTag)
        {
            onEnteringTrigger.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == triggeringTag)
        {
            onExitingTrigger.Invoke();

        }
    }
}
