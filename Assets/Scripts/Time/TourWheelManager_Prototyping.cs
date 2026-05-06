using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
using UnityEngine.UI;
using UnityEngine.Splines;
using UnityEngine.XR.OpenXR.Input;
using DG.Tweening;
using HighlightPlus;
using static UnityEngine.ParticleSystem;
using FMOD.Studio;
using TMPro;
using static TourWheelUI_Prototyping;

public class TourWheelManager_Prototyping : MonoBehaviour
{
    //USER SETTINGS
    private List<TourStop_Prototyping> tourBeats;
    [SerializeField] SplineData splineData;

    //REFERENCES
    [SerializeField] private GameObject tourWheel;
    [SerializeField] private Text tourText;
    [SerializeField] private Text tourIndex;
    [SerializeField] private TMP_Text tourIndicatorIndex;
    [SerializeField] private SplineAnimate tourIndicator; // This tracks the position of where the player is and can jump on and off of the tour.
    [SerializeField] private SplineAnimate playerSpline;
    [SerializeField] private SplineAnimate platformSpline;
    [SerializeField] public TourWheelUI_Prototyping tourUI;

    //REFACTOR LATER
    [SerializeField] private List<GameObject> tourStopMesh;
    [SerializeField] private CharacterController playerController;

    //VARIABLES 
    private float currentTime = 0;
    public int currentBeat = 1;
    private FMOD.Studio.EventInstance voiceoverEvent;
    public bool mute = true; 
    Coroutine voiceOverCoroutine;
    Coroutine animationCoroutine;
    Coroutine sequenceCoroutine;

    void Start(){
        
      
        // Subscribe to events.
        WIAC_Manager.Instance.OnToggleNexusTour += ToggleTour;
    }


    //TO DO... MUTE AND UNMUTE FEATURES HAVE TOO MANY HARDCODED LINES. IT'S A MESS. 


    void ToggleTour(bool nexusTourActive)
    {
        if (splineData == null)
        {
            Debug.Log("Nexus Tour did not properly setup. splineData is null");
            return;
        }
        
        if (nexusTourActive) 
        {
            //Gets all the tour beat information
            tourBeats = splineData.GetTourStops();

            //UPDATE PLAYER STATE
            PlayerManager.Instance.UpdatePlayerState(PlayerManager.PlayerState.NexusTour);

            TimeManager.Instance.SetPlayState(false);
            TimeManager.Instance.SetTime(currentTime);

            // Deal with UI.
            tourWheel.SetActive(true);
            SetupTourWheelUI();

            // Teleport and lock player to the spline.
            PlayerManager.Instance.SetJetForce(0);
            PlayerManager.Instance.SplineAnimateEnabled(true);
            PlayerManager.Instance.transform.localPosition = Vector3.zero;

            // Deactivate indicator.
            tourIndicator.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = false;
            tourIndicator.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            tourIndicatorIndex.gameObject.SetActive(false);

            // Activate any annotations.
            ToggleAnnotations(true);

            JumpToBeat(currentBeat);
            //NOTE: SPLINE ANIMATE HAS TO BE SWITCHED ACTIVE ON START FOR SOME REASON. 
        }
        else {
            //UPDATE PLAYER STATE
            PlayerManager.Instance.UpdatePlayerState(PlayerManager.PlayerState.Nexus);

            // Deal with UI.
            tourWheel.SetActive(false);

            // Unlock player from spline.
            PlayerManager.Instance.SetJetForce(WIAC_Manager.Instance.currentNexusManager.GetCurrentJetForce());
            PlayerManager.Instance.SplineAnimateEnabled(false);

            // Activate indicator.
            tourIndicator.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = true;
            tourIndicator.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
            tourIndicatorIndex.gameObject.SetActive(true);

            // Deactivate annotations.
            ToggleAnnotations(false);

            if(sequenceCoroutine != null)
            {
                StopCoroutine(sequenceCoroutine);
            }

            if(animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }
            TimeManager.Instance.SetPlayState(false);

            if(voiceOverCoroutine!= null)
            {
                StopCoroutine(voiceOverCoroutine);
                voiceoverEvent.release();
                voiceoverEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                tourUI.SetAudioPlayButton(0);
            }
        }
    }

