using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.UI;
using FMOD.Studio;

public class BasicNarrator : MonoBehaviour
{
    //public AudioSource localAudio;
    public StudioEventEmitter localEmitter;
    public NarrationPlaylist currentNarration;
    public FMOD.Studio.EventInstance narrationEvent;
    public GameObject onButton, offButton, notifPing;
    //public List<FMODUnity.EventReference> voiceEvents;

    public Text subtitleText;

    public bool autoNarrate;

    public void SetNewAudio(AudioClip newClip)
    {
        //localAudio.Stop();

        //localAudio.clip = newClip;

        onButton.SetActive(true);
        offButton.SetActive(false);
        notifPing.SetActive(true);
    }

    public void SetNewNarration(int narrationIndex)
    {
        //localEmitter.Stop();
        //localEmitter.EventReference = currentNarration.NarrationBeats[narrationIndex].narrationTrack;
        //localEmitter.Play();

        //FMODUnity.RuntimeManager.PlayOneShot(sceneNarrationRefs.NarrationBeats[narrationIndex].narrationTrack);
        narrationEvent = FMODUnity.RuntimeManager.CreateInstance(currentNarration.NarrationBeats[narrationIndex].narrationTrack);

        onButton.SetActive(true);
        offButton.SetActive(false);
        notifPing.SetActive(true);

        if(subtitleText != null)
        {
            subtitleText.text = currentNarration.NarrationBeats[narrationIndex].narrationText;
        }

        if(autoNarrate)
        {
            PlayNarration();
            notifPing.SetActive(false) ;
        }
    }

    public void SetNewScene(Transform sceneRoot)
    {
        if(sceneRoot.gameObject.GetComponent<StoryBeatNav>() != null)
        {
            currentNarration = sceneRoot.gameObject.GetComponent<StoryBeatNav>().sceneNarration;
        }
    }

    public void PlayNarration()
    {
        narrationEvent.start();
    }

    public void PauseNarration()
    {
        narrationEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void NoNarration()
    {

    }
}
