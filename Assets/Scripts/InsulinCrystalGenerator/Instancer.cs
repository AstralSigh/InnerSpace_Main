using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instancer : MonoBehaviour
{
    public int AnimatedHexamerCount;
    public int StaticHexamerCount;
    public int LowPolyHexamerCount;
    public Mesh LowPolyHexamerMesh;
    public Mesh HighPolyHexamerMesh;
    public Material HighPolyMaterial;
    public Material LowPolyMaterial;
    private List<List<Matrix4x4>> LowPolyBatches = new List<List<Matrix4x4>>();
    private List<Matrix4x4> HighPolyBatch = new List<Matrix4x4>();
    public PossibleHexamerLocations masterList;
    public Transform player;
    public List<DimerMaster> _hexamerList = new List<DimerMaster>();
    public GameObject[] _hexamerPrefabs;
    public bool createNewAnimatedHexamers;
    public bool runHexamerReleaseSimulation;
    public float _durationBetweenDisolves = 1f;
    private void RenderBatches()
    {
        foreach (var Batch in LowPolyBatches)
        {
            for(int i = 0; i < LowPolyHexamerMesh.subMeshCount; i++)
            {
                Graphics.DrawMeshInstanced(LowPolyHexamerMesh, i, LowPolyMaterial, Batch);
            }
        }

        for (int i = 0; i < HighPolyHexamerMesh.subMeshCount; i++)
        {
            Graphics.DrawMeshInstanced(HighPolyHexamerMesh, i, HighPolyMaterial, HighPolyBatch);
        }
    }

    private void Update()
    {
        RenderBatches();
        if (runHexamerReleaseSimulation)
        {
            StartCoroutine(RunHexamerReleaseSimulation());
            runHexamerReleaseSimulation = false;
        }
    }

    private void Start()
    {
        int AddedMatricies = 0;

        LowPolyBatches.Add(item: new List<Matrix4x4>());

        if (createNewAnimatedHexamers)
        {
            for (int i = 0; i < AnimatedHexamerCount; i++)
            {
                List<GameObject> tempList = new List<GameObject>();
                for (int y = 0; y < 2; y++)
                {
                    tempList.Add(Instantiate(_hexamerPrefabs[y], masterList.sortedList[i], Quaternion.identity));
                    tempList[y].transform.parent = this.transform;
                    tempList[y].name = i + "_" + tempList[y].name;
                }
                GameObject spawnedHeaxmer = Instantiate(_hexamerPrefabs[2], masterList.sortedList[i], Quaternion.identity);
                spawnedHeaxmer.transform.parent = this.transform;
                spawnedHeaxmer.GetComponent<DimerMaster>()._index = i;
                _hexamerList.Add(spawnedHeaxmer.GetComponent<DimerMaster>());
                _hexamerList[i].name = i + "_" + _hexamerList[i].name;
                _hexamerList[i]._dimerChild01 = tempList[0];
                _hexamerList[i]._dimerchild02 = tempList[1];
            }
        }

        for(int i = AnimatedHexamerCount; i < StaticHexamerCount + AnimatedHexamerCount; i++)
        {
            HighPolyBatch.Add(item: Matrix4x4.TRS(pos: masterList.sortedList[i], Quaternion.identity, s: Vector3.one));
        }

        for(int i = StaticHexamerCount + AnimatedHexamerCount; i < masterList.sortedList.Count; i++)
        {   
            if(Vector3.Distance(masterList.sortedList[i], player.position) < 1000) //normally 200
            {
                if (AddedMatricies < 1000)
                {
                    LowPolyBatches[LowPolyBatches.Count - 1].Add(item: Matrix4x4.TRS(pos: masterList.sortedList[i], Quaternion.identity, s: Vector3.one));
                    AddedMatricies += 1;
                    LowPolyHexamerCount++;
                }
                else
                {
                    LowPolyBatches.Add(item: new List<Matrix4x4>());
                    AddedMatricies = 0;
                }
            }
        }
    }

    IEnumerator RunHexamerReleaseSimulation()
    {
        for (int x = 0; x < _hexamerList.Count; x++)
        {
            _hexamerList[x]._currentbindingState = DimerMaster.bindingState.crystalToHex;
            float disolveDuration = (0.1f + (x / _hexamerList.Count)) * _durationBetweenDisolves;
            yield return new WaitForSeconds(disolveDuration);
        }
    }
}
