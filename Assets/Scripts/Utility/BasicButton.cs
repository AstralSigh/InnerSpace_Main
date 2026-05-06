using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BNG;
using UnityEngine.UI;

// TODO: Do we use this class anymore?
public class BasicButton : MonoBehaviour
{
    public UnityEvent onTouch, onExit, OnGrip, onGripButtonWithTimer, onGripRelease;

    //VARIABLES TO CONTROL
    public float loadDuration = 1;
    [SerializeField] private Image localFillSprite;
    [SerializeField] private bool usingTimer = true;
    [SerializeField] private ControllerBinding leftButton = ControllerBinding.LeftTrigger;
    [SerializeField] private ControllerBinding leftButtonDown = ControllerBinding.LeftTriggerDown;
    [SerializeField] private ControllerBinding rightButton = ControllerBinding.RightTrigger;
    [SerializeField] private ControllerBinding rightButtonDown = ControllerBinding.RightTriggerDown;

    //STATE MANAGEMENT
    private enum ButtonState { notStarted, progressing, finished, stopped }
    private ControllerHand currentButtonInput = ControllerHand.None;
    private ButtonState currentButtonState = ButtonState.notStarted;

    //FMOD
    private FMOD.Studio.EventInstance loadAud;


    //EVENTS
    public delegate void RightTouch();
    public static event RightTouch OnRightTouch;

    public delegate void LeftTouch();
    public static event LeftTouch OnLeftTouch;

    public delegate void RightRelease();
    public static event RightRelease OnRightRelease;

    public delegate void LeftRelease();
    public static event LeftRelease OnLeftRelease;


    private void Start()
    {
        if (usingTimer)
        {
            if (!localFillSprite)
            {
                Transform temp = FindObject(this.transform, "LoadIcon").transform;
                localFillSprite = FindObject(temp, "Image").GetComponent<Image>();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        onTouch.Invoke();
        if (other.tag == "Hand")
        {
            HapticsManager.Instance.OneShotVibration(HapticsManager.VibrationPreset.OnTouch, other.GetComponent<Grabber>().HandSide);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (currentButtonInput == ControllerHand.None)
        {
            if (other.tag == "Hand")
            {
                if (other.GetComponent<Grabber>().HandSide == ControllerHand.Left)
                {
                    currentButtonInput = ControllerHand.Left;
                    OnLeftTouch();
                }
                else if (other.GetComponent<Grabber>().HandSide == ControllerHand.Right)
                {
                    currentButtonInput = ControllerHand.Right;
                    OnRightTouch();
                }
            }
        }
    }

    private void Update()
    {
        if (usingTimer)
        {
            switch (currentButtonState)
            {
                case ButtonState.notStarted:           
                    if ((leftButtonDown.GetDown() && currentButtonInput == ControllerHand.Left) ||
                        (rightButtonDown.GetDown() && currentButtonInput == ControllerHand.Right))
                    {
                        //IF LEFT OR RIGHT HAND GRABS THIS GAMEOBJECT 
                        currentButtonState = ButtonState.progressing;
                        HapticsManager.Instance.OneShotVibration(HapticsManager.VibrationPreset.OnGrab, currentButtonInput);
                    }
                    break;

                case ButtonState.progressing:
                    if (((leftButton.GetDown() && currentButtonInput == ControllerHand.Left)) ||
                        ((rightButton.GetDown() && currentButtonInput == ControllerHand.Right)))
                    {
                        ///WHILE BUTTON IS PROGRESSING
                        localFillSprite.fillAmount += Time.deltaTime;

                        HapticsManager.Instance.OnHoldVibration(localFillSprite.fillAmount, .2f, currentButtonInput);
                        

                        if (localFillSprite.fillAmount >= loadDuration)
                        {
                            currentButtonState = ButtonState.finished;
                        }
                    }
                    else
                    {
                        currentButtonState = ButtonState.stopped;
                    }
                    break;

                //IF GRABBING COMPLETES LOAD DURATION
                case ButtonState.finished:
                    FMODUnity.RuntimeManager.PlayOneShot2D("event:/UI Events/hydro_select_oneshot");

                    HapticsManager.Instance.OneShotVibration(HapticsManager.VibrationPreset.OnComplete, currentButtonInput);

                    onGripButtonWithTimer.Invoke();
                    ResetButton();
                    break;

                //IF GRABBING IS RELEASED BEFORE LOAD DURATION
                case ButtonState.stopped:
                    onGripRelease.Invoke();
                    ResetButton();
                    break;
            }
        }
        else //IF NOT USING TIMER
        {
            switch (currentButtonState)
            {
                case ButtonState.notStarted:
                    if ((leftButtonDown.GetDown() && currentButtonInput == ControllerHand.Left) || 
                        (rightButtonDown.GetDown() && currentButtonInput == ControllerHand.Right))
                    {
                        OnGrip.Invoke();
                        currentButtonState = ButtonState.progressing;
                    }
                    break;

                case ButtonState.progressing:
                    if (currentButtonInput == ControllerHand.Left)
                    {
                        if (!leftButtonDown.GetDown())
                        {
                            onGripRelease.Invoke();
                            currentButtonState = ButtonState.notStarted;
                        }
                    }

                    if (currentButtonInput == ControllerHand.Right)
                    {
                        if (!rightButtonDown.GetDown())
                        {
                            onGripRelease.Invoke();
                            currentButtonState = ButtonState.notStarted;
                        }
                    }
                    break;
            }                
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentButtonState == ButtonState.notStarted)
        {
            if (other.tag == "Hand")
            {
                ResetButton();
                onExit.Invoke();
            }
        }
    }

    private void ResetButton()
    {
        //EndAudioLoop();
        if(localFillSprite != null)
        {
            localFillSprite.fillAmount = 0;
        }

        switch (currentButtonInput)
        {
            case ControllerHand.Left:
                OnLeftRelease();
                break;

            case ControllerHand.Right:
                OnRightRelease();
                break;
        }

        currentButtonInput = ControllerHand.None;
        currentButtonState = ButtonState.notStarted;
        onExit.Invoke();
    }

    //NOT BEING USED
    private void StartAudioLoop()
    {
        if (usingTimer)
        {
            loadAud = FMODUnity.RuntimeManager.CreateInstance("event:/UI Events/hold_confirm");
            loadAud.start();
        }
    }

    //NOT BEING USED
    private void EndAudioLoop()
    {
        if (usingTimer)
        {
            loadAud.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            loadAud.release();
        }
    }   

    private void OnDisable()
    {
        ResetButton();
    }

    public GameObject FindObject(Transform obj, string name)
    {
        Transform[] trs = obj.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }

}
