using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using BNG;

public class NewHandJet : MonoBehaviour
{
    [SerializeField]
    public float JetForce = 10f;
    [SerializeField]
    public ParticleSystem JetFX;
    [SerializeField]
    ControllerHand hand;
    [SerializeField]
    private int _forward = 1;
    bool _firstFire = true;

    CharacterController characterController;
    SmoothLocomotion smoothLocomotion;
    PlayerGravity playerGravity;

    //AudioSource audioSource;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player)
        {
            characterController = player.GetComponentInChildren<CharacterController>();
            playerGravity = player.GetComponentInChildren<PlayerGravity>();
            smoothLocomotion = player.GetComponentInChildren<SmoothLocomotion>();
        }
        else
        {
            Debug.Log("No player object found.");
        }

        //audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(hand == ControllerHand.Left){
            float leftThumbstick = InputBridge.Instance.LeftThumbstickAxis.y;
            if(leftThumbstick != 0){
                doJet(leftThumbstick);
            }
        }
        else{
            float rightThumbstick = InputBridge.Instance.RightThumbstickAxis.y;
            if(rightThumbstick != 0){
                doJet(rightThumbstick);
            }
        }     
    }


    public void setForward(int forward)
    {
        _forward = forward;
    }


    public void movementController(float triggerAmount)
    {
        if (triggerAmount > 0.25f || triggerAmount < -0.25f)
        {
            doJet(triggerAmount);
        }
        else {
            stopJet();
        }
    }

    void doJet(float triggerValue)
    {
        if (_firstFire)
        {
            //FMODUnity.RuntimeManager.PlayOneShot2D("event:/UI Events/handjet_start");
            _firstFire = false;
        }

        Vector3 moveDirection = transform.forward * JetForce;

        // Use smooth loco method if available
        if (smoothLocomotion)
        {
            smoothLocomotion.MoveCharacter(moveDirection * Time.deltaTime * triggerValue * _forward);
        }
        // Fall back to character controller
        else if (characterController)
        {
            characterController.Move(moveDirection * Time.deltaTime * triggerValue * _forward);
        }

        // Gravity is always off while jetting
        ChangeGravity(false);

        // Sound
        //if (!audioSource.isPlaying)
        //{
        //    audioSource.pitch = Time.timeScale;
        //    audioSource.Play();
        //}

        // Particle FX
        if (JetFX != null && !JetFX.isPlaying)
        {
            JetFX.Play();
        }

        //Haptics
        //if (input && thisGrabber != null)
        //{
        //    input.VibrateController(0.1f, 0.5f, 0.2f, thisGrabber.HandSide);
        //}
    }

    void stopJet()
    {
        _firstFire = true;
        //if (audioSource.isPlaying)
        //{
        //    audioSource.Stop();
        //}

        if (JetFX != null && JetFX.isPlaying)
        {
            JetFX.Stop();
        }
    }
    public void ChangeGravity(bool gravityOn)
    {
        if (playerGravity)
        {
            playerGravity.ToggleGravity(gravityOn);
        }
    }

}
