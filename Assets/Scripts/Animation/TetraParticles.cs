using System.Collections;
using System.Collections.Generic;
using System.Drawing.Design;
using Unity.Services.Analytics;
using UnityEngine;

public class TetraParticles : MonoBehaviour
{
    //References 
    private Vector3 positionOffset;
    private Transform tetraCell;
    private Quaternion rotationOffset;

    //CURRENT STATE
    public bool assigned = false;
    public enum MovementStates { Default, MovingToTetra, LerpingToTetra, Complete }
    public MovementStates currentState;


    //MOVEMENT VARIABLES 
    public float currentSpeed;
    private Vector3 currentDir;
    private Transform target;
    [SerializeField] private float maxSpeed = 8;
    [SerializeField] private Vector3 rotation;

    [SerializeField] float scaleUpTime = .5f;

    //SELF DESTRUCT
    [SerializeField] private float lifetime = 30f;
    private float time;
    private Vector3 finalScale;
    public bool scalingUp = true;


    public void SetParameters(float maxSpeed, Vector3 rotation, Vector3 scale)
    {
        this.maxSpeed = maxSpeed;
        this.currentSpeed = maxSpeed;
        finalScale = scale;
        transform.localScale = Vector3.zero;
        this.rotation = rotation;
        currentState = MovementStates.Default;

    }

    void ApplyMovement()
    {
        transform.Translate(-Vector3.forward * currentSpeed * Time.deltaTime, Space.World);
    }

    void ApplyRotation()
    {
        transform.Rotate(rotation * Time.deltaTime);
    }

    void Update()
    {
        if (scalingUp) {
            float scaleT = Mathf.Clamp01(Mathf.InverseLerp(0, scaleUpTime, time));
            transform.localScale = finalScale*scaleT;
            if (scaleT >= 1f) {
                scalingUp = false;
            }
        }
        switch (currentState)
        {
            case MovementStates.Default:
                ApplyMovement();
                ApplyRotation();
                break;

            //GoToTetra() GETS CALLED FROM TETRACELLPARTICLES

            case MovementStates.MovingToTetra:
                Vector3 targetPos = tetraCell.position + positionOffset;
                float distToTarget = Vector3.Distance(this.transform.position, targetPos);
                float duration = (distToTarget / currentSpeed) * 1.5f;
                StartCoroutine(LerpOverTime(duration, transform.position, transform.rotation));
                
                currentState = MovementStates.LerpingToTetra;
                break;

            case MovementStates.LerpingToTetra:
                break;

            case MovementStates.Complete:
                break;
        }
        time += Time.deltaTime;
        if(time > lifetime)
        {
            Destroy(this.gameObject);
        }
    }

    public void GoToTetra(Vector3 positionOffset, Transform tetraCell, Quaternion rotationOffset)
    {
        this.tetraCell = tetraCell;
        this.positionOffset = positionOffset; 
        this.rotationOffset = rotationOffset;   
        currentState = MovementStates.MovingToTetra;
    }

    public IEnumerator LerpOverTime(float duration, Vector3 startPosition, Quaternion startRotation)
    {
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            //float lerpT = Mathf.Sin((timeElapsed / duration) * (Mathf.PI / 2));
            float lerpT = timeElapsed / duration;
            transform.position = Vector3.Lerp(startPosition, tetraCell.position + tetraCell.forward * positionOffset.z + tetraCell.right * positionOffset.x + tetraCell.up * positionOffset.y, Mathf.Pow(lerpT,3f));
            transform.localRotation = Quaternion.Lerp(transform.localRotation, tetraCell.rotation * rotationOffset, Mathf.Pow(lerpT, 3f));

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = tetraCell.position + tetraCell.forward * positionOffset.z + tetraCell.right * positionOffset.x + tetraCell.up * positionOffset.y;

        this.transform.parent = tetraCell;
        transform.localPosition = positionOffset / finalScale.x;
        transform.localRotation = rotationOffset;
        FMODUnity.RuntimeManager.PlayOneShot("event:/Intro_SFX/tetra_impact", this.transform.position);
        currentState = MovementStates.Complete;
    }
}
