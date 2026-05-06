using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class SimulationAudioManager : MonoBehaviour
{
    public static SimulationAudioManager Instance { get; private set; }

    public class EncodedAudioEvent{
        public Transform objRef;
        public float bindingTime;
        public bool hexamer; 

        public EncodedAudioEvent(Transform objRef, float bindingTime, bool hexamer)
        {
            this.objRef = objRef;
            this.bindingTime = bindingTime;
            this.hexamer = hexamer;
        }
    }

    //NEXUS08
    [SerializeField] private InsulinSavedEventTimes bindingTimes08;
    [SerializeField] private float startOffset08;
    [SerializeField] private float duration08;
    [SerializeField] private GameObject reference08;

    //NEXUS09
    [SerializeField] private InsulinSavedEventTimes bindingTimes09;
    [SerializeField] private float startOffset09;
    [SerializeField] private float duration09;
    [SerializeField] private GameObject reference09;

    //LOCAL VARIABLES
    private InsulinSavedEventTimes currentBindingTimes;
    private float currentStartOffset;
    private float currentDuration;
    private GameObject currentNexusReference;
    private List<EncodedAudioEvent> currentEncodedAudioList;
    private bool scrubbing;
    private Nexus_Data.eNexusType currentNexus;

    void Awake()
    {
        Instance = this;
    }

    public void Update()
    {
        if(currentNexus == Nexus_Data.eNexusType.ImmatureCrystal || currentNexus == Nexus_Data.eNexusType.InsulinRelease)
        {
            if (!scrubbing && TimeManager.Instance.GetCurrentPlayState())
            {
                CheckAndPlaySounds();
            }
        }
    }

    // TODO: HOOK THIS UP TO THE ON ONCHANGENEXUS EVENT SO THAT NEXUS 8 AND 9 AUDIO WILL WORK.
    // SIMULATIONAUDIOMANAGER IS CURRENTLY UNPLUGGED. 7/3/2023 MICHAEL
    // NICK REPLUG 09/29/2023
    public void OnEnterNexus(Nexus_Data.eNexusType nexusType)
    {
        currentNexus = nexusType;
        if(currentNexus == Nexus_Data.eNexusType.ImmatureCrystal)
        {         
            currentBindingTimes = bindingTimes08;
            currentStartOffset = startOffset08;
            currentDuration = duration08;
            currentNexusReference = reference08;
            FillLocalList();
        }
        else if(currentNexus == Nexus_Data.eNexusType.InsulinRelease)
        {
            currentBindingTimes = bindingTimes09;
            currentStartOffset = startOffset09;
            currentDuration = duration09;
            currentNexusReference = reference09;
            FillLocalList();
        }
    }

    //Parses InsulinSavedEventTimes data into the local list currentEncodedAudioList
    public void FillLocalList()
    {
        if (currentEncodedAudioList == null)
        {
            currentEncodedAudioList = new List<EncodedAudioEvent>();
        }
        else
        {
            currentEncodedAudioList.Clear();
        }

        foreach(InsulinSavedEventTimes.audioEvent a in currentBindingTimes.bindingEventTimes)
        {
            Transform objToAdd = currentNexusReference.transform.Find(a._locationName);
            float encodedBindingTime = 0;
            if (currentNexus == Nexus_Data.eNexusType.ImmatureCrystal)
            {
                encodedBindingTime = currentStartOffset + currentDuration - a._time;
            }
            else if (currentNexus == Nexus_Data.eNexusType.InsulinRelease)
            {
                encodedBindingTime = currentStartOffset + ((currentDuration - a._time)/2);
            }
            
            bool isHexamer = (a._bindingType == InsulinSavedEventTimes.audioEvent.type.hexamer) ;
            currentEncodedAudioList.Add(new EncodedAudioEvent(objToAdd, encodedBindingTime, isHexamer));
        }
    }

    //Checks if there is a audio to play on this frame and plays it. 
    public void CheckAndPlaySounds()
    {
        float currentTime = TimeManager.Instance.GetCurrentTime();

        foreach (EncodedAudioEvent a in currentEncodedAudioList)
        {
            if (Mathf.Abs(currentTime - a.bindingTime) < Time.deltaTime)
            {
                if (a.hexamer)
                {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Object_Emitters/hexamer_binding", a.objRef.position);
                }
                else
                {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Object_Emitters/dimer_binding", a.objRef.position);
                }
            }
        }
    }

    //Set from TimeWheelManager
    public void SetScrubbing(bool scrubbing)
    {
        this.scrubbing = scrubbing;
    }
}
