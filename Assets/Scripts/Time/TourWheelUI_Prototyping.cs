using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
//using UnityEditor.ShaderGraph.Internal;
using UnityEngine.Events;
using UnityEngine.Splines;
using BNG;
using DG.Tweening;
using UnityEngine.Timeline;

public class TourWheelUI_Prototyping : MonoBehaviour
{
    [SerializeField] private TourWheelManager_Prototyping tourManager;
    //[SerializeField] private Image jumponButton;
    [SerializeField] private Image jumpoffButton; //THIS IS OUTSIDE OF THE PREFAB
    [SerializeField] private Sprite[] jumpoffButtonSprites;
    [SerializeField] private Image audioPlayButton;
    [SerializeField] private Sprite[] audioPlayButtonSprites;
    [SerializeField] private Image replayButton;
    [SerializeField] private Sprite[] replayButtonSprites;
    [SerializeField] private float radius;
    [SerializeField] private Image progressBar;
    [SerializeField] private SpriteRenderer timeWheelUI;
    [SerializeField] private GameObject beatButtonPrefab; 
    [SerializeField] private Transform beatButtonContainer;
    [SerializeField] private Text tourText;
    [SerializeField] private Text tourIndex;
    //[SerializeField] private GameObject tourStopMarkerPrefab; //Stop Indicator on the visible track of the tour
    
    private List<GameObject> beatButtons = new List<GameObject>();

    private Tweener jumpoffButtonTween;
    private Tweener audioPlayButtonTween;
    private Tweener replayButtonTween;

    public enum ButtonState { inactive, hover, activate }
    public ButtonState currentState;
    public enum AudioButtonState { unmute, mute}
    public AudioButtonState currentAudioState;


    public void SetJumpOffButton(int state)
    {
        jumpoffButton.transform.parent.parent.gameObject.SetActive(true);
        if (jumpoffButtonTween.IsActive())
        {
            jumpoffButtonTween.Kill();
        }
        switch (state)
        {
            case 0:
                //Inactivated
                jumpoffButton.transform.parent.parent.GetComponent<LaserButton>().enabled = true;
                jumpoffButton.transform.parent.parent.GetComponent<Collider>().enabled = true;
                jumpoffButton.sprite = jumpoffButtonSprites[state];
                jumpoffButton.DOColor(Color.white, .5f);
                break;
            case 1:
                //Hovering
                jumpoffButton.transform.parent.parent.GetComponent<LaserButton>().enabled = true;
                jumpoffButton.transform.parent.parent.GetComponent<Collider>().enabled = true;
                jumpoffButton.sprite = jumpoffButtonSprites[state];
                break;
            case 2:
                //Activate
                jumpoffButton.transform.parent.parent.GetComponent<LaserButton>().enabled = false;
                jumpoffButton.transform.parent.parent.GetComponent<Collider>().enabled = false;
                jumpoffButton.sprite = jumpoffButtonSprites[state];
                break;
            case -1:
                //Disable
                jumpoffButton.transform.parent.parent.GetComponent<LaserButton>().enabled = false;
                jumpoffButton.transform.parent.parent.GetComponent<Collider>().enabled = false;
                jumpoffButton.DOColor(Color.clear, .5f);
                break;
        }
    }
        public void SetReplayButton(ButtonState currentState)
    {
        replayButton.transform.parent.parent.gameObject.SetActive(true);
        if(replayButtonTween.IsActive())
        {
            replayButtonTween.Kill();
        }
        switch (currentState)
        {
            case ButtonState.inactive:
                //Inactivated
                replayButton.sprite = replayButtonSprites[0];
                replayButton.DOColor(Color.white, .5f);
                break;
            case ButtonState.hover:
                //Hovering
                replayButton.sprite = replayButtonSprites[1];
                break;
            case ButtonState.activate:
                //Activate
                replayButton.sprite = replayButtonSprites[2];
                break;
        }
    }

    /// <summary>
    /// A short cut to set all the beat buttons
    /// </summary>
    /// <param name="state"></param>
    public void SetBeatButtonsInactive()
    {
        foreach (GameObject beatButton in beatButtons)
        {
            beatButton.GetComponent<TourBeatButton_Prototyping>().UpdateButtonState(0);
        }
    }

    public void SelectBeatButton(int index)
    {
        beatButtons[index].GetComponent<TourBeatButton_Prototyping>().UpdateButtonState(TourBeatButton_Prototyping.ButtonState.CurrentBeat);
    }

