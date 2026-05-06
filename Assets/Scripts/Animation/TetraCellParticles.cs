using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using UnityEngine;
using FMOD.Studio;
using FMOD;
using FMODUnity;

public class TetraCellParticles : MonoBehaviour
{
    private float velocity;
    private Vector3 rotation;
    List<Vector3> slotPositionOffset;
    List<Quaternion> slotRotationOffset;
    public float searchRadius = 10.0f;
    public List<Transform> nearbyObjects;
    float timer = 0;
    public float timeOfAssembly = 5;
    public bool assembled = false;
    private EventInstance layer1;
    private EventInstance layer2;
    private EventInstance oneshot;

    //AUDIO
    float closestTetraDuration = 0;
    float furthestTetraDuration = 0;

    //SELF DESTRUCT
    [SerializeField] private float lifetime = 31f;
    private float time;

    private void Start()
    {
        layer1 = FMODUnity.RuntimeManager.CreateInstance("event:/Intro_SFX/tetra_cell_assemble_layer_1");
        layer2 = FMODUnity.RuntimeManager.CreateInstance("event:/Intro_SFX/tetra_cell_assemble_layer_2");
        oneshot = FMODUnity.RuntimeManager.CreateInstance("event:/Intro_SFX/tetra_cell_assemble_oneshots");
    }

    public void SetParameters(float velocity, Vector3 rotation, Vector3 scale)
    {
        this.velocity = velocity;
        this.rotation = rotation;
        this.transform.localScale = scale;
        slotPositionOffset = new List<Vector3>();
        slotRotationOffset = new List<Quaternion>();

        for (int i = 0; i < transform.childCount; i++)
        {
            slotPositionOffset.Add(transform.GetChild(i).transform.position - this.transform.position);
            slotRotationOffset.Add(transform.GetChild(i).transform.rotation);
        }
    }

    void Update()
    {
        layer1.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
        layer2.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
        oneshot.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));

        transform.Translate(-Vector3.forward * velocity * Time.deltaTime, Space.World);
        transform.Rotate(rotation * Time.deltaTime);
        timer += Time.deltaTime;
        if(timer > timeOfAssembly && assembled == false)
        {
            AssembleCell();
            assembled = true;
        }
        if (time > lifetime)
        {
            StopAllCoroutines();
            StartCoroutine(WaitAndDestroy(7));
        }
    }

    IEnumerator WaitAndDestroy(float seconds)
    {
        layer1.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        layer2.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        oneshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        yield return new WaitForSeconds(seconds);
        Destroy(this.gameObject);
    }

    public void AssembleCell()
    {
        nearbyObjects = new List<Transform>();

        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius);

        foreach (Collider collider in colliders)
        {
            TetraParticles tp = collider.gameObject.GetComponent<TetraParticles>();

            if (nearbyObjects.Count >= slotPositionOffset.Count)
            {
                break; 
            }

            if (tp != null && tp.assigned == false && tp.scalingUp == false)
            {
                tp.assigned = true;
                nearbyObjects.Add(collider.transform);
            }
        }

        //ORDER WITH INDEX 0 BEING CENTER OF CELL 
        nearbyObjects = nearbyObjects.OrderBy(t => (t.position - transform.position).sqrMagnitude).ToList();

        for (int x = 0; x < nearbyObjects.Count; x++) 
        {
            //START TETRA ANIMATIONS 
            nearbyObjects[x].GetComponent<TetraParticles>().GoToTetra(slotPositionOffset[x], this.transform, slotRotationOffset[x]);

            if(x == 0)
            {
                float distToTarget = Vector3.Distance(nearbyObjects[x].position, this.transform.position + slotPositionOffset[x]);
                closestTetraDuration = (distToTarget / nearbyObjects[x].GetComponent<TetraParticles>().currentSpeed) * 1.5f;
            }
            else if(x == nearbyObjects.Count - 1)
            {
                float distToTarget = Vector3.Distance(nearbyObjects[x].position, this.transform.position + slotPositionOffset[x]);
                furthestTetraDuration = (distToTarget / nearbyObjects[x].GetComponent<TetraParticles>().currentSpeed) * 1.5f;
            }
        }
        StartCoroutine(PlayAudio());
    }

    IEnumerator PlayAudio()
    {
        
        layer1.start();
        layer1.release();
        //UnityEngine.Debug.DrawRay(transform.position, Vector3.up * 30, Color.white, 5f);
        yield return new WaitForSeconds(closestTetraDuration);
        //UnityEngine.Debug.DrawRay(transform.position, Vector3.up * 30, Color.yellow, 5f);
        layer2.start();
        layer2.release();
        yield return new WaitForSeconds(furthestTetraDuration - closestTetraDuration);
        //UnityEngine.Debug.DrawRay(transform.position, Vector3.up * 30, Color.red, 5f);
        layer1.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        layer2.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        oneshot.start();
        oneshot.release();
    }

}
