using BNG;
using System.Collections;
using UnityEngine;
using FMOD.Studio;
using System.Collections.Generic;
using DG.Tweening;
/// <summary>
/// This is the brain of the hexamer formation sequence. To create one hexamer assembly sequence you need one of this + one monomer GameObject, and two dimer GameObjects. Everything is currently hardcoded and manually referenced in inspector.   
/// </summary>

public class MonomerMaster_MiniGame02 : MonoBehaviour
{
    //STATE
    public enum MonomerStates { IdleMonomer, GrabbedMonomer, CloseMonomers, MonomerAnimation, IdleDimer, GrabbedDimer, CloseDimer, DimerAnimation, IdleTrimer, GrabbedTrimer, CloseTrimer, TrimerAnimation, FinishedHexamer};
    public MonomerStates currentState;

    //SETTINGS
    [SerializeField] private bool isFirstMonomer;
    float blinkingTime = 0;
    public float blinkingDuration = 0.2f;
    public float blinkingFrequency = 1; // Number of pulses per second. 
    public float maxDistance = 1f;
    public float particlePow;
    public float particleIntensity = 10f;
    public float tolerance = 1f;
    public float lerpSpeed = 4f;
    public float lerpDuration = 2f;
    public float particleLifetime = 0.5f;
    //REFERENCES 
    public HexamerFormationColorProfiles colorProfiles;
    public GameObject monomer01;
    public GameObject monomerGhost;
    public GameObject dimerGhost;
    public GameObject dimer01;
    public GameObject dimer02;
    public Animator monomerAnimator;
    public Animator dimerAnimator;
    public GameObject threeFoldAxis;
    public GameObject zincTop;
    public GameObject zincBottom;
    public ParticleSystem[] ps = new ParticleSystem[6];
    ParticleSystem.EmissionModule[] em = new ParticleSystem.EmissionModule[6];
    ParticleSystem.MainModule[] m = new ParticleSystem.MainModule[6];
    public Material[] helicesMaterial;
    public Transform centerCamera;
    private Transform monomer01Forward;
    private Transform monomer01Mesh;
    private Transform thisMonomerForward;
    private Transform thisDimerForward;
    private Transform thisTrimerForward;
    private Transform thisMonomerMesh;
    private Transform dimer01Forward;
    private Transform dimer01MeshA;
    private Transform dimer01MeshB;
    private Transform dimer02Forward;
    private Transform dimer02MeshA;
    private Transform dimer02MeshB;
    EventInstance hexamerAssemblySFX;
    private bool previousState1 = false;
    private bool previousState2 = false;
    private bool previousState3 = false;
    private bool previousState4 = false;
    public Transform finalLerpTarget;
    public GameObject nextMonomerA;
    public GameObject nextMonomerB;
    public CrystalGameManager crystalGameManager;