    public void SetAudioPlayButton(AudioButtonState currentState)
    {
        audioPlayButton.transform.parent.parent.gameObject.SetActive(true);
        if (audioPlayButtonTween.IsActive())
        {
            audioPlayButtonTween.Kill();
        }
        switch (currentState)
        {
            case AudioButtonState.mute:
                //Inactive
                audioPlayButton.sprite = audioPlayButtonSprites[0];
                break;
            case AudioButtonState.unmute:
                //Hovering
                audioPlayButton.sprite = audioPlayButtonSprites[1];
                break;
        }
    }

    //Edits fill amount on progress bar that overlays the timeWheel
    public void UpdateProgressBar()
    {      
        //Equidistant
        var currentTime = TimeManager.Instance.GetCurrentTime();
        var currentBeatTimestamps = tourManager.GetCurrentBeatTimestamps();
        if (currentBeatTimestamps.y != 0 && (currentBeatTimestamps.x <= currentTime))
        {
            float currentFill;
            //When no animation play at current beat
            if (currentBeatTimestamps.x == currentBeatTimestamps.y)
            {
                currentFill = (1f / (tourManager.GetTourBeats().Count));
            }
            else
            {
                currentFill = ((currentTime - currentBeatTimestamps.x) / (currentBeatTimestamps.y - currentBeatTimestamps.x)) * (1f / (tourManager.GetTourBeats().Count-1));
            }
            var fill = ((float)(tourManager.currentBeat-1) / (tourManager.GetTourBeats().Count-1)) + currentFill;
            progressBar.fillAmount = fill;
        }
        else
        {
            progressBar.fillAmount = 0;
            Debug.LogWarning("Time doesn't match with current tour beat timeslot");
        }

    }

    //Should get called whenever you enter a Nexus
    public void CreateJumpToBeatButtons()
    {
        //Clears beat buttons list. 
        if (beatButtons != null)
        {
            foreach (GameObject marker in beatButtons)
            {
                Destroy(marker);
            }
            beatButtons.Clear();
        }

        //Create all beat buttons
        for(int i = 0; i < tourManager.GetTourBeats().Count-1; i++)
        {
            //Instantiate Markers(Buttons) and plot them on wheel equidistantly
            beatButtons.Add(Instantiate(beatButtonPrefab));
            beatButtons[i].transform.SetParent(beatButtonContainer);
            var normalized = (float)(i) / (float)(tourManager.GetTourBeats().Count-1) + 1f/(2* (float)(tourManager.GetTourBeats().Count - 1));
            float anglePos = 2 * Mathf.PI * normalized + (Mathf.PI / 2);
            float angleRot = 2 * Mathf.PI * normalized;
            beatButtons[i].transform.localPosition = new Vector3(Mathf.Cos(anglePos) * radius, Mathf.Sin(anglePos) * radius, -.001f);
            beatButtons[i].transform.localRotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angleRot);
            beatButtons[i].transform.localScale = Vector3.one;
            beatButtons[i].transform.GetComponent<TourBeatButton_Prototyping>().beatIndex = i+1;
            beatButtons[i].transform.GetComponent<TourBeatButton_Prototyping>().tourManager = tourManager;
        }
    }
    
    public void UpdateUIOnJumpOnOff(bool isJumpOff)
    {
        if (isJumpOff)
        {
            audioPlayButton.transform.parent.parent.gameObject.SetActive(false);
            jumpoffButton.transform.parent.parent.gameObject.SetActive(false);
            replayButton.transform.parent.parent.gameObject.SetActive(false);
            foreach(GameObject button in beatButtons)
            {
                button.SetActive(false);
            }

            tourText.enabled = false;
            //tourIndex.enabled = false;
            GetComponent<Grabbable>().enabled = false;
        }
        else
        {
            SetReplayButton(0);
            SetAudioPlayButton(AudioButtonState.mute);
            SetJumpOffButton(0);
            foreach (GameObject button in beatButtons)
            {
                button.SetActive(true);
            }
            tourText.enabled = true;
            tourIndex.enabled = true;
            GetComponent<Grabbable>().enabled = true;
        }
    }
    //Plot item on timeWheel based on its time -- Used when we had markers.
    private void PlotOnTimeWheel(GameObject item, float percent)
    {
        //float timeInPercent = time / (float)timeManager.GetCurrentDuration();
        //float timeInPercent = tourManager.currentBeat / tourManager.tourBeats.Count;
        float angle = 2 * Mathf.PI * percent + Mathf.PI * .5f;
        item.transform.localPosition = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        item.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle);
    }
}
