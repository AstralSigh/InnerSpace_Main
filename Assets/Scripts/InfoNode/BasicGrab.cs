using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BNG;
public class BasicGrab : MonoBehaviour
{
    [SerializeField] private ControllerBinding leftButton = ControllerBinding.LeftTrigger;
    [SerializeField] private ControllerBinding leftButtonDown = ControllerBinding.LeftTriggerDown;
    [SerializeField] private ControllerBinding rightButton = ControllerBinding.RightTrigger;
    [SerializeField] private ControllerBinding rightButtonDown = ControllerBinding.RightTriggerDown;

    public UnityEvent onLeftGrab, onRightGrab, onLeftRelease, onRightRelease, onTouch;

    enum ButtonState { notGrabbed, grabbed, released}
    enum ButtonInput { left, right, notAssigned }
    ButtonState currentButtonState = ButtonState.notGrabbed;
    ButtonInput currentButtonInput = ButtonInput.notAssigned;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Hand")
        {
            HapticsManager.Instance.OneShotVibration(HapticsManager.VibrationPreset.OnTouch, other.GetComponent<Grabber>().HandSide);
        }
        onTouch.Invoke();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Hand")
        {
            switch (currentButtonState)
            {
                case ButtonState.notGrabbed:
                    if (leftButtonDown.GetDown() && other.GetComponent<Grabber>().HandSide == ControllerHand.Left)
                    {
                        HapticsManager.Instance.OneShotVibration(HapticsManager.VibrationPreset.OnGrab, ControllerHand.Left);
                        onLeftGrab.Invoke();
                        currentButtonInput = ButtonInput.left;
                        currentButtonState = ButtonState.grabbed;   
                    }
                    if (rightButtonDown.GetDown() && other.GetComponent<Grabber>().HandSide == ControllerHand.Right)
                    {
                        HapticsManager.Instance.OneShotVibration(HapticsManager.VibrationPreset.OnGrab, ControllerHand.Right);
                        onRightGrab.Invoke();
                        currentButtonInput = ButtonInput.right;
                        currentButtonState = ButtonState.grabbed;
                    }
                    break;
                //add onupdate 
                case ButtonState.grabbed:
                    if(currentButtonInput == ButtonInput.left && !leftButton.GetDown())
                    {
                        onLeftRelease.Invoke();
                        currentButtonState = ButtonState.notGrabbed;
                        currentButtonInput = ButtonInput.notAssigned;
                    }

                    if(currentButtonInput == ButtonInput.right && !rightButton.GetDown())
                    {
                        onRightRelease.Invoke();
                        currentButtonState = ButtonState.notGrabbed;
                        currentButtonInput = ButtonInput.notAssigned;
                    }
                    break;
            }
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Hand")
        {
            currentButtonState = ButtonState.notGrabbed;
            currentButtonInput = ButtonInput.notAssigned;
        }
    }

    private void OnDisable()
    {
        currentButtonState = ButtonState.notGrabbed;
        currentButtonInput = ButtonInput.notAssigned;
    }
}
