using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InsulinCrystalGenerator : MonoBehaviour
{
    [System.Serializable]
    public class Hexamer{
        InsulinCrystalGenerator parent; //The parent script that is being used. 
        public Vector3 position; //Hexamers position 
        Vector3[] slotPositions = new Vector3[6]; //Positions of the 6 slots surrounding this hexamer (that other hexamers can fill)
        bool[] availableSlots = new bool[]{true,true,true,true,true,true}; //State of whether or not the slots are filled. 
        bool bindingOrder; //Not implemented yet.

        public Hexamer(Vector3 position, InsulinCrystalGenerator parent){
            this.position = position;
            this.parent = parent;
            SetupSlots();
            bindingOrder = parent.GetBindingOrder();
        }

        //Fills the Vector3[] slotPositions with all the possible binding locations. 
        public void SetupSlots(){
            for(int x = 0; x < 6; x++){
                float angle = ((Mathf.PI * x)/3f) + (Mathf.PI /6f);
                Vector3 offsetPosition = new Vector3(Mathf.Cos(angle) * parent.GetRadius(), 0, Mathf.Sin(angle) * parent.GetRadius());
                Vector3 targetPosition = offsetPosition + position;
                Vector3 targetPosUp = targetPosition + new Vector3(0,parent.GetYOffset(),0);
                Vector3 targetPosDown = targetPosition + new Vector3(0,-parent.GetYOffset(),0);
                if(x%2 != 0 ){
                    slotPositions[x] = targetPosUp;
                }
                else{
                    slotPositions[x] = targetPosDown;
                }
            }            
        }

        public void CheckSlotAvailability(List<Hexamer> incomingCoordinate){
            for(int x = 0; x < availableSlots.Length; x++){
                if(availableSlots[x]){
                        foreach(Hexamer p in incomingCoordinate){
                            if(Vector3.Distance(p.position, slotPositions[x]) < 0.1f){
                            availableSlots[x] = false;
                        }
                    }
                }
            }
        }

        public void AddNewHexamer(){
            for(int x = 0; x < availableSlots.Length; x++){
                if(availableSlots[x]){
                    if(parent.AddNewHexamer(slotPositions[x])){
                        availableSlots[x] = false;
                    }
                    else{
                        break;
                    }
                }
            }
        }
    }

    [SerializeField] private int hexamerCount = 1;
    [SerializeField] private float yOffset = 0.2394075462f * 4.934055f;
    [SerializeField] private float radius = 1 * 4.934055f;
    [SerializeField] private Mesh hexamerMesh;
    [SerializeField] private Material hexamerMaterial;
    [SerializeField] private bool randomizeBindingOrder;
    [SerializeField] private GameObject[] monomerPrefabs;
    [SerializeField] private GameObject[] dimerPrefabs; 
    [SerializeField] bool animateHexamers = false;
    private enum GeneratorState { RenderingPreview, AnimatingWithMonomers, AnimatingWithDimers};
    private GeneratorState currentGeneratorState = GeneratorState.RenderingPreview;

    private enum SimulationType { AnimateWithMonomers, AnimateWithDimers};
    [SerializeField] private SimulationType currentSimulationType;
    private List<Hexamer> localCrystal = new List<Hexamer>();
    [SerializeField] private List<DimerMaster> dimerMasters = new List<DimerMaster>();
    [SerializeField] private List<MonomerMaster> monomerMasters = new List<MonomerMaster>();

    private List<List<Matrix4x4>> renderBatches = new List<List<Matrix4x4>>(); //Keeps track of coordinates to render for GPU instancing
    private int batchCount = 0; //Used for GPU instancing. Count batches of 1000 Matrix4x4 
    [SerializeField] private float simulationTime = -1;
    [SerializeField] private int releaseIndex = -1; 
    [SerializeField] float dissolveFrequency = 1;
    [SerializeField] float dissolveRandomness = 0.1f;

    public void Awake(){
        localCrystal.Add(new Hexamer(new Vector3(0,0,0), this));
    }

    public void Update(){
        switch(currentGeneratorState){
            case GeneratorState.RenderingPreview:

                //Add new hexamers when hexamerCount input is increased 
                if(localCrystal.Count < hexamerCount){
                    for(int x = 0; x < localCrystal.Count; x++){
                        localCrystal[x].CheckSlotAvailability(localCrystal);
                        localCrystal[x].AddNewHexamer();
                    }
                }
                UpdateRenderBatches();
                RenderAllBatches();

                //CHECK IF YOU ARE ANIMATING 
                if(animateHexamers){
                    //Reset values for new animation. 
                    simulationTime = 1f/ dissolveFrequency;
                    releaseIndex = -1;
                        //User inputs whether this should animate with monomers or dimers. 
                        switch(currentSimulationType){
                            case SimulationType.AnimateWithMonomers:
                                InstantiateHexamersWithMonomers();
                                localCrystal.Clear();
                                currentGeneratorState = GeneratorState.AnimatingWithMonomers;
                                //transform.GetComponent<RecordTransformHierarchy>().StartRecording();
                                break;
                            case SimulationType.AnimateWithDimers:
                                InstantiateHexamersWithDimers();
                                localCrystal.Clear();
                                currentGeneratorState = GeneratorState.AnimatingWithDimers;
                                //transform.GetComponent<RecordTransformHierarchy>().StartRecording();
                                break;
                        }
                    break;
                }

                //Reset to 1 when hexamerCount is decreased  
                if(localCrystal.Count > hexamerCount){
                    localCrystal.Clear();
                    localCrystal.Add(new Hexamer(new Vector3(0,0,0), this));
                }
                break;

            case GeneratorState.AnimatingWithMonomers:
                //CHECK IF THERE ARE HEXAMERS LEFT TO ANIMATE (RELEASE INDEX STARTS AS -1)
                if(releaseIndex < monomerMasters.Count -1){
                    if(simulationTime < 0){
                    releaseIndex++;
                    monomerMasters[monomerMasters.Count -1 - releaseIndex]._currentbindingState = MonomerMaster.bindingState.crystalToHex;
                    //RESETS THE TIMER WITH INPUTED FREQUENCY + RANDOMNESS
                    simulationTime = (1f/ dissolveFrequency) * Random.Range(1 - dissolveRandomness, 1 + dissolveRandomness);
                    }
                    else{
                        simulationTime -= Time.deltaTime;
                    }
                }
                break;

            case GeneratorState.AnimatingWithDimers:
                //CHECK IF THERE ARE HEXAMERS LEFT TO ANIMATE
                if(releaseIndex < dimerMasters.Count -1){
                    if(simulationTime < 0){
                    releaseIndex++;
                    dimerMasters[dimerMasters.Count -1 - releaseIndex]._currentbindingState = DimerMaster.bindingState.crystalToHex;
                    //RESETS THE TIMER WITH INPUTED FREQUENCY + RANDOMNESS
                    simulationTime = (1f/ dissolveFrequency) * Random.Range(1 - dissolveRandomness, 1 + dissolveRandomness);
                    }
                    else{
                        simulationTime -= Time.deltaTime;
                    }
                }
                break;              
        } 
    }

    public bool AddNewHexamer(Vector3 position){
        if(localCrystal.Count < hexamerCount){
            localCrystal.Add(new Hexamer(position, this));
            return true;
        }
        else{
            return false;
        }
    }         

    public float GetYOffset(){
        return yOffset;
    }
    public float GetRadius(){
        return radius;
    }

    public bool GetBindingOrder(){
        return randomizeBindingOrder;
    }

    public void UpdateRenderBatches()
    {
        batchCount = 0;
        renderBatches.Clear();
        renderBatches.Add(new List<Matrix4x4>());
        foreach(Hexamer m in localCrystal)
        {
            if (renderBatches[batchCount].Count >= 999){
                batchCount++;
                renderBatches.Add(new List<Matrix4x4>()); 
            }
            renderBatches[batchCount].Add(Matrix4x4.TRS(m.position, Quaternion.identity, Vector3.one));
        }
    }

    public void RenderAllBatches()
    {
        foreach(List<Matrix4x4> batch in renderBatches){
            for(int x = 0; x < hexamerMesh.subMeshCount; x++){
                Graphics.DrawMeshInstanced(hexamerMesh, x, hexamerMaterial, batch);
            }
        }
    }

    //INSANTIATE MONOMERS FOR NEXUS 08 
    //EACH HEXAMER IS MADE OF 6 MONOMER GAME OBJECTS

        //monomerFollowers
    //00_invertedMonomer01
    //01_dimerChild01
    //02_invertedMonomer02
    //03_dimerChild02
    //04_invertedMonomer03

        //monomerMaster
    //05_hexamer

    public void InstantiateHexamersWithMonomers(){
        for (int x = 0; x < localCrystal.Count; x++){
            List<GameObject> monomerFollower = new List<GameObject>();

            //Instantiate the monomerFollowers
            for (int y = 0; y < 5; y++){
                monomerFollower.Add(Instantiate(monomerPrefabs[y], localCrystal[x].position, Quaternion.identity));
                monomerFollower[y].transform.parent = this.transform;
                monomerFollower[y].name = x + "_" + monomerFollower[y].name;
            }

            //Instantiate the monomerMaster
            GameObject monomerMaster = Instantiate(monomerPrefabs[5], localCrystal[x].position, Quaternion.identity);
            monomerMaster.transform.parent = this.transform;
            monomerMaster.GetComponent<MonomerMaster>()._index = x;
            monomerMasters.Add(monomerMaster.GetComponent<MonomerMaster>());

            //Set references for monomerMaster
            monomerMaster.GetComponent<MonomerMaster>().name = x + "_" + monomerMaster.GetComponent<MonomerMaster>().name;
            monomerMaster.GetComponent<MonomerMaster>()._invertedMonomer01 = monomerFollower[0];
            monomerMaster.GetComponent<MonomerMaster>()._dimerChild01 = monomerFollower[1];
            monomerMaster.GetComponent<MonomerMaster>()._invertedMonomer02 = monomerFollower[2];
            monomerMaster.GetComponent<MonomerMaster>()._dimerchild02 = monomerFollower[3];
            monomerMaster.GetComponent<MonomerMaster>()._invertedMonomer03 = monomerFollower[4];
        }
    }

    //DIMER FOR NEXUS 09
    public void InstantiateHexamersWithDimers(){
        for (int i = 0; i < localCrystal.Count; i++){
            List<GameObject> dimerFollower = new List<GameObject>();
            for (int y = 0; y < 2; y++){
                dimerFollower.Add(Instantiate(dimerPrefabs[y], localCrystal[i].position, Quaternion.identity));
                dimerFollower[y].transform.parent = this.transform;
                dimerFollower[y].name = i + "_" + dimerFollower[y].name;
            }
            GameObject dimerMaster = Instantiate(dimerPrefabs[2], localCrystal[i].position, Quaternion.identity);
            dimerMaster.transform.parent = this.transform;
            dimerMaster.GetComponent<DimerMaster>()._index = i;

            dimerMasters.Add(dimerMaster.GetComponent<DimerMaster>());
            dimerMasters[i].name = i + "_" + dimerMasters[i].name;
            dimerMasters[i]._dimerChild01 = dimerFollower[0];
            dimerMasters[i]._dimerchild02 = dimerFollower[1];
        }
    }           
        
}