    private void Start()
    {
        //ASSIGN REFERENCES
        thisMonomerForward = transform.Find("ForwardDirection");
        thisDimerForward = thisMonomerForward.Find("DimerForward");
        thisTrimerForward = thisDimerForward.Find("TrimerForward");
        thisMonomerMesh = thisTrimerForward.Find("MonomerMesh");
        monomer01Forward = monomer01.transform.Find("ForwardDirection");
        monomer01Mesh = monomer01Forward.transform.Find("MonomerMesh");
        dimer01Forward = dimer01.transform.Find("ForwardDirection");
        dimer01MeshA = dimer01Forward.Find("Monomer01");
        dimer01MeshB = dimer01Forward.Find("Monomer02");
        dimer02Forward = dimer02.transform.Find("ForwardDirection");
        dimer02MeshA = dimer02Forward.Find("Monomer01");
        dimer02MeshB = dimer02Forward.Find("Monomer02");
        ps[0] = thisMonomerForward.Find("ParticleSystem").GetComponent<ParticleSystem>();
        ps[1] = monomer01Forward.Find("ParticleSystem").GetComponent<ParticleSystem>();
        ps[2] = dimer01Forward.Find("ParticleSystemTop").GetComponent<ParticleSystem>();
        ps[3] = dimer01Forward.Find("ParticleSystemBottom").GetComponent<ParticleSystem>();
        ps[4] = dimer02Forward.Find("ParticleSystemTop").GetComponent<ParticleSystem>();
        ps[5] = dimer02Forward.Find("ParticleSystemBottom").GetComponent<ParticleSystem>();
        
        //SET UP PARTICLE SYSTEMS
        for(int x = 0; x < ps.Length; x++)
        {
            em[x] = ps[x].emission;
            m[x] = ps[x].main;
        }
        
        //STOP ALL PARTICLE SYSTEMS 
        foreach(ParticleSystem p in ps)
        {
            p.Stop();
        }

        //CHANGE COLORS
        thisMonomerMesh.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.monomer);
        monomer01Mesh.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.monomer);
        dimer01MeshA.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.dimer);
        dimer01MeshB.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.dimer);
        dimer02MeshA.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.dimer);
        dimer02MeshB.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.dimer);

        //SET UP AUDIO
        hexamerAssemblySFX = FMODUnity.RuntimeManager.CreateInstance("event:/UI Events/hexamer_assembly");
        
    }

    public void LookAt(Transform thisForward, Transform targetFoward, Vector3 thisUpwards, Vector3 targetUpwards)
    {
        float step = lerpSpeed * Time.deltaTime;
        Vector3 dirToTarget = targetFoward.position - transform.position;
        Quaternion dirToTargetRot = Quaternion.LookRotation(dirToTarget, thisUpwards);
        thisForward.rotation = Quaternion.Lerp(thisForward.rotation, dirToTargetRot, step);
        Vector3 dirToThis = transform.position - targetFoward.position;
        Quaternion dirToThisRotation = Quaternion.LookRotation(dirToThis, targetUpwards);
        targetFoward.rotation = Quaternion.Lerp(targetFoward.rotation, dirToThisRotation, step);
    }

    public void PlayGrabReleaseSounds()
    {
        bool currentState1 = transform.GetComponent<Grabbable>().BeingHeld;
        bool currentState2 = monomer01.GetComponent<Grabbable>().BeingHeld;
        bool currentState3 = dimer01.GetComponent<Grabbable>().BeingHeld;
        bool currentState4 = dimer02.GetComponent<Grabbable>().BeingHeld;

        //TO DO CHANGE THIS LATER
        if (currentState1 != previousState1)
        {
            //If held
            if (currentState1)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/UI Events/grab", this.transform.position);
            }
            //If released
            else
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/UI Events/text_close", this.transform.position);
            }
            previousState1 = currentState1;
        }

        if (currentState2 != previousState2)
        {
            //If held
            if (currentState2)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/UI Events/grab", this.transform.position);
            }
            //If released
            else
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/UI Events/text_close", this.transform.position);
            }
            previousState2 = currentState2;
        }

        if (currentState3 != previousState3)
        {
            //If held
            if (currentState3)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/UI Events/grab", this.transform.position);
            }
            //If released
            else
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/UI Events/text_close", this.transform.position);
            }
            previousState3 = currentState3;
        }

        if (currentState4 != previousState4)
        {
            //If held
            if (currentState4)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/UI Events/grab", this.transform.position);
            }
            //If released
            else
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/UI Events/text_close", this.transform.position);
            }
            previousState4 = currentState4;
        }
    }


    private void Update()
    {
        PlayGrabReleaseSounds();

        //ASSIGNING VARIABLES FOR PARTICLE SYSTEM 
        float monomerDistance = Vector3.Distance(ps[0].transform.position, ps[1].transform.position);
        float dimer01Distance = Vector3.Distance(zincTop.transform.GetChild(0).position, ps[2].transform.position);
        float dimer02Distance = Vector3.Distance(zincTop.transform.GetChild(0).position, ps[4].transform.position);
        
        Vector3 cameraToDimer = transform.position - Camera.main.transform.position;

        switch (currentState)
        {
            case MonomerStates.IdleMonomer:
                //IF BOTH MONOMERS ARE HELD
                if(transform.GetComponent<Grabbable>().BeingHeld && monomer01.GetComponent<Grabbable>().BeingHeld)
                {
                    ps[0].Play();
                    ps[1].Play();
                    em[0].rateOverTime = CalculateParticleEmission(monomerDistance);
                    em[1].rateOverTime = CalculateParticleEmission(monomerDistance);
                    m[0].startLifetime = monomerDistance * particleLifetime;
                    m[1].startLifetime = monomerDistance * particleLifetime;
                    hexamerAssemblySFX.start();
                    hexamerAssemblySFX.setParameterByName("mag_force", 0);
                    currentState = MonomerStates.GrabbedMonomer;
                }
                break;

            case MonomerStates.GrabbedMonomer:
                //IF EITHER MONOMERS ARE RELEASED 
                if (!transform.GetComponent<Grabbable>().BeingHeld || !monomer01.GetComponent<Grabbable>().BeingHeld)
                {
                    ps[0].Stop();
                    ps[1].Stop();
                    hexamerAssemblySFX.stop(STOP_MODE.ALLOWFADEOUT);
                    currentState = MonomerStates.IdleMonomer;
                }
                //IF INSIDE TOLERANCE RADIUS 
                else if (Vector3.Distance(thisMonomerForward.transform.position, monomer01Forward.transform.position) < tolerance)
                {
                    hexamerAssemblySFX.setParameterByName("mag_force", 1);
                    currentState = MonomerStates.CloseMonomers;
                }
                //Look at
                //LookAt(thisMonomerForward, monomer01Forward.transform, Vector3.down, Vector3.up);
                //LookAt(thisMonomerForward, monomer01Forward.transform, Quaternion.Euler(90,0,0) * cameraToDimer, Quaternion.Euler(-90, 0, 0) * cameraToDimer);
                LookAt(thisMonomerForward, monomer01Forward.transform, Quaternion.Euler(90,0,0) * cameraToDimer, -(Quaternion.Euler(90,0,0) * cameraToDimer));


                //Update Particle System
                em[0].rateOverTime = CalculateParticleEmission(monomerDistance)/4;
                em[1].rateOverTime = CalculateParticleEmission(monomerDistance)/4;
                m[0].startLifetime = monomerDistance * particleLifetime;
                m[1].startLifetime = monomerDistance * particleLifetime;
                break;

            case MonomerStates.CloseMonomers:
                //IF INSIDE TOLERANCE RADIUS
                if (Vector3.Distance(thisMonomerForward.transform.position, monomer01Forward.transform.position) < tolerance)
                {
                    //IF HELD
                    if (transform.GetComponent<Grabbable>().BeingHeld && monomer01.GetComponent<Grabbable>().BeingHeld)
                    {
                        //Look at
                        //LookAt(thisMonomerForward, monomer01Forward.transform, Quaternion.Euler(90,0,0) * cameraToDimer, Quaternion.Euler(-90, 0, 0) * cameraToDimer);
                        LookAt(thisMonomerForward, monomer01Forward.transform, Quaternion.Euler(90,0,0) * cameraToDimer, -(Quaternion.Euler(90,0,0) * cameraToDimer));


                        //Update Particle System
                        em[0].rateOverTime = CalculateParticleEmission(monomerDistance);
                        em[1].rateOverTime = CalculateParticleEmission(monomerDistance);
                        m[0].startLifetime = monomerDistance * particleLifetime;
                        m[1].startLifetime = monomerDistance * particleLifetime;


                        //Blinking betasheets
                        blinkingTime += Time.deltaTime;
                        if (blinkingTime > Mathf.Clamp((1f / blinkingFrequency), blinkingDuration, float.MaxValue))
                        {
                            StartCoroutine(BetaSheetPulse(blinkingDuration));
                            blinkingTime = 0;
                        }
                    }
                    //IF RELEASED
                    else
                    {
                        //Stop Particle system 
                        ps[0].Stop();
                        ps[1].Stop();

                        //Start Animation Coroutine
                        StartCoroutine(MonomerLerp(lerpDuration));
                        hexamerAssemblySFX.stop(STOP_MODE.ALLOWFADEOUT);
                        currentState = MonomerStates.MonomerAnimation;
                    }
                }
                //IF OUTSIDE OF TOLERANCE RADIUS 
                else if (Vector3.Distance(thisMonomerForward.transform.position, monomer01Forward.transform.position) > tolerance)
                {
                    //IF HELD
                    if (transform.GetComponent<Grabbable>().BeingHeld && monomer01.GetComponent<Grabbable>().BeingHeld)
                    {
                        hexamerAssemblySFX.setParameterByName("mag_force", 0);
                        currentState = MonomerStates.GrabbedMonomer;
                    }
                    //IF RELEASED
                    else
                    {
                        //Stop Particle System 
                        ps[0].Stop();
                        ps[1].Stop();
                        currentState = MonomerStates.IdleMonomer;
                    }
                }
                break;

            case MonomerStates.IdleDimer:
                if (transform.GetComponent<Grabbable>().BeingHeld && dimer01.GetComponent<Grabbable>().BeingHeld)
                {
                    //Play Particle System
                    ps[2].Play();
                    ps[3].Play();

                    //Update Particle System 
                    em[2].rateOverTime = CalculateParticleEmission(dimer01Distance);
                    em[3].rateOverTime = CalculateParticleEmission(dimer01Distance); ;
                    m[2].startLifetime = dimer01Distance * particleLifetime;
                    m[3].startLifetime = dimer01Distance * particleLifetime;
                    hexamerAssemblySFX.start();
                    hexamerAssemblySFX.setParameterByName("mag_force", 0);
                    currentState = MonomerStates.GrabbedDimer;
                }
                break;

            case MonomerStates.GrabbedDimer:
                //IF EITHER MONOMERS ARE RELEASED
                if (!transform.GetComponent<Grabbable>().BeingHeld || !dimer01.GetComponent<Grabbable>().BeingHeld)
                {
                    //Stop Particle System
                    ps[2].Stop();
                    ps[3].Stop();
                    hexamerAssemblySFX.stop(STOP_MODE.ALLOWFADEOUT);
                    currentState = MonomerStates.IdleDimer;
                }
                //IF INSIDE TOLERANCE RADIUS
                else if (Vector3.Distance(transform.position, dimer01.transform.position) < tolerance)
                {
                    hexamerAssemblySFX.setParameterByName("mag_force", 1);
                    currentState = MonomerStates.CloseDimer;
                }
                //Look at
                LookAt(thisDimerForward, dimer01Forward.transform, cameraToDimer, cameraToDimer);

                //Update Particle System
                em[2].rateOverTime = CalculateParticleEmission(dimer01Distance)/4;
                em[3].rateOverTime = CalculateParticleEmission(dimer01Distance)/4;
                m[2].startLifetime = dimer01Distance * particleLifetime;
                m[3].startLifetime = dimer01Distance * particleLifetime;
                break;

            case MonomerStates.CloseDimer:
                //IF INSIDE TOLERANCE RADIUS
                if (Vector3.Distance(transform.position, dimer01.transform.position) < tolerance)
                {
                    //IF HELD
                    if (transform.GetComponent<Grabbable>().BeingHeld && dimer01.GetComponent<Grabbable>().BeingHeld)
                    {
                        //Look at
                        LookAt(thisDimerForward, dimer01Forward.transform, cameraToDimer, cameraToDimer);

                        //Update Particle System
                        em[2].rateOverTime = CalculateParticleEmission(dimer01Distance);
                        em[3].rateOverTime = CalculateParticleEmission(dimer01Distance);
                        m[2].startLifetime = dimer01Distance * particleLifetime;
                        m[3].startLifetime = dimer01Distance * particleLifetime;

                        //Run blinking of Betasheets
                        blinkingTime += Time.deltaTime;
                        if (blinkingTime > Mathf.Clamp((1f / blinkingFrequency), blinkingDuration, float.MaxValue))
                        {
                            StartCoroutine(ZincPulse(blinkingDuration));
                            blinkingTime = 0;
                        }
                    }
                    //IF RELEASED
                    else
                    {
                        //Stop Particle System
                        ps[2].Stop();
                        ps[3].Stop();

                        hexamerAssemblySFX.stop(STOP_MODE.ALLOWFADEOUT);

                        //Dimer animation
                        StartCoroutine(DimerLerp(lerpDuration));
                        currentState = MonomerStates.DimerAnimation;
                    }
                }
                //IF OUTSIDE OF TOLERANCE RADIUS 
                else if (Vector3.Distance(transform.position, dimer01.transform.position) > tolerance)
                {
                    //IF HELD
                    if (transform.GetComponent<Grabbable>().BeingHeld && dimer01.GetComponent<Grabbable>().BeingHeld)
                    {
                        hexamerAssemblySFX.setParameterByName("mag_force", 0);
                        currentState = MonomerStates.GrabbedDimer;
                    }
                    //IF RELEASED
                    else
                    {
                        //Stop Particle System 
                        ps[2].Stop();
                        ps[3].Stop();
                        hexamerAssemblySFX.stop(STOP_MODE.ALLOWFADEOUT);
                        currentState = MonomerStates.IdleDimer;
                        
                    }
                }
                break;

            case MonomerStates.IdleTrimer:
                if (transform.GetComponent<Grabbable>().BeingHeld && dimer02.GetComponent<Grabbable>().BeingHeld)
                {
                    //Play Particle System
                    ps[4].Play();
                    ps[5].Play();

                    //Update Particle System 
                    em[4].rateOverTime = CalculateParticleEmission(dimer02Distance);
                    em[5].rateOverTime = CalculateParticleEmission(dimer02Distance);
                    m[4].startLifetime = dimer02Distance * particleLifetime;
                    m[5].startLifetime = dimer02Distance * particleLifetime;
                    hexamerAssemblySFX.start();
                    hexamerAssemblySFX.setParameterByName("mag_force", 0);
                    currentState = MonomerStates.GrabbedTrimer;
                }
                break;

            case MonomerStates.GrabbedTrimer:
                //IF EITHER MONOMERS ARE RELEASED
                if (!transform.GetComponent<Grabbable>().BeingHeld || !dimer02.GetComponent<Grabbable>().BeingHeld)
                {
                    //Stop Particle System
                    ps[4].Stop();
                    ps[5].Stop();
                    hexamerAssemblySFX.stop(STOP_MODE.ALLOWFADEOUT);
                    currentState = MonomerStates.IdleTrimer;
                }
                //IF INSIDE TOLERANCE RADIUS
                else if (Vector3.Distance(transform.position, dimer02.transform.position) < tolerance)
                {
                    hexamerAssemblySFX.setParameterByName("mag_force", 1);
                    currentState = MonomerStates.CloseTrimer;
                }
                //Look at
                LookAt(thisTrimerForward, dimer02Forward.transform, cameraToDimer, cameraToDimer);

                //Update Particle System
                em[4].rateOverTime = CalculateParticleEmission(dimer02Distance)/4;
                em[5].rateOverTime = CalculateParticleEmission(dimer02Distance)/4;
                m[4].startLifetime = dimer02Distance * particleLifetime;
                m[5].startLifetime = dimer02Distance * particleLifetime;
                break;

            case MonomerStates.CloseTrimer:
                //IF INSIDE TOLERANCE RADIUS
                if (Vector3.Distance(transform.position, dimer02.transform.position) < tolerance)
                {
                    //IF HELD
                    if (transform.GetComponent<Grabbable>().BeingHeld && dimer02.GetComponent<Grabbable>().BeingHeld)
                    {
                        //Look at
                        LookAt(thisTrimerForward, dimer02Forward.transform, cameraToDimer, cameraToDimer);

                        //Update Particle System
                        em[4].rateOverTime = CalculateParticleEmission(dimer02Distance);
                        em[5].rateOverTime = CalculateParticleEmission(dimer02Distance);
                        m[4].startLifetime = dimer02Distance * particleLifetime;
                        m[5].startLifetime = dimer02Distance * particleLifetime;

                        //Run blinking of Betasheets
                        blinkingTime += Time.deltaTime;
                        if (blinkingTime > Mathf.Clamp((1f / blinkingFrequency), blinkingDuration, float.MaxValue))
                        {
                            StartCoroutine(ZincPulse(blinkingDuration));
                            blinkingTime = 0;
                        }
                    }
                    //IF RELEASED
                    else
                    {
                        //Stop Particle System
                        ps[4].Stop();
                        ps[5].Stop();

                        hexamerAssemblySFX.stop(STOP_MODE.ALLOWFADEOUT);

                        //Dimer animation
                        StartCoroutine(TrimerLerp(lerpDuration));
                        currentState = MonomerStates.TrimerAnimation;
                    }
                }
                //IF OUTSIDE OF TOLERANCE RADIUS 
                else if (Vector3.Distance(transform.position, dimer02.transform.position) > tolerance)
                {
                    //IF HELD
                    if (transform.GetComponent<Grabbable>().BeingHeld && dimer02.GetComponent<Grabbable>().BeingHeld)
                    {
                        hexamerAssemblySFX.setParameterByName("mag_force", 0);
                        currentState = MonomerStates.GrabbedTrimer;
                    }
                    //IF RELEASED
                    else
                    {
                        //Stop Particle System 
                        ps[4].Stop();
                        ps[5].Stop();
                        hexamerAssemblySFX.stop(STOP_MODE.ALLOWFADEOUT);
                        currentState = MonomerStates.IdleTrimer;
                    }
                }
                break;
        }
    }

    public float CalculateParticleEmission(float distance)
    {
        return Mathf.Pow(Mathf.Abs(1 - (distance / maxDistance)) * particleIntensity, particlePow);
    }

    IEnumerator BetaSheetPulse(float duration)
    {
        HapticsManager.Instance.OneShotVibration(HapticsManager.VibrationPreset.OnGrab, ControllerHand.Right);
        HapticsManager.Instance.OneShotVibration(HapticsManager.VibrationPreset.OnGrab, ControllerHand.Left);
        thisMonomerMesh.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.monomerHighlighted);
        monomer01Mesh.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.monomerHighlighted);
        yield return new WaitForSeconds(duration);
        thisMonomerMesh.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.monomer);
        monomer01Mesh.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.monomer);
    }

    IEnumerator ZincPulse(float duration)
    {
        HapticsManager.Instance.OneShotVibration(HapticsManager.VibrationPreset.OnGrab, ControllerHand.Right);
        HapticsManager.Instance.OneShotVibration(HapticsManager.VibrationPreset.OnGrab, ControllerHand.Left);
        if (currentState== MonomerStates.CloseDimer)
        {
            dimer01MeshA.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.dimerHighlighted);
            dimer01MeshB.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.dimerHighlighted);

        }
        else if(currentState == MonomerStates.CloseTrimer)
        {
            dimer02MeshA.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.dimerHighlighted);
            dimer02MeshB.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.dimerHighlighted);
        }
        ToggleZinc(1);
        yield return new WaitForSeconds(duration);

        dimer01MeshA.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.dimer);
        dimer01MeshB.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.dimer);
        dimer02MeshA.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.dimer);
        dimer02MeshB.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.dimer);
        ToggleZinc(0);
    }

    IEnumerator MonomerLerp(float duration)
    {
        //CHANGE COLORS
        thisMonomerMesh.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.monomerHighlighted);
        monomer01Mesh.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.monomerHighlighted);

        //TURN OFF RIGID BODIES AND RELEASE GRABS
        if (monomer01.transform.GetComponent<Grabbable>().BeingHeld)
        {
            Grabber grabber1 = monomer01.transform.GetComponent<Grabbable>().GetPrimaryGrabber();
            monomer01.transform.GetComponent<Grabbable>().DropItem(grabber1, true, true);
        }
        monomer01.transform.GetComponent<Grabbable>().enabled = false;
        monomer01.transform.GetComponent<Rigidbody>().isKinematic = true;

        if (transform.GetComponent<Grabbable>().BeingHeld)
        {
            Grabber grabber2 = transform.GetComponent<Grabbable>().GetPrimaryGrabber();
            transform.GetComponent<Grabbable>().DropItem(grabber2, true, true);
        }
        transform.GetComponent<Grabbable>().enabled = false;
        transform.GetComponent<Rigidbody>().isKinematic = true; 

        //RECALIBRATE POSITION 
        transform.position = thisMonomerForward.transform.position;
        thisMonomerForward.localPosition = Vector3.zero;
        monomer01.transform.position = monomer01Forward.transform.position;
        monomer01Forward.transform.localPosition = Vector3.zero;

        Vector3 up = thisMonomerForward.up;
        
        Quaternion startR1 = thisMonomerForward.rotation;
        Quaternion startR2 = monomer01Forward.rotation;
        Quaternion targetR1 = Quaternion.LookRotation(monomer01.transform.position - this.transform.position, up);
        Quaternion targetR2 = Quaternion.LookRotation(this.transform.position - monomer01.transform.position, -up);

        float time = 0; 
        while(time < 0.7f){
            float t = time/ 0.7f;
            thisMonomerForward.rotation = Quaternion.Lerp(startR1, targetR1, t);
            monomer01Forward.rotation = Quaternion.Lerp(startR2, targetR2, t);
            time+= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //RECALIBRATE ROTATION
        Vector3 temp = thisMonomerForward.rotation.eulerAngles;
        Vector3 temp2 = monomer01Forward.rotation.eulerAngles;
        transform.rotation = thisMonomerForward.transform.rotation * Quaternion.Euler(-135, 0,0);
        thisMonomerForward.transform.rotation = Quaternion.Euler(temp);        
        monomer01.transform.rotation = monomer01Forward.rotation * Quaternion.Euler(-135, 0,0);
        monomer01Forward.transform.rotation = Quaternion.Euler(temp2);

        //MOVE TO TARGET
        Vector3 targetPosC = Vector3.Lerp(transform.position, monomer01.transform.position, 0.5f);
        transform.DOMove(targetPosC, duration);
        monomer01.transform.DOMove(targetPosC, duration);
        thisMonomerForward.DOLocalMove(new Vector3(1.2f, 0.6f, 0.6f), duration);
        monomer01Forward.DOLocalMove(new Vector3(.12f, .06f, .06f), duration);
    
        yield return new WaitForSeconds(duration);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Object_Emitters/dimer_binding", this.transform.position);

        //SET PARENT 
        monomer01.transform.SetParent(thisTrimerForward);

        if (isFirstMonomer)
        {
            //WAIT
            yield return new WaitForSeconds(1);

            //PLAY ANIMATION
            monomerGhost.SetActive(true);
            monomerGhost.transform.parent.GetComponent<MeshRenderer>().enabled = true;
            monomerAnimator.SetTrigger("Play");
            yield return new WaitForSeconds(5f);

            //FINISH 
            monomerGhost.SetActive(false);
            monomerGhost.transform.parent.GetComponent<MeshRenderer>().enabled = false;
        }
        
        //CHANGE COLORS
        thisMonomerMesh.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.dimer);
        monomer01Mesh.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.dimer);

        //TURN ON GRABBABLE
        transform.GetComponent<Grabbable>().enabled = true;
        transform.GetComponent<Rigidbody>().isKinematic = false;
        
        //ACTIVE ZINC
        zincTop.SetActive(true);
        zincBottom.SetActive(true);

        dimer01.SetActive(true);
        ps[2].Stop();
        ps[3].Stop();

        //SWITCH STATE
        currentState = MonomerStates.IdleDimer;
        QuestLogManager.Instance.NextBeat();
    }

    IEnumerator DimerLerp(float duration)
    {
        //LIGHT UP ZINC HIGHLIGHTS
        ToggleZinc(1);

        //TURN OFF RIGID BODIES
        if (transform.GetComponent<Grabbable>().BeingHeld)
        {
            Grabber grabber2 = transform.GetComponent<Grabbable>().GetPrimaryGrabber();
            transform.GetComponent<Grabbable>().DropItem(grabber2, true, true);
        }
        transform.GetComponent<Grabbable>().enabled = false;
        transform.GetComponent<Rigidbody>().isKinematic = true;

        if (dimer01.GetComponent<Grabbable>().BeingHeld)
        {
            Grabber grabber2 = dimer01.GetComponent<Grabbable>().GetPrimaryGrabber();
            dimer01.GetComponent<Grabbable>().DropItem(grabber2, true, true);
        }
        dimer01.GetComponent<Grabbable>().enabled = false;
        dimer01.GetComponent<Rigidbody>().isKinematic = true;

        //FINISH LOOK AT
        while(Vector3.Dot(thisDimerForward.forward, dimer01Forward.transform.forward) > -1)
        {
            Vector3 forward = transform.position - Camera.main.transform.position;
            LookAt(thisDimerForward, dimer01Forward.transform, forward, forward);
            yield return new WaitForEndOfFrame();
        }

        //RECALIBRATE VECTORS
        transform.rotation = thisDimerForward.rotation;
        thisDimerForward.rotation = transform.rotation;
        dimer01.transform.rotation = dimer01Forward.rotation;
        dimer01Forward.rotation = dimer01.transform.rotation;
        
        //THIS START VARIABLES 
        Quaternion thisStartRot = transform.rotation;

        //DIMER01 START VARIABLES 
        Vector3 dimerStartPos = dimer01.transform.position;
        Quaternion dimerStartRot = dimer01.transform.rotation;


        yield return new WaitForEndOfFrame();

        //CALCULATE LERP
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            dimer01.transform.position = Vector3.Lerp(dimerStartPos, transform.position, time / duration);
            dimer01.transform.rotation = Quaternion.Lerp(dimerStartRot, dimerStartRot * Quaternion.Euler(0, -60, 0), time / duration);
            yield return new WaitForEndOfFrame();
        }
        FMODUnity.RuntimeManager.PlayOneShot("event:/Object_Emitters/dimer_binding", this.transform.position);

        //SET PARENT 
        dimer01.transform.SetParent(thisTrimerForward);

        if (isFirstMonomer)
        {
            //ANIMATION STUFF 
            threeFoldAxis.SetActive(true);
            time = 0;
            while (time < 1)
            {
                threeFoldAxis.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(0.2f, 0.2f, 0.2f), time / 1);
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }


            //PLAY GHOST
            dimerGhost.SetActive(true);
            dimerAnimator.SetTrigger("PlayDimer01");
            yield return new WaitForSeconds(5f);
            dimerGhost.SetActive(false);
            threeFoldAxis.gameObject.SetActive(false);
        }

        //TURN ON GRABBABLE
        transform.GetComponent<Grabbable>().enabled = true;
        transform.GetComponent<Rigidbody>().isKinematic = false;

        //TURN OFF ZINC HIGHLIGHT
        ToggleZinc(0);

        dimer02.SetActive(true);
        ps[4].Stop();
        ps[5].Stop();

        //SWITCH STATES
        currentState = MonomerStates.IdleTrimer;
        QuestLogManager.Instance.NextBeat();
    }

    IEnumerator TrimerLerp(float duration)
    {
        //LIGHT UP ZINCS HIGHLIGH
        ToggleZinc(0);

        //TURN OFF RIGID BODIES
        if (transform.GetComponent<Grabbable>().BeingHeld)
        {
            Grabber grabber2 = transform.GetComponent<Grabbable>().GetPrimaryGrabber();
            transform.GetComponent<Grabbable>().DropItem(grabber2, true, true);
        }
        transform.GetComponent<Grabbable>().enabled = false;
        transform.GetComponent<Rigidbody>().isKinematic = true;

        if (dimer02.GetComponent<Grabbable>().BeingHeld)
        {
            Grabber grabber2 = dimer02.GetComponent<Grabbable>().GetPrimaryGrabber();
            dimer02.GetComponent<Grabbable>().DropItem(grabber2, true, true);
        }
        dimer02.GetComponent<Grabbable>().enabled = false;
        dimer02.GetComponent<Rigidbody>().isKinematic = true;

        //FINISH LOOK AT
        while (Vector3.Dot(thisTrimerForward.forward, dimer02Forward.transform.forward) > -1)
        {
            Vector3 forward = transform.position - Camera.main.transform.position;
            LookAt(thisTrimerForward, dimer02Forward.transform, forward, forward);
            yield return new WaitForEndOfFrame();
        }

        //RECALIBRATE VECTORS
        transform.rotation = thisTrimerForward.rotation;
        thisTrimerForward.rotation = transform.rotation;
        dimer02.transform.rotation = dimer02Forward.rotation;
        dimer02Forward.rotation = dimer02.transform.rotation;

        //DIMER02 START VARIABLES 
        Vector3 dimerStartPos = dimer02.transform.position;
        Quaternion dimerStartRot = dimer02.transform.rotation;


        yield return new WaitForEndOfFrame();

        //CALCULATE LERP
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            dimer02.transform.position = Vector3.Lerp(dimerStartPos, transform.position, time / duration);
            dimer02.transform.rotation = Quaternion.Lerp(dimerStartRot, dimerStartRot * Quaternion.Euler(0, 15, 0), time / duration);
            yield return new WaitForEndOfFrame();
        }
        FMODUnity.RuntimeManager.PlayOneShot("event:/Object_Emitters/dimer_binding", this.transform.position);
        //SET PARENT 
        dimer02.transform.SetParent(thisTrimerForward);

        //CHANGE COLORS
        thisMonomerMesh.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.hexamer3);
        monomer01Mesh.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.hexamer3);
        dimer01MeshA.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.hexamer3);
        dimer01MeshB.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.hexamer3);
        dimer02MeshA.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.hexamer3);
        dimer02MeshB.GetComponent<MeshRenderer>().SetMaterials(colorProfiles.hexamer3);

        if (isFirstMonomer)
        {
            //ANIMATION STUFF 
            threeFoldAxis.SetActive(true);
            time = 0;
            while (time < 1)
            {
                threeFoldAxis.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(0.2f, 0.2f, 0.2f), time / 1);
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            //PLAY GHOST
            dimerGhost.SetActive(true);
            dimerAnimator.SetTrigger("PlayDimer02");
            yield return new WaitForSeconds(5f);
            dimerGhost.SetActive(false);
            threeFoldAxis.gameObject.SetActive(false);
        }
        
        //LIGHT UP ZINC HIGHLIGHTS
        ToggleZinc(0);
        
        //SWITCH STATES
        currentState = MonomerStates.IdleTrimer;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        //FINAL LERP
        time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, finalLerpTarget.position, time / duration);
            transform.rotation = Quaternion.Lerp(startRotation, finalLerpTarget.rotation, time / duration);
            yield return new WaitForEndOfFrame();
        }
        if (nextMonomerA != null)
        {
            nextMonomerA.SetActive(true);
            nextMonomerB.SetActive(true);
        }
        crystalGameManager.FinishMonomer();
        QuestLogManager.Instance.NextBeat();
    }

    public void ToggleZinc(int state)
    {
        //LIGHT UP ZINCS
        zincTop.GetComponent<MeshRenderer>().material = colorProfiles.zinc[state];
        zincBottom.GetComponent<MeshRenderer>().material = colorProfiles.zinc[state];
    }

    public void ChangeColor(List<Material> colorProfile)
    {
        thisMonomerMesh.GetComponent<MeshRenderer>().SetMaterials(colorProfile);
        monomer01Mesh.GetComponent<MeshRenderer>().SetMaterials(colorProfile);
        dimer01MeshA.GetComponent<MeshRenderer>().SetMaterials(colorProfile);
        dimer01MeshB.GetComponent<MeshRenderer>().SetMaterials(colorProfile);
        dimer02MeshA.GetComponent<MeshRenderer>().SetMaterials(colorProfile);
        dimer02MeshB.GetComponent<MeshRenderer>().SetMaterials(colorProfile);
    }

}
