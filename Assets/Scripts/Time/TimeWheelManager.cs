using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
using UnityEngine.UI;
public class TimeWheelManager : MonoBehaviour
{
    //EXTERNAL MANAGERS 
    // [SerializeField] private TimeManager timeManager;
    [SerializeField] private WIAC_Manager questManager;
    [SerializeField] SpriteManager playButtonUI;
    [SerializeField] SimulationAudioManager simulationAudioManager;
    //REFERENCES 
    [SerializeField] private GameObject checkpointPrefab;
    [SerializeField] private GameObject checkpointMarkerPrefab;
    [SerializeField] private GameObject timeHandleGrabbable;
    [SerializeField] private GameObject timeHandleUI;
    [SerializeField] private Transform timeHandleReferencePoint;
    [SerializeField] private GameObject player;
    [SerializeField] private Image progressBar;

    //EDIT IN INSPECTOR 
    [SerializeField] private float scrubSensitivity = 10;
    [SerializeField] private float radius = 0.346f;
    [SerializeField] private bool gateScrubbing;
    private List<GameObject> checkpoints;
    private List<GameObject> checkpointMarkers;
    private bool scrubbing = false;
    private bool playingCache = false;
    private bool held = false;

    //timeHandleGrabbable has BasicGrab script that calls SetScrubbing on grip and release. 
    public void SetScrubbing(bool scrubbing)
    {
        this.scrubbing = scrubbing;
        simulationAudioManager.SetScrubbing(scrubbing); //Tells simulationAudioManager it's scrubbing so it doesn't play sounds.

        if (scrubbing && held == false)
        {
            held = true;
            playingCache = TimeManager.Instance.GetCurrentPlayState();
            TimeManager.Instance.SetPlayState(false);
        }
        //When timeHandleGrabbable is released its position is set back to 0,0,0 (At the position of timeHandleUI) to reset handle grabbability 
        else if(!scrubbing)
        {
            held = false;
            TimeManager.Instance.SetPlayState(playingCache);
            timeHandleGrabbable.transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    void Update()
    {
        if (scrubbing)
        {
            CalculateScrub();
        }
        else
        {
            float currentTime = TimeManager.Instance.GetCurrentTime();
            PlotOnTimeWheel(timeHandleUI, currentTime);
            UpdateProgressBar(currentTime);
        }
    }

    private void CalculateScrub()
    {
        // Do the calculations to figure out how much time and timePoint should change
        Vector3 timeHandleUIForward = timeHandleUI.transform.TransformDirection(Vector3.up);
        Vector3 timeHandlerGrabbablePos = timeHandleGrabbable.transform.position - timeHandleReferencePoint.position;
        float directionToGrabbable = Vector3.Dot(timeHandleUIForward, timeHandlerGrabbablePos);
        float targetTime = TimeManager.Instance.GetCurrentTime() + (directionToGrabbable * scrubSensitivity);

        if (gateScrubbing)
        {
            // If targetTime is greater than duration or smaller than 0 lock point to zero. 
            if (targetTime > TimeManager.Instance.GetCurrentDuration() || targetTime < 0)
            {
                PlotOnTimeWheel(timeHandleUI, 0);
                UpdateProgressBar(targetTime);
            }
            // If targetTime is valid plot timeHandleUI and update TimeManager.Instance (playable director) 
            else
            {
                PlotOnTimeWheel(timeHandleUI, targetTime);
                UpdateProgressBar(targetTime);
                TimeManager.Instance.SetTime(targetTime);
            }
        }
        else
        {
            //If targetTime is greater than duration it goes back past 0
            if (targetTime > TimeManager.Instance.GetCurrentDuration())
            {
                targetTime -= TimeManager.Instance.GetCurrentDuration();
            }
            //If targetTime is smaller than zero it goes past max inversly 
            else if (targetTime < 0)
            {
                targetTime = TimeManager.Instance.GetCurrentDuration() + targetTime;
            }
            PlotOnTimeWheel(timeHandleUI, targetTime);
            UpdateProgressBar(targetTime);
            TimeManager.Instance.SetTime(targetTime);
        }
    }

    //Plot item on timeWheel based on its time
    public void PlotOnTimeWheel(GameObject item, float time)
    {
        float timeInPercent = time / (float)TimeManager.Instance.GetCurrentDuration();
        float angle = 2 * Mathf.PI * timeInPercent + Mathf.PI * .5f;
        item.transform.localPosition = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        item.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle);
    }

    //Teleport player to the target infoNode
    // TODO: I think this is no longer used. Delete it. Michael 7/4/2023
    // public void TeleportToInfoNode(int index)
    // {
    //     player.GetComponent<PlayerTeleport>().TeleportPlayer(
    //         questManager.currentNexusManager.LocalTimeWheelInfoNodes[index].objectRef.position -
    //         ((questManager.currentNexusManager.LocalTimeWheelInfoNodes[index].objectRef.GetComponent<InfoNodeHead>().GetStartRotation() * Vector3.forward).normalized * 0.5f )
    //         ,
    //         questManager.currentNexusManager.LocalTimeWheelInfoNodes[index].objectRef.GetComponent<InfoNodeHead>().GetStartRotation()
    //         );
    // }

    //Updates graphic on play button 
    public void UpdatePlayButtonUI(bool playing)
    {
        if (playing)
        {
            playButtonUI.UpdateSprite(0); 
        }
        else
        {
            playButtonUI.UpdateSprite(1);
        }
    }

    //Edits fill amount on progress bar that overlays the timeWheel
    public void UpdateProgressBar(float time)
    {
        progressBar.fillAmount = time / TimeManager.Instance.GetCurrentDuration();
    }
}