    //First time you enter the tour, spline animate doesn't work. For some reason it works if you wait one frame???????? Can't figure it out. - Nick 11/14/2023
    IEnumerator WaitOneFrame()
    {
        yield return new WaitForEndOfFrame();
        JumpToBeat(currentBeat);
    }

    void SetupTourWheelUI()
    {
        tourUI.CreateJumpToBeatButtons();
        tourUI.SetJumpOffButton(0);
        tourUI.SetReplayButton(TourWheelUI_Prototyping.ButtonState.inactive);
        tourUI.SetAudioPlayButton(AudioButtonState.mute);
    }

    void ToggleAnnotations(bool active)
    {
        foreach (GameObject g in tourBeats[currentBeat].annotation)
        {
            g.SetActive(active);
        }
        foreach (HighlightEffect g in tourBeats[currentBeat].highlight)
        {
            g.highlighted = active;
        }
    }

    
    public void SelectJumpOffButton()
    {
        WIAC_Manager.Instance.ToggleNexusTour(false);
    }

    /// <summary>
    /// Mute or unmute voiceover audio. Called by the Audio Button
    /// </summary>
    public void ToggleMute()
    {         
        mute = !mute; 

        if(mute)
        {
            tourUI.SetAudioPlayButton(TourWheelUI_Prototyping.AudioButtonState.mute);
            if(voiceOverCoroutine != null)
            {
                StopCoroutine(voiceOverCoroutine);
                voiceoverEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                if(animationCoroutine == null)
                {
                    tourUI.SetReplayButton(TourWheelUI_Prototyping.ButtonState.inactive);
                }
            }
        }
        else
        {
            tourUI.SetAudioPlayButton(TourWheelUI_Prototyping.AudioButtonState.unmute);
        }
    }


    /// <summary>
    /// Mute or unmute voiceover audio. Called by the Audio Button
    /// </summary>
    public void SelectReplayButton()
    {
        if(sequenceCoroutine!= null)
        {
            StopCoroutine(sequenceCoroutine);
        }
        if(animationCoroutine!= null)
        {
            StopCoroutine(animationCoroutine);
        }
        if(voiceOverCoroutine!= null)
        {
            StopCoroutine(voiceOverCoroutine);
        }
        sequenceCoroutine = StartCoroutine(PlaySequence());
    }

    //Currently removed from gameobject to isolate bug
    public void HoverOnReplayButton(bool onHover)
    {
        if (animationCoroutine == null && voiceOverCoroutine == null)
        {
            if (onHover)
            {
                tourUI.SetReplayButton(TourWheelUI_Prototyping.ButtonState.hover);
            }
            else
            {
                tourUI.SetReplayButton(TourWheelUI_Prototyping.ButtonState.inactive);
            }
        }
    }

    //IF YOU HIT MUTE 
    //STOP VOICE OVER COROUTINE 
    //

