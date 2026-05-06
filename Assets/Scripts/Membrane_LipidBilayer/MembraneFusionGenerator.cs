using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


#if UNITY_EDITOR
[CustomEditor(typeof(MembraneFusionGenerator))]
public class MembraneFusionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MembraneFusionGenerator myScript = (MembraneFusionGenerator)target;
        
        if(GUILayout.Button("Reset Membrane"))
        {
            myScript.ResetMembrane();
        }
        if(GUILayout.Button("Update Properties"))
        {
            myScript.UpdateProperties();
        }
    }
}
#endif

public class MembraneFusionGenerator : MonoBehaviour
{
    //MATH
    float goldenRatio;
    float angleIncrement;

    //INSTANCING 
    public int membraneCount = 10000;
    public float vesicleRadius = 125; //From center of vesicle
    public Mesh membrane;
    public Material vesicleMembraneMaterial;
    public Material cellMembraneMaterial;
    List<VesicleMembrane> vesicleMembraneList = new List<VesicleMembrane>();
    List<List<Matrix4x4>> vesicleRenderBatches = new List<List<Matrix4x4>>();
    List<CellMembrane> cellMembraneList = new List<CellMembrane>();
    List<List<Matrix4x4>> cellRenderBatches = new List<List<Matrix4x4>>();
    private int batchCount = 0;

    //MEMBRANE PROPERTIES
    [Tooltip("The speed in which membranes rotates during fusion")]
    [SerializeField] private float rotationSpeed = 90;
    [Tooltip("The speed in which the membrane progesses during fusion")]
    [SerializeField] private float progressionSpeed = 10; 
    [Tooltip("vesicleRadius in which the membrane is created")]
    [SerializeField] float spawnvesicleRadius = 150;
    [Tooltip("Offsets the the distance needed between vesicle and cell for fusion to begin")]
    [SerializeField] float fusionIntersectionOffset = -15;
    [Tooltip("The amount of randomness given to the position of each membrane (Create a random Vector3")]
    [SerializeField] private float randomness = .5f;
    private Vector3 cellMembranePositionOffset; 

    //REFRENCES 
    public Transform cellPlane;
    public Transform vesicleSphere;

    //PRIVATE VARIABLES
    private Vector3 cellPlaneStartingPos;

    void Start()
    {
        vesicleSphere = this.transform;
        //SET UP MATH
        goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        angleIncrement = Mathf.PI * 2 * goldenRatio;

        //SET UP MEMBRANE
        CreateVesicleMembrane();
        CreateCellMembrane();
        UpdateProperties();
    }

    private void Update()
    {
        ApplyMovementAndUpdateRenderVesicleBatches();
        ApplyMovementAndUpdateRenderCellBatches();
        RenderAllBatches();
        RenderAllCellBatches();
    }

    void CreateVesicleMembrane()
    {
        for (int i = 1; i < membraneCount; i++)
        {
            //COVERTS SPAWN vesicleRadius INTO RATIO FOR SPHERE INSTANTIATION
            float spawnRatioOfSphere = (Mathf.PI * Mathf.Pow(spawnvesicleRadius, 2)) / (4 * Mathf.PI * Mathf.Pow(vesicleRadius, 2));

            //GENERATES POINTS ON A SPHERE
            float t = ((float)i / membraneCount) * spawnRatioOfSphere;
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = angleIncrement * i;
            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth) + (Random.Range(-randomness, randomness)/ vesicleRadius);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth) + (Random.Range(-randomness, randomness)/ vesicleRadius);
            float z = Mathf.Cos(inclination) + (Random.Range(-randomness, randomness)/ vesicleRadius);

            Vector3 membraneOffset = new Vector3(x, y, z) * vesicleRadius;

            vesicleMembraneList.Add(new VesicleMembrane(vesicleSphere, vesicleRadius, cellPlane, membraneOffset, rotationSpeed, progressionSpeed));
        }
    }

    public void CreateCellMembrane()
    {
        cellPlaneStartingPos = cellPlane.position;
        for (int i = 1; i < membraneCount; i++){
            float dst = Mathf.Pow(i / (membraneCount - 1f), 0.5f);
            float angle = 2 * Mathf.PI * 1.6180f * i;
            float x = dst * Mathf.Cos(angle) * spawnvesicleRadius + Random.Range(-randomness, randomness);
            float z = dst * Mathf.Sin(angle) * spawnvesicleRadius + Random.Range(-randomness, randomness);

            Vector3 membraneOffset = new Vector3(x,0,z);
            cellMembraneList.Add(new CellMembrane(cellPlane, membraneOffset, rotationSpeed, progressionSpeed));
        }
    }

    public void ResetMembrane(){
        vesicleMembraneList.Clear();
        CreateVesicleMembrane();
        cellMembraneList.Clear();
        CreateCellMembrane();
        UpdateProperties();
    }

    public void ApplyMovementAndUpdateRenderVesicleBatches()
    {
        batchCount = 0;
        vesicleRenderBatches.Clear();
        vesicleRenderBatches.Add(new List<Matrix4x4>());
        
        
        foreach(VesicleMembrane m in vesicleMembraneList)
        {  
            m.UpdatePosition(fusionIntersectionOffset);       

            if (vesicleRenderBatches[batchCount].Count >= 999)
            {
                batchCount++;
                vesicleRenderBatches.Add(new List<Matrix4x4>()); 
            }
            vesicleRenderBatches[batchCount].Add(Matrix4x4.TRS(m.position, m.rotation, Vector3.one));
        }
    }

    public void ApplyMovementAndUpdateRenderCellBatches()
    {
        batchCount = 0;
        cellRenderBatches.Clear();
        cellRenderBatches.Add(new List<Matrix4x4>());
        
        foreach(CellMembrane m in cellMembraneList)
        {  
            //CALCULATES DIRECTION VECTORS BASED ON CELL MEMBRANE
            Vector3 startUp = cellPlane.forward;
            Vector3 forward = (cellPlane.position - m.position).normalized;

            m.UpdatePosition(transform.position, vesicleRadius, cellPlane, fusionIntersectionOffset);       

            if (cellRenderBatches[batchCount].Count >= 999)
            {
                batchCount++;
                cellRenderBatches.Add(new List<Matrix4x4>()); 
            }
            cellRenderBatches[batchCount].Add(Matrix4x4.TRS(m.position, m.rotation, Vector3.one));
        }
    }

    public void RenderAllBatches()
    {
        foreach(List<Matrix4x4> batch in vesicleRenderBatches)
        {
            for(int x = 0; x < membrane.subMeshCount; x++)
            {
                Graphics.DrawMeshInstanced(membrane, x, vesicleMembraneMaterial, batch);
            }
        }
    }

      public void RenderAllCellBatches()
    {
        foreach(List<Matrix4x4> batch in cellRenderBatches)
        {
            for(int x = 0; x < membrane.subMeshCount; x++)
            {
                Graphics.DrawMeshInstanced(membrane, x, cellMembraneMaterial, batch);
            }
        }
    }

    public void UpdateProperties()
    {
        foreach (VesicleMembrane m in vesicleMembraneList)
        {
            m.UpdateProperties(rotationSpeed, progressionSpeed);
        }

        foreach (CellMembrane m in cellMembraneList)
        {
            m.UpdateProperties(rotationSpeed, progressionSpeed);
        }
    }

}
