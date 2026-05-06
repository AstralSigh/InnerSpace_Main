using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace BNG
{
    public class HapticsManager : GrabbableEvents
    {
        public static HapticsManager Instance { get; private set; }
        public enum VibrationPreset { OnTouch, OnGrab, OnLaser, OnComplete };

        private bool vibrating;

        private void Start()
        {
            Instance = this;
        }

        //Automatically identifies active hand based on Action Menu Manager -- DOMINANT LASER HAND. 
        public void OneShotVibration(VibrationPreset preset)
        {
            ControllerHand activeHand = ActionMenuManager_Prototype.Instance.GetCurrentLaserHand();

            switch (preset)
            {
                case VibrationPreset.OnTouch:
                    OnTouchVibration(activeHand);
                    break;
                case VibrationPreset.OnGrab:
                    OnGrabVibration(activeHand);
                    break;
                case VibrationPreset.OnLaser:
                    OnLaserVibration(activeHand);
                    break;
                case VibrationPreset.OnComplete:
                    OnCompleteVibration(activeHand);
                    break;
            }
        }

        //User manually inputs controller hand to vibrate. 
        public void OneShotVibration(VibrationPreset preset, ControllerHand hand)
        {
            switch (preset)
            {
                case VibrationPreset.OnTouch:
                    OnTouchVibration(hand);
                    break;
                case VibrationPreset.OnGrab:
                    OnGrabVibration(hand);
                    break;
                case VibrationPreset.OnLaser:
                    OnLaserVibration(hand);
                    break;
                case VibrationPreset.OnComplete:
                    OnCompleteVibration(hand);
                    break;
            }
        }

        //PRESETS. DESIGNERS EDIT THIS STUFF. LATER MOVE THIS TO SCRIPTABLE GAME OBJECT
        private void OnTouchVibration(ControllerHand hand)
        {

            input.VibrateController(1f, 0.2f, .1f, hand);    
        }

        private void OnGrabVibration(ControllerHand hand)
        {   
            input.VibrateController(1f, 0.2f, .1f, hand);
        }
        
        private void OnLaserVibration(ControllerHand hand)
        {
            input.VibrateController(1f, 0.1f, .1f, hand);
        }

        private void OnCompleteVibration(ControllerHand hand)
        {
            {
                input.VibrateController(1f, 0.5f, 0.1f, hand);
            }
        }

        //MESSY STUFF FOR VERY SPECIFC INTERACTION. REFACTOR LATER 

        public void OnHoldVibration(float frequency, float duration, ControllerHand hand)
        {
            if(!vibrating)
            {
                input.VibrateController(1, frequency / 3f, duration, hand);
                StartCoroutine(VibrationTimer(duration));
            }

        }

        IEnumerator VibrationTimer(float duration)
        {
            vibrating = true;
            float time = 0;
            while(time < duration)
            {
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            vibrating = false;
        }
    }
}