    public IEnumerator PlaySequence()
    {
        float time = 0;
        //SET DURATION AS ANIMATION LENGTH 
        float duration = (tourBeats[currentBeat].animationEndTime - tourBeats[currentBeat - 1].animationEndTime) / (tourBeats[currentBeat].animationPlaySpeed);

        //SET ICON ACTIVE 
        tourUI.SetReplayButton(TourWheelUI_Prototyping.ButtonState.activate);

        //PLAY ANIMATION 
        animationCoroutine = StartCoroutine(PlayAnimation(duration));

        if (!mute)
        {
            //GET VO DURATION 
            voiceoverEvent.getDescription(out EventDescription eventDescription);
            eventDescription.getLength(out int miliseconds);
            float voDuration = miliseconds / 1000f;

            //START VOICE OVER 
            voiceOverCoroutine = StartCoroutine(PlayVoiceOver(voDuration));

            //UPDATES DURATION IF voDuration IS LONGER THAN ANIMATION DURATION
            if(voDuration > duration)
            {
                duration = voDuration; 
            }
        }
        //SET ICON INACTIVE 
        while(time < duration)
        {
            tourUI.UpdateProgressBar();
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        tourUI.SetReplayButton(TourWheelUI_Prototyping.ButtonState.inactive);
        tourUI.UpdateProgressBar();
    }



    public IEnumerator PlayVoiceOver(float duration) 
    {
        voiceoverEvent.start();
        float time = 0;
        //tourUI.SetAudioPlayButton(2);

        while (time < duration)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        voiceOverCoroutine = null;
    }

    public IEnumerator PlayAnimation(float duration)
    {
        //Manage Time
        currentTime = currentBeat == 0 ? 0 : tourBeats[currentBeat - 1].animationEndTime;
        TimeManager.Instance.SetTime(currentTime);
        TimeManager.Instance.SetPlaySpeed(tourBeats[currentBeat].animationPlaySpeed);
        TimeManager.Instance.SetPlayState(true);

        float time = 0;

        while (time < duration)
        {
            currentTime = TimeManager.Instance.GetCurrentTime();
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        currentTime = tourBeats[currentBeat].animationEndTime;
        TimeManager.Instance.SetPlayState(false);
        animationCoroutine = null;

        if (mute)
        {
            tourUI.SetReplayButton(TourWheelUI_Prototyping.ButtonState.inactive);
        }

    }

    /// <summary>
    /// Teleport players to a specific beat, and then play the animation as they just arrive the stop
    /// Called by the script TourBeatButton when players interact with the button. The script is attached to the beat button prefab
    /// </summary>
    /// <param name="beatIndex"></param>
    public void JumpToBeat(int beatIndex)
    {
        if(0 <= beatIndex && beatIndex < tourBeats.Count)
        {
            tourUI.SelectBeatButton(beatIndex-1);

            //Sets all current annoations and highlights disabled. 
            ToggleAnnotations(false);
            currentBeat = beatIndex;
            TourStop_Prototyping currentStop = tourBeats[currentBeat];

            //Toggles TourStopMesh
            foreach (GameObject g in tourStopMesh)
            {
                g.SetActive(false);
            }
            tourStopMesh[currentBeat - 1].SetActive(true);
            playerController.transform.rotation = tourStopMesh[currentBeat - 1].transform.rotation;

            //Sets all current annoation and highlights active. 
            ToggleAnnotations(true);

            //Update player position by changing spline animation time
            playerSpline.NormalizedTime = currentStop.splineEndTime;
            platformSpline.NormalizedTime = currentStop.splineEndTime;
            tourIndicator.NormalizedTime = currentStop.splineEndTime;

            if(sequenceCoroutine != null)
            {
                StopCoroutine(sequenceCoroutine);
                sequenceCoroutine = null;
            }           

            if(animationCoroutine != null) 
            { 
                StopCoroutine(animationCoroutine);
                animationCoroutine = null;
            }

            if (voiceOverCoroutine != null)
            {
                voiceoverEvent.release();
                voiceoverEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                StopCoroutine(voiceOverCoroutine);
                voiceOverCoroutine = null;
            }

            voiceoverEvent = FMODUnity.RuntimeManager.CreateInstance(currentStop.voiceOverEvent);
            sequenceCoroutine = StartCoroutine(PlaySequence());

            //Update text
            if (currentBeat > 0){
                tourIndex.text = "Step " + currentBeat.ToString();
                tourIndicatorIndex.SetText(currentBeat.ToString());
                tourText.text = currentStop.text;
            }

            WIAC_Manager.Instance.NotifyNexusTourStopChanged(currentStop);
        }
        else
        {
            Debug.LogWarning("Beat Index Out of range");
        }
    }    

    /// <summary>
    /// Utility
    /// Return a Vector2 including start and end timestamp for current beat animation
    /// </summary>
    /// <returns></returns>
    public Vector2 GetCurrentBeatTimestamps()
    {
        Vector2 timestamps = Vector2.zero;
        if(0 < currentBeat && currentBeat <= tourBeats.Count - 1)
        {
            timestamps.x = currentBeat == 0 ? 0 : tourBeats[currentBeat - 1].animationEndTime;
            timestamps.y = tourBeats[currentBeat].animationEndTime;
        }
        return timestamps;
        
    }
    public List<TourStop_Prototyping> GetTourBeats(){
        return tourBeats;
    }
}
