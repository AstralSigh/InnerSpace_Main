using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsulinCrystalGenerator_FORNEXUS09 : MonoBehaviour
{
    public PossibleHexamerLocations hexamerSavedLocation;
    public Mesh lowpolyMesh;
    public Material hexamerMaterial;
    public GameObject hexamerPrefab;
    public Transform sortTarget;
    private List<List<Matrix4x4>> renderBatches = new List<List<Matrix4x4>>();
    private int batchCount;
    public List<Vector3> hexamerPositions = new List<Vector3>();
    public List<Vector3> hexamerToExport = new List<Vector3>();
    public List<Vector3> hexamersToAdd = new List<Vector3>();

    public float radius; 
    public float yOffset; 
    public bool addLayer = false;
    public bool instantiateHexamers = false;
    public bool setByLayer = false;
    public bool clear = false;
    public bool sortList = false;
    public bool saveList = false;
    public bool renderSavedHexamers = false;
    public int inputLayer;
    public int hexamerCount; 
    public int layerCount;
    public List<int> indexPerLayer;


    // Start is called before the first frame update
    void Start()
    {
        hexamerPositions.Add(transform.position);
        indexPerLayer.Add(0);
        hexamerCount =1;

        hexamerSavedLocation.exportToBlender();
    }

    void Update(){
        if(addLayer){
            AssignNewPositions();
            AddToHexamerList();
            UpdateRenderBatches();
            layerCount++;
            addLayer = false;
            indexPerLayer.Add(hexamerCount -1);
        }

        if(setByLayer){
            Clear();
            for(int i = 0; i < inputLayer; i++){
                AssignNewPositions();
                AddToHexamerList();
                layerCount++;
            }
            UpdateRenderBatches();
            setByLayer = false;
        }

        if(instantiateHexamers){
            foreach(Vector3 h in hexamerPositions){
                Instantiate(hexamerPrefab, h, Quaternion.identity);
            }
            instantiateHexamers = false;
        }

        if(sortList){
            SortList();
            sortList = false;
        }

        if(clear){
            Clear();
            clear = false;
        }

        if (saveList)
        {
            hexamerSavedLocation.possibleHexamerLocations.Clear();
            for (int x = 6012; x < hexamerPositions.Count; x++)
            {
                hexamerSavedLocation.possibleHexamerLocations.Add(hexamerPositions[x]);
            }
            hexamerSavedLocation.possibleHexamerLocations.Sort((a, b) => Vector3.Distance(a, sortTarget.position).CompareTo(Vector3.Distance(b, sortTarget.position)));
            saveList = false;
        }

        if (renderSavedHexamers)
        {
            for (int x = 90; x < hexamerSavedLocation.possibleHexamerLocations.Count; x++)
            {
                hexamersToAdd.Add(hexamerSavedLocation.possibleHexamerLocations[x]);
            }
            AddToHexamerList();
            UpdateRenderBatches();
            renderSavedHexamers = false;
        }

        RenderAllBatches();
    }

    void Clear(){
        hexamerPositions.Clear();
        hexamerPositions.Add(transform.position);
        hexamerCount =1;
        indexPerLayer.Clear();
        indexPerLayer.Add(0);
        layerCount = 1;
        UpdateRenderBatches();
    }

    void RenderAllBatches(){
        foreach(List<Matrix4x4> batch in renderBatches){
            for(int i = 0; i < lowpolyMesh.subMeshCount; i++){
                Graphics.DrawMeshInstanced(lowpolyMesh, i, hexamerMaterial, batch);
            }
        }
    }

    void UpdateRenderBatches()
    {
        batchCount = 0;
        renderBatches.Clear();
        renderBatches.Add(new List<Matrix4x4>());
        foreach(Vector3 h in hexamerPositions)
        {
            if(renderBatches[batchCount].Count >= 999)
            {
                batchCount++;
                renderBatches.Add(new List<Matrix4x4>());
            }
            renderBatches[batchCount].Add(Matrix4x4.TRS(h, this.transform.rotation, Vector3.one));
            
        }
    }


    void SortList(){
        hexamerPositions.Sort((a, b) => Vector3.Distance(a, sortTarget.position).CompareTo(Vector3.Distance(b, sortTarget.position)));
    }

    void AssignNewPositions(){
        for(int h = 0; h < hexamerPositions.Count; h++){
            for(int x = 0; x < 6; x++)
            {
                float angle = 2* Mathf.PI * x/6f + 2*Mathf.PI* 1/12f;
                Vector3 targetPosition = hexamerPositions[h] + 
                    transform.right * Mathf.Cos(angle) * radius + 
                    transform.forward * Mathf.Sin(angle) * radius;
    
                if(x%2 != 0 ) //UP
                {
                    if(!InList(targetPosition + transform.up * yOffset))
                    {
                    hexamersToAdd.Add(targetPosition + transform.up * yOffset);
                    hexamerCount++;
                    }
                }
                else // DOWN
                {
                    if(!InList(targetPosition - transform.up * yOffset))
                    {
                    hexamersToAdd.Add(targetPosition - transform.up * yOffset);
                    hexamerCount++;
                    }
                }
            }
        }
    }

    void AddToHexamerList(){
        for(int x = 0; x < hexamersToAdd.Count; x++){
            hexamerPositions.Add(hexamersToAdd[x]);
            }
            hexamersToAdd.Clear();
    }

    bool InList(Vector3 input){
        for(int i = 0; i < hexamerPositions.Count; i++){
            if(Vector3.Distance(input, hexamerPositions[i]) < yOffset){
                return true;
            }
        }
        for(int i = 0; i < hexamersToAdd.Count; i++){
            if(Vector3.Distance(input, hexamersToAdd[i]) < yOffset){
                return true;
            }
        }

        return false;
    }

}

