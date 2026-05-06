using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


#if UNITY_EDITOR
[CustomEditor(typeof(WholeCellMembraneGenerator))]
public class ExampleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WholeCellMembraneGenerator myScript = (WholeCellMembraneGenerator)target;

        if (GUILayout.Button("Reset Membrane"))
        {
#if UNITY_STANDALONE_WIN
            myScript.ResetMembrane();
#endif
        }
    }
}
#endif

public class WholeCellMembraneGenerator : MonoBehaviour
{
    //MATH
    float goldenRatio;
    float angleIncrement;

    //INSTANCING 
    public int membraneCount;
    public float wholeCellRadius;
    public Mesh membrane;
    public Material membraneMaterial;
    List<IntroMembrane> membraneList = new List<IntroMembrane>();
    List<List<Matrix4x4>> renderBatches = new List<List<Matrix4x4>>();
    private int batchCount = 0;

    //MEMBRANE PROPERTIES
    [SerializeField] float spawnRadius;
    [SerializeField] private float randomness;

    //REFRENCES 
    public Transform targetToGenerate;
    public Transform wholeCellPivot;

#if UNITY_STANDALONE_WIN 
    void Start()
    {
        wholeCellPivot = this.transform;
        //SET UP MATH
        goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        angleIncrement = Mathf.PI * 2 * goldenRatio;

        //SET UP MEMBRANE
        CreateMembrane();
    }
    private void Update()
    {
        ApplyMovementAndUpdateRenderVesicleBatches();
        RenderAllBatches();
    }

    void CreateMembrane()
    {
        for (int i = 1; i < membraneCount; i++)
        {
            //COVERTS SPAWN RADIUS INTO RATIO FOR SPHERE INSTANTIATION
            float spawnRatioOfSphere = (Mathf.PI * Mathf.Pow(spawnRadius, 2)) / (4 * Mathf.PI * Mathf.Pow(wholeCellRadius, 2));

            //GENERATES POINTS ON A SPHERE
            float t = ((float)i / membraneCount) * spawnRatioOfSphere;
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = angleIncrement * i;
            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth) + (Random.Range(-randomness, randomness) / wholeCellRadius);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth) + (Random.Range(-randomness, randomness) / wholeCellRadius);
            float z = Mathf.Cos(inclination) + (Random.Range(-randomness, randomness) / wholeCellRadius);

            Vector3 membraneOffset = new Vector3(x, y, z) * wholeCellRadius;

            membraneList.Add(new IntroMembrane(wholeCellPivot, wholeCellRadius, targetToGenerate, membraneOffset));
        }
    }

    public void ResetMembrane()
    {
        membraneList.Clear();
        CreateMembrane();
    }

    public void ApplyMovementAndUpdateRenderVesicleBatches()
    {
        batchCount = 0;
        renderBatches.Clear();
        renderBatches.Add(new List<Matrix4x4>());


        foreach (IntroMembrane m in membraneList)
        {
            m.UpdatePosition();

            if (renderBatches[batchCount].Count >= 999)
            {
                batchCount++;
                renderBatches.Add(new List<Matrix4x4>());
            }
            renderBatches[batchCount].Add(Matrix4x4.TRS(m.position, m.rotation, Vector3.one));
        }
    }

    public void RenderAllBatches()
    {
        foreach (List<Matrix4x4> batch in renderBatches)
        {
            for (int x = 0; x < membrane.subMeshCount; x++)
            {
                Graphics.DrawMeshInstanced(membrane, x, membraneMaterial, batch);
            }
        }
    }
#endif
}
