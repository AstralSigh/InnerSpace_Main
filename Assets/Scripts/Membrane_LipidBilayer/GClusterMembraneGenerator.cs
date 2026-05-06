using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GClusterMembraneGenerator : MonoBehaviour
{
    class Membrane{
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 offsetPosition;
        public Membrane(Transform parentPosition, Vector3 offsetPosition)
        {
            this.offsetPosition = offsetPosition;
            rotation = parentPosition.rotation;
            this.position = parentPosition.position + parentPosition.right * offsetPosition.x + parentPosition.forward * offsetPosition.z + parentPosition.up * offsetPosition.y;

        }

        public void UpdatePosition(Transform parentPosition)
        {
            this.position = parentPosition.position + parentPosition.right * offsetPosition.x + parentPosition.forward * offsetPosition.z + parentPosition.up * offsetPosition.y;
            rotation = parentPosition.rotation;
        }
        public bool CheckCulling(Vector3 player, float cullingDistance){
            if(Vector3.Distance(position, player) < cullingDistance){
                return true;
            }
            return false;
        }
    }
    [SerializeField] private bool resetMembrane;
    [SerializeField] private int membraneCount;
    [SerializeField] private int spawnRadius;
    [SerializeField] private int membraneCountQuest;
    [SerializeField] private int spawnRadiusQuest;
    [SerializeField] float cullingDistance;
    [SerializeField] private Mesh membraneMesh;
    [SerializeField] private Material membraneOpaque;
    [SerializeField] private Material membraneTransparent;
    [Tooltip("Each frame membrane will reposition based on the parents posiiton")]
    [SerializeField] private bool followParentObject;
    [Tooltip("Numerical value of randomness added to x and z positions")]
    [SerializeField] private float randomness; 
    private int opaqueBatchCount;
    private int transparentBatchCount;
    private List<Membrane> membraneList; 
    private List<List<Matrix4x4>> opaqueRenderBatches; 
    private List<List<Matrix4x4>> transparentRenderBatches; 
    private bool makeAllTransparent = false;

    void Start(){
        CreateMembrane();
        #if UNITY_ANDROID
            CreateQuestRenderBatches();
        #endif
    }

    void Update(){
        if(resetMembrane){
            CreateMembrane();
            resetMembrane = false;
        }
        if (followParentObject)
        {
            UpdatePosition();
        }
        #if UNITY_STANDALONE_WIN
            UpdateRenderBatches();
        #endif

        RenderAllBatches();
    }

    public void CreateMembrane(){
        if(membraneList == null)
        {
            membraneList = new List<Membrane>();
        }
        else
        {
            membraneList.Clear();
        }

        #if UNITY_ANDROID
            membraneCount = membraneCountQuest;
            spawnRadius = spawnRadiusQuest;
        #endif

        for (int i = 0; i < membraneCount; i++){
            float dst = Mathf.Pow(i / (membraneCount - 1f), 0.5f);
            float angle = 2 * Mathf.PI * 1.6180f * i;
            float x = dst * Mathf.Cos(angle) * spawnRadius + randomness * (Random.Range(-1,1));
            float z = dst * Mathf.Sin(angle) * spawnRadius + randomness * (Random.Range(-1,1));

            membraneList.Add(new Membrane(this.transform, new Vector3(x, 0, z)));
        }
    }

    //TOGGLED ON OR OFF BASED ON "followParentObject"
    public void UpdatePosition()
    {
        foreach(Membrane m in membraneList)
        {
            m.UpdatePosition(this.transform);
        }
    }

    //MAKE NEW METHOD THAT ONLY DOES 2000 MEMBRANE, TRANSPARENT FOR QUEST. 

    public void UpdateRenderBatches()
    {
        if(opaqueRenderBatches == null){
            opaqueRenderBatches = new List<List<Matrix4x4>>();
        }
        opaqueBatchCount = 0;
        opaqueRenderBatches.Clear();
        opaqueRenderBatches.Add(new List<Matrix4x4>());
        
        if(transparentRenderBatches == null){
            transparentRenderBatches = new List<List<Matrix4x4>>();
        }
        transparentBatchCount = 0;
        transparentRenderBatches.Clear();
        transparentRenderBatches.Add(new List<Matrix4x4>());

        foreach(Membrane m in membraneList)
        {
            
            if (opaqueRenderBatches[opaqueBatchCount].Count >= 999)
            {
                opaqueBatchCount++;
                opaqueRenderBatches.Add(new List<Matrix4x4>()); 
            }
            if (transparentRenderBatches[transparentBatchCount].Count >= 999)
            {
                transparentBatchCount++;
                transparentRenderBatches.Add(new List<Matrix4x4>()); 
            }

            //IF GOOD TO RENDER  
            if(!m.CheckCulling(Camera.main.transform.position, cullingDistance)){
                opaqueRenderBatches[opaqueBatchCount].Add(Matrix4x4.TRS(m.position, m.rotation, Vector3.one));
            }
            else{
                transparentRenderBatches[transparentBatchCount].Add(Matrix4x4.TRS(m.position, Quaternion.identity, Vector3.one));
            }
        }
    }

    public void CreateQuestRenderBatches()
    {
        if (transparentRenderBatches == null)
        {
            transparentRenderBatches = new List<List<Matrix4x4>>();
        }
        transparentBatchCount = 0;
        transparentRenderBatches.Clear();
        transparentRenderBatches.Add(new List<Matrix4x4>());

        foreach (Membrane m in membraneList)
        {
            if (transparentRenderBatches[transparentBatchCount].Count >= 999)
            {
                transparentBatchCount++;
                transparentRenderBatches.Add(new List<Matrix4x4>());
            }
            transparentRenderBatches[transparentBatchCount].Add(Matrix4x4.TRS(m.position, Quaternion.identity, Vector3.one));
        }
    }

    public void ToggleMembraneTransparency(){
        makeAllTransparent = !makeAllTransparent;
    }

    public void RenderAllBatches()
    {
        foreach (List<Matrix4x4> batch in transparentRenderBatches)
        {
            for (int x = 0; x < membraneMesh.subMeshCount; x++)
            {
                Graphics.DrawMeshInstanced(membraneMesh, x, membraneTransparent, batch);
            }
        }

        #if UNITY_ANDROID
            //QUEST WON'T HAVE OPAQUE MEMBRANE
            return;
        #endif


        foreach (List<Matrix4x4> batch in opaqueRenderBatches)
        {
            for(int x = 0; x < membraneMesh.subMeshCount; x++)
            {
                if(makeAllTransparent)
                {
                Graphics.DrawMeshInstanced(membraneMesh, x, membraneTransparent, batch);
                }
                else
                {
                Graphics.DrawMeshInstanced(membraneMesh, x, membraneOpaque, batch);
                }
            }
        }
        
    }

}
