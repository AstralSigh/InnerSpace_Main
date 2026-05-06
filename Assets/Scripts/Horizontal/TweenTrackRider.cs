using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TweenTrackRider : MonoBehaviour
{
    public Transform[] trackPoints;
    public GameObject riderRoot;
    public float trackProgress;
    public float trackSpeed = 0.001f;
    public Transform dropPoint;

    private void Update()
    {
        /*
        if (Input.GetKey(KeyCode.Space) && trackProgress < 1.0f)
        {
            TrackForwards();
        }

        if (Input.GetKeyUp(KeyCode.Space) && trackProgress > 0.99f)
        {
            RevealDestination();
        }

        if (Input.GetKey(KeyCode.LeftControl) && trackProgress > trackSpeed)
        {
            TrackBackwards();
        }
        */

        if(Keyboard.current.spaceKey.isPressed && trackProgress < 1.0f)
        {
            TrackForwards();
        }

        if(Keyboard.current.spaceKey.wasReleasedThisFrame && trackProgress > 0.99f)
        {
            RevealDestination();
        }

        if(Keyboard.current.leftCtrlKey.isPressed && trackProgress > trackSpeed)
        {
            TrackBackwards();
        }

    }
    public void TrackForwards()
    {
        trackProgress += trackSpeed;
        iTween.PutOnPath(riderRoot, trackPoints, trackProgress);
    }

    public void TrackBackwards()
    {
        trackProgress -= trackSpeed;
        iTween.PutOnPath(riderRoot, trackPoints, trackProgress);
    }

    public void DropOff()
    {
        iTween.MoveTo(riderRoot, iTween.Hash("position", dropPoint.position, "time", 1.5f, "easetype", iTween.EaseType.easeOutBack));
    }

    public void RevealDestination()
    {
        if(!dropPoint.gameObject.activeSelf)
            dropPoint.gameObject.SetActive(true);
    }
}
