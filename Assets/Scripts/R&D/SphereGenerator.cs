using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereGenerator : MonoBehaviour
{
    public int tierOneCount = 97;
    public int tierTwoCount = 97;
    public int tierThreeCount = 97;
    public int tierTwoClusterCount = 3;
    public int tierThreeClusterCount = 7;
    public float radius = 500;
    private List<List<Matrix4x4>> renderBatches = new List<List<Matrix4x4>>();
    private int batchIndex = 0;
    public List<Vector3> tierOnePoints = new List<Vector3>();
    public List<Vector3> tierTwoPoints = new List<Vector3>(); 
    public List<Vector3> tierThreePoints = new List<Vector3>(); //Where membrane lives. 
    public List<Vector3> tierThreeRandomness = new List<Vector3>(); //Where membrane lives. 
    public float randomness = 0.5f;
    public Vector3[] t3CachedList;
    public Vector3[] t2CachedList;
    public Mesh membrane;
    public Material membraneMaterial;
    public Transform player;
    public Vector3 cachedPlayerPosition; 
    float goldenRatio; 
    float angleIncrement; 
    public GameObject membraneGenerator;

    private void Start() {
        for(int x = 0; x < tierThreeCount; x++){
            tierThreeRandomness.Add(new Vector3(Random.Range(-randomness, randomness), Random.Range(-randomness, randomness), Random.Range(-randomness, randomness)));
        }

        t3CachedList = new Vector3 [tierThreeClusterCount];
        t2CachedList = new Vector3[tierTwoClusterCount];
        cachedPlayerPosition = player.position;
        goldenRatio = (1 + Mathf.Sqrt (5)) / 2;
        angleIncrement = Mathf.PI * 2 * goldenRatio;
        CreateTierOnePoints();
        ResetTierTwoAndThree();
    }

    void Update(){

        if(Vector3.Distance(player.position, cachedPlayerPosition) > 1){
            ResetTierTwoAndThree();
            cachedPlayerPosition = player.position;
        }
        RenderBatches();
        Vector3 maskTarget = transform.position + (player.position - transform.position).normalized * radius;
        transform.GetComponent<Renderer>().sharedMaterial.SetVector("_Target", maskTarget);
        Test();

        foreach (Vector3 a in tierOnePoints)
        {
            Debug.DrawRay(a, -Vector3.up * 10, Color.red);
        }

        foreach (Vector3 b in tierTwoPoints)
        {
            Debug.DrawRay(b, -Vector3.up * 10, Color.blue);
        }

    }

    public void ResetTierTwoAndThree(){
        tierOnePoints.Sort((a,b) => Vector3.Distance(a,player.position).CompareTo(Vector3.Distance(b,player.position)));
        Vector3[] t2SpawnLocation = new Vector3[tierTwoClusterCount];
        for (int x = 0; x < t2SpawnLocation.Length; x++)
        {
            t2SpawnLocation[x] = tierOnePoints[x];
        }

        if(t2SpawnLocation != t2CachedList)
        {
            createTierTwoPoints(t2SpawnLocation);
            t2CachedList = t2SpawnLocation;
        }

        
        tierTwoPoints.Sort((a,b) => Vector3.Distance(a,player.position).CompareTo(Vector3.Distance(b,player.position)));
        Vector3[] t3SpawnLocation = new Vector3[tierThreeClusterCount];
        for(int x = 0; x < t3SpawnLocation.Length; x++)
        {
            t3SpawnLocation[x] = tierTwoPoints[x];
        }

        if (t3SpawnLocation != t3CachedList)
        {
            createTierThreePoints(t3SpawnLocation);
            t3CachedList = t3SpawnLocation;
        }
        
    }

    public void CreateTierOnePoints(){
        tierOnePoints.Clear();
        for (int i = 0; i < tierOneCount; i++) {
            float t = (float) i / tierOneCount;
            float inclination = Mathf.Acos (1 - 2 * t);
            float azimuth = angleIncrement * i;
            float x = Mathf.Sin (inclination) * Mathf.Cos (azimuth);
            float y = Mathf.Sin (inclination) * Mathf.Sin (azimuth);
            float z = Mathf.Cos (inclination);
            tierOnePoints.Add(new Vector3 (x, y, z) * radius + transform.position);
        }
    }

    public void Test(){
        for (int i = 0; i < 50; i++) {
            float t = ((float) i / 50) * (1f/500f);
            float inclination = Mathf.Acos (1 - 2 * t);
            float azimuth = angleIncrement * i;
            float x = Mathf.Sin (inclination) * Mathf.Cos (azimuth);
            float y = Mathf.Sin (inclination) * Mathf.Sin (azimuth);
            float z = Mathf.Cos (inclination);
            Vector3 localPosition = new Vector3 (x, y, z) * radius + transform.position;
        }
    }


    public void createTierTwoPoints(Vector3[] t2SpawnLocation){
        tierTwoPoints.Clear();
            foreach(Vector3 s in t2SpawnLocation)
            {
                for (int i = 0; i < tierTwoCount; i++)
                {
                    float t = ((float)i / tierTwoCount) * (1.5f / tierOneCount);
                    float inclination = Mathf.Acos(1 - 2 * t);
                    float azimuth = angleIncrement * i;
                    float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
                    float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
                    float z = Mathf.Cos(inclination);
                    Vector3 localPositon = new Vector3(x, y, z) * radius + transform.position;
                    Vector3 crossAxis = Vector3.Cross(Vector3.forward, s);
                    Vector3 globalPosition = Quaternion.AngleAxis(Vector3.Angle(s, Vector3.forward), crossAxis) * localPositon;
                    tierTwoPoints.Add(globalPosition);
                }
            
            }
    }

    public void createTierThreePoints(Vector3[] t3SpawnLocation){
        Vector3 membranePosition = (membraneGenerator.transform.position - transform.position).normalized * radius;
        float membraneRadius = membraneGenerator.GetComponent<VectorExploration>().globalRadius;
        renderBatches.Clear();
        renderBatches.Add(new List<Matrix4x4>());
        batchIndex = 0;
        foreach(Vector3 s in t3SpawnLocation){
            for (int i = 0; i < tierThreeCount; i++) {
            float t =  ((float) i / tierThreeCount) * (1.5f/tierOneCount) * (1.5f/tierTwoCount);
            float inclination = Mathf.Acos (1 - 2 * t);
            float azimuth = angleIncrement * i;
            float x = Mathf.Sin (inclination) * Mathf.Cos (azimuth);
            float y = Mathf.Sin (inclination) * Mathf.Sin (azimuth);
            float z = Mathf.Cos (inclination);
            Vector3 localPositon = new Vector3 (x, y, z) * radius + transform.position;
            Vector3 crossAxis = Vector3.Cross(Vector3.forward, s);
            Vector3 right = Vector3.Cross(transform.position - localPositon, Vector3.up).normalized;
            Vector3 up = Vector3.Cross(right, transform.position - localPositon).normalized;
            
            Vector3 globalPosition = Quaternion.AngleAxis(Vector3.Angle(s - transform.position, Vector3.forward), crossAxis) * localPositon + 
                (right * tierThreeRandomness[i].x) 
                //+ (up * tierThreeRandomness[i].y)
                ;
            

            Quaternion rotation = Quaternion.LookRotation(transform.position - globalPosition, up) * Quaternion.AngleAxis(90,right);
            

                if(Vector3.Distance(globalPosition, membranePosition) > membraneRadius){
                    tierThreePoints.Add(globalPosition);
                    if(renderBatches[batchIndex].Count > 999)
                    {
                        batchIndex++;
                        renderBatches.Add(new List<Matrix4x4>());
                    }
                    else{
                        renderBatches[batchIndex].Add(item: Matrix4x4.TRS
                        (globalPosition, rotation, Vector3.one));
                    }
                }
                    

            }
        }
    }

    void RenderBatches()
    {
        for(int b = 0; b < renderBatches.Count; b++){
            for (int i = 0; i < membrane.subMeshCount; i++)
            {
                Graphics.DrawMeshInstanced(membrane, i, membraneMaterial, renderBatches[b]);
            }
        }
        foreach(Vector3 point in tierThreePoints){
            //Debug.DrawRay(point, Vector3.forward, Color.blue);
        }
    }

}
