using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    EventInstance ambienceInstance;

    public static AudioManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ambienceInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Ambience_engine 2");
        ambienceInstance.setParameterByName("Region", 3); // Play lobby audio. <- this is hardcoded. Change later
        ambienceInstance.start();
    }

    public void SetAudioRegion(int region)
    {
        ambienceInstance.setParameterByName("Region", region);
    }

    public void PlaySceneTransition()
    {
        FMODUnity.RuntimeManager.PlayOneShot2D("event:/UI Events/scene_transition");
    }

    public void EndAmbienceInstance()
    {
        ambienceInstance.stop(STOP_MODE.IMMEDIATE);
    }
}
