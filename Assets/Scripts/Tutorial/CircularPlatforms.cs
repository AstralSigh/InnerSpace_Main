using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CircularPlatforms : MonoBehaviour
{
    public TextureManager platformUI;
    public enum PlatformStates { idle, active, complete };
    public PlatformStates currentPlatformState;
    public UnityEvent platformActivated;
    public UnityEvent platformCompleted;
    public bool visualDebug;

    private void Update()
    {
        if (visualDebug)
        {
            platformUI.UpdateTexture((int)currentPlatformState);
            visualDebug = false;
        }
    }

    //Activates the platform when user enters for the first time.
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(currentPlatformState == PlatformStates.idle)
            {
                currentPlatformState = PlatformStates.active;
                platformActivated.Invoke();
                //FMODUnity.RuntimeManager.PlayOneShot2D("event:/UI Events/hydro_select_oneshot");
                platformUI.UpdateTexture((int)currentPlatformState);
            }           
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if(currentPlatformState == PlatformStates.active)
            {
                currentPlatformState = PlatformStates.complete;
                platformCompleted.Invoke();
                platformUI.UpdateTexture((int)currentPlatformState);
            }
        }
    }
}
