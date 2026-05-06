using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using TMPro;
using BNG;
using System.Linq;
using Unity.XR.Oculus;

public class TutorialManager_Prototyping : MonoBehaviour
{
    public static TutorialManager_Prototyping Instance;

    [SerializeField] private bool isQuest;

    //DEBUGGING TOOLS
    [SerializeField] private int currentBeatIndex = 0;

    //List of all beats in tutorial 
    [SerializeField]
    public List<TutorialBeat_Prototyping> tutorialBeats;

    //External Managers
    public InputManager inputManager;
    public TextMeshPro tutorialText;
    public PivotManager pivotManager;
    public TooltipManager tooltipManager;
    public HighlightManager highlightManager;
    public WIAC_Manager questManager;

    //Private Variables
    private IEnumerator overrideTimer;

    //pivotManager.UpdatePivot(beats[currentBeatIndex].textPivot);

    public void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(this); }

        questManager = WIAC_Manager.Instance.GetComponent<WIAC_Manager>();
        // TODO: REWORK INPUTMANAGER AND MAKE IT AN INSTANCE TO AVOID THIS FIND.
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
    }

    public void Start()
    {
        PlayBeat();
    }

    //Called from gameObjects around the scene with the script TutorialActionEmitter
    public void ValidateInput(TutorialInputs.InputName incomingInput)
    {

    }

    public void PlaySuccessSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot2D("event:/UI Events/UI_success");
    }

    void PlayBeat()
    {
        if (currentBeatIndex < tutorialBeats.Count)
        {
            StartCoroutine(PlayingBeat(tutorialBeats[currentBeatIndex]));
        }
    }

    public void JumpBeat(int val)
    {
        currentBeatIndex = val;
    }

    public void IncrementBeat()
    {
        currentBeatIndex++;
    }

    IEnumerator PlayingBeat(TutorialBeat_Prototyping beat)
    {
        beat.BeginBeat();
        yield return new WaitUntil(() => beat.CheckBeat(ref currentBeatIndex));
        beat.EndBeat();

        PlayBeat();
    }
}
