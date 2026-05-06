using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsulinMovementSystem : MonoBehaviour
{
    //Settings
    [Tooltip("Settings for monomer movement. (Saved as scriptable game object)")]
    [SerializeField] private BoidSettings settings;
    public bool hasTarget;
    public bool hasLifetimeTimer;
    public bool useDeltaTime = true;
    public float refreshEveryXSecond = 1;
    public float lifetime = 30;
    public float fadingDuration;

    //Physics
    private Vector3 _currentDirection = Vector3.zero;

    [SerializeField] private float _acceleration = 1;
    [SerializeField] private float smoothRotation = 5; //Smaller number more it avoids.
    [SerializeField] private float targetInfluence = 5; //Smaller number more it attracts.

    //DEBUG
    public float _currentSpeed = 0;
    public Transform[] _targetLocation;
    public int _curTargetIndex =1;
    public float timer = 0;
    public float lifetimeTimer =0;
    public float fadingTimer;
    public Vector3 _centerPivot = Vector3.zero;
    public List<GameObject> _child;

    //Private Variables
    RaycastHit[] nearbyObjects = new RaycastHit[0];
    float timeFromLastFrame;

    private void Start()
    {
        if (hasTarget) //Input in inspector.
        {
            GameObject root = GameObject.Find("hexamerPrefabDisolvePath");

            if(root == null){
                Debug.Log("Scene does not have a hexamerPrefabDisolvePath Object");
            }
            _targetLocation = root.GetComponentsInChildren<Transform>();
        }
        if (hasLifetimeTimer)
        {
            StartCoroutine(timerTillDeath()); //Used for video recording 
        }
    }

    private void Update()
    {
        //Use DeltaTime for VR builds, but not for video renders. 
        if (useDeltaTime){
            timeFromLastFrame = Time.deltaTime;
        } 
        else {
            timeFromLastFrame = 1f / 24f;
        }

        UpdateMovement();
        transform.Translate(_currentDirection.normalized * _currentSpeed * timeFromLastFrame);
        //Debug.DrawRay(transform.position, _currentDirection.normalized * _currentSpeed, Color.green);

        //Fake parenting so child follows where you are. 
        if (_child != null)
        {
            for (int x = 0; x < _child.Count; x++)
            {
                _child[x].transform.position = this.transform.position;
            }
        }
    }

    private void UpdateMovement()
    {

        //Does a sphere cast at refreshEveryXSecond
        if (timer <= 0) {
            nearbyObjects = Physics.SphereCastAll(transform.position + _centerPivot, settings.collisionAvoidDst, Vector3.forward, settings.collisionAvoidDst);
            timer = refreshEveryXSecond;

            //Checks if you have passed the target index
            if (hasTarget && Vector3.Dot(_targetLocation[_curTargetIndex].forward, transform.position - _targetLocation[_curTargetIndex].position) >  0) {
                if(_curTargetIndex < _targetLocation.Length -1){
                    _curTargetIndex ++;
                }
            }
        }
        timer -= timeFromLastFrame;
        
        Vector3 tempDir = new Vector3();
        int tempCount = 0;
        

        //All the vector math. Grabs all nearby objects, finds average velocity, and adds that to current velocity
        if(nearbyObjects.Length > 0)
        {
            for(int x =0; x < nearbyObjects.Length; x++)
            {
                tempDir += nearbyObjects[x].transform.position;
                tempCount++;
            }
        }
        _currentDirection -= ((tempDir / tempCount) - transform.position) * timeFromLastFrame;
        if(hasTarget)
        {
            _currentDirection += (_targetLocation[_curTargetIndex].transform.position - transform.position).normalized * timeFromLastFrame * targetInfluence;
        }
        _currentDirection = Vector3.ClampMagnitude(_currentDirection, smoothRotation);
        _currentSpeed = Mathf.Clamp((_currentSpeed + _acceleration* timeFromLastFrame), 0, settings.maxSpeed);
    }

    //USED TO FADEOUT MATERIAL FOR VIDEO RENDER
    IEnumerator timerTillDeath()
    {
        while(lifetimeTimer < lifetime)
        {
            lifetimeTimer += 1f / 24f;
            yield return new WaitForEndOfFrame();      
        }
        while(fadingTimer < fadingDuration)
        {
            float opacity = 1 - fadingTimer / fadingDuration * 0.5019f;
            this.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, opacity);
            this.transform.GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(1, 1, 1, opacity);
            fadingTimer += 1f / 24f;
            yield return new WaitForEndOfFrame();
        }
        Destroy(this.gameObject); 
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
