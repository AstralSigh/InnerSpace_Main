using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    private PlayableDirector currentDirector;
    [SerializeField] private TimeWheelManager timeWheelManager;

    void Awake()
    {
        Instance = this;
    }

    public void SetPlaySpeed(float newSpeed)
    {
        currentDirector.RebuildGraph();
        currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(newSpeed);
    }

    //Called from Unity Event on BasicButton on playButton gameObject 
    public void TogglePlay()
    {
        SetPlayState(!(currentDirector.state == PlayState.Playing));
    }

    //Pauses or plays the animation
    public void SetPlayState(bool playing)
    {
        if(currentDirector == null) return;

        if (playing)
        {
            currentDirector.Play();
        }
        else
        {
            currentDirector.Pause();
        }
        timeWheelManager.UpdatePlayButtonUI(playing);
    }

    //Called by TimeWheelManager when scrubbing
    public void SetTime(float time)
    {
        currentDirector.time = time;
        currentDirector.RebuildGraph();
        currentDirector.Evaluate();
    }

    //Set through Quest Manager everytime player enters a nexi.
    public void SetCurrentDirector(PlayableDirector currentDirector)
    {   
        this.currentDirector = currentDirector;
        currentDirector.RebuildGraph();
    }

    public float GetCurrentTime()
    {
        if(currentDirector == null)
        {
            return 0;
        }
        return (float)currentDirector.time;
    }

    public float GetCurrentDuration()
    {
        if (currentDirector == null)
        {
            return 0;
        }
        return (float)currentDirector.duration;
    }

    public bool GetCurrentPlayState()
    {
        return currentDirector.state == PlayState.Playing; 
    }
}
