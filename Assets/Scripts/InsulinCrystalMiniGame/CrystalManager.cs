using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using static UnityEngine.GraphicsBuffer;
using System.Runtime.InteropServices;

/// <summary>
/// This manages the creation of the insulin crystal gameboard and tiling. 
/// </summary>

public class CrystalManager : MonoBehaviour
{
    public static CrystalManager Instance { get; private set; }

    //SETUP
    public float rotationSpeed;
    [Tooltip("1.18125f is default values for NexusHub scale")]
    public float yOffset = 1.18125f; // DEFAULT VALUES 
    [Tooltip("4.934052f is default values for NexusHub scale")]
    public float radius = 4.934052f; // DEFAULT VALUES 
    [SerializeField] private GameObject hexamerPrefab;
    [SerializeField] private GameObject slotPrefab;
    [Tooltip("Size of gameboard. How many layers can the crystal grow.")]
    [SerializeField] private int totalLayers;
    private List<Vector3> currentSetupSlots = new List<Vector3>();
    private List<Vector3> tempSetupSlots = new List<Vector3>();

    //MAIN VARIABLES
    [Tooltip("Parent object for all the slots that get generated")]
    [SerializeField] private Transform slotContainer;
    [Tooltip("Current tiling layer of the crystal from the inside out.")]
    [SerializeField] public int currentLayer = 0;
    [Tooltip("Currently used to spawn hexamer GameObjects ")]
    [SerializeField] private ObjectSpawner objectSpawner;
    private List<GameObject> allSlots = new List<GameObject>();
    private bool setup = false;
    private int boundaryCollisionCount = 0;
    private int totalHexamerCount = 0;
    private int regularHexamerCount = 0;
    private int stableHexaerCount = 0;

    //SPIDER GRAPH
    private Vector3 furthestObj1;
    private Vector3 furthestObj2;
    private float maxDistance = 0f;
    void Awake()
    {
        Instance = this;
    }

    // SETTING UP THE GAME BOARD 
    public void OnEnable()
    {
        //Generate starting hexamerPrefab at center of the map
        currentSetupSlots.Add(this.transform.position);
        GameObject startinghexamerPrefab = Transform.Instantiate(hexamerPrefab, this.transform.position, Quaternion.identity);
     startinghexamerPrefab.transform.Find("SphereCollider").GetComponent<Hexamer_MiniGame>().currentHexamerState = Hexamer_MiniGame.HexamerState.Placed; 
     startinghexamerPrefab.transform.Find("SphereCollider").GetComponent<Hexamer_MiniGame>().SetGrabbable(false);
        startinghexamerPrefab.transform.SetParent(slotContainer);

        //Generate starting slot at center of map 
        allSlots.Add(Transform.Instantiate(slotPrefab, this.transform.position, Quaternion.identity, slotContainer));
        allSlots[allSlots.Count - 1].GetComponent<Slot>().Initialize(0, allSlots.Count, this, false);
        allSlots[allSlots.Count - 1].GetComponent<Slot>().FillOnInitialize(startinghexamerPrefab.transform.Find("SphereCollider").GetComponent<Hexamer_MiniGame>());

        //Create all the potential slots
        SetupSlots();
        SetupNeighbors();
        UnlockLayer();
        GameObject hexamer2 = Transform.Instantiate(hexamerPrefab, allSlots[1].transform.position, Quaternion.identity);
        hexamer2.transform.Find("SphereCollider").GetComponent<Hexamer_MiniGame>().currentHexamerState = Hexamer_MiniGame.HexamerState.Placed;
        hexamer2.transform.Find("SphereCollider").GetComponent<Hexamer_MiniGame>().SetGrabbable(false);
        hexamer2.transform.SetParent(slotContainer);
        allSlots[1].GetComponent<Slot>().FillOnInitialize(hexamer2.transform.Find("SphereCollider").GetComponent<Hexamer_MiniGame>());
        GameObject hexamer3 = Transform.Instantiate(hexamerPrefab, allSlots[2].transform.position, Quaternion.identity);
        hexamer3.transform.Find("SphereCollider").GetComponent<Hexamer_MiniGame>().currentHexamerState = Hexamer_MiniGame.HexamerState.Placed;
        hexamer3.transform.Find("SphereCollider").GetComponent<Hexamer_MiniGame>().SetGrabbable(false);
        hexamer3.transform.SetParent(slotContainer);
        allSlots[2].GetComponent<Slot>().FillOnInitialize(hexamer3.transform.Find("SphereCollider").GetComponent<Hexamer_MiniGame>());

        RecalculateBoard();
        this.transform.Rotate(new Vector3(90, 0, 0));
        objectSpawner.GeneratehexamerPrefab();
        objectSpawner.GeneratehexamerPrefab();
        objectSpawner.GeneratehexamerPrefab();
        objectSpawner.GeneratehexamerPrefab();
        objectSpawner.GeneratehexamerPrefab();
        objectSpawner.GeneratehexamerPrefab();
        objectSpawner.GeneratehexamerPrefab();
        setup = true;
    }

    public void Update(){
        if(setup){
        transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0), Space.World);
        }
    }

    public void SetupSlots()
    {
        for (int layer = 1; layer <= totalLayers; layer++)
        {
            foreach (Vector3 slotPos in currentSetupSlots)
            {
                for (int x = 0; x < 6; x++)
                {
                    float angle = ((Mathf.PI * x) / 3f) + (Mathf.PI / 6f);
                    Vector3 offsetPosition = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                    Vector3 targetPosition = offsetPosition + slotPos;
                    Vector3 targetPosUp = targetPosition + new Vector3(0, yOffset, 0);
                    Vector3 targetPosDown = targetPosition + new Vector3(0, -yOffset, 0);
                    if (x % 2 != 0)
                    {
                        if (CheckSlot(targetPosUp))
                        {
                            tempSetupSlots.Add(targetPosUp);
                            allSlots.Add(Transform.Instantiate(slotPrefab, targetPosUp, Quaternion.identity, slotContainer));
                            allSlots[allSlots.Count - 1].GetComponent<Slot>().Initialize(layer, allSlots.Count, this, true);
                        }
                    }
                    else
                    {
                        if (CheckSlot(targetPosDown))
                        {
                            tempSetupSlots.Add(targetPosDown);
                            allSlots.Add(Transform.Instantiate(slotPrefab, targetPosDown, Quaternion.identity, slotContainer));
                            allSlots[allSlots.Count - 1].GetComponent<Slot>().Initialize(layer, allSlots.Count, this, true);
                        }
                    }
                }
            }
            currentSetupSlots.Clear();
            currentSetupSlots.AddRange(tempSetupSlots);
            tempSetupSlots.Clear();
        }
    }

    public bool CheckSlot(Vector3 currentSlot)
    {
        foreach (GameObject previousSlot in allSlots)
        {
            if (Vector3.Distance(previousSlot.transform.position, currentSlot) < 0.01f)
            {
                return false;
            }
        }
        return true;
    }

    public void SetupNeighbors()
    {
        foreach (GameObject slot in allSlots)
        {
            slot.GetComponent<Slot>().SetupNeighbors();
        }
    }

    // RUNNING THE GAME BOARD 

    public void UnlockLayer()
    {
        if (currentLayer != 0)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Object_Emitters/glp1_binding_oneshot", this.transform.position);
            QuestLogManager.Instance.NextBeat();
        }
        currentLayer++;
        RecalculateBoard();
    }

    //ANYTIME A SLOT GETS FILLED THIS IS CALLED 
    public void RecalculateBoard()
    {
        int remainingSlots = 0;
        totalHexamerCount = 0;
        regularHexamerCount = 0;
        stableHexaerCount = 0;
        foreach (GameObject slot in allSlots)
        {
            slot.GetComponent<Slot>().RecalculateSlot(currentLayer);
            if (slot.GetComponent<Slot>().IsFilled())
            {
                totalHexamerCount++;
                if(slot.GetComponent<Slot>().GetLayer() <= currentLayer)
                {
                    regularHexamerCount ++;
                }
                if(slot.GetComponent<Slot>().currentSlotState == Slot.SlotState.Ideal) 
                {
                    stableHexaerCount++;
                }
            }
            else if (!slot.GetComponent<Slot>().IsFilled() && slot.GetComponent<Slot>().GetLayer() == currentLayer)
            {
                remainingSlots++;
            }
        }
        objectSpawner.GeneratehexamerPrefab();
        QuestLogManager.Instance.UpdateProgressIndex(regularHexamerCount, regularHexamerCount + remainingSlots);
        if (remainingSlots == 0 && currentLayer < totalLayers)
        {
            UnlockLayer();
        }
    }

    public void TurnOffSlots(){
        foreach(GameObject slot in allSlots){
            slot.GetComponent<MeshRenderer>().enabled = false;
            slot.transform.GetChild(0).gameObject.SetActive(false);
            slot.transform.GetChild(1).gameObject.SetActive(false);
        }            
    }

    public float[] GetCrystalData(){
        RecalculateBoard();
        float[] cd = new float[6];
        cd[0] = totalHexamerCount;
        cd[1] = regularHexamerCount;
        cd[2] = currentLayer;
        cd[3] = CalculateDiameter();
        cd[4] = stableHexaerCount;
        cd[5] = boundaryCollisionCount;
        return cd;
    }

    private float CalculateDiameter()
    {
        List<Vector3> positions = new List<Vector3>();

        foreach (GameObject slot in allSlots)
        {
            slot.GetComponent<Slot>().RecalculateSlot(currentLayer);
            if (slot.GetComponent<Slot>().IsFilled())
            {
                positions.Add(slot.transform.position);
            }
        }

        // Initialize the furthest objects
        furthestObj1 = positions[0];
        furthestObj2 = positions[1];

        // Calculate distances between all pairs of game objects
        for (int i = 0; i < positions.Count; i++)
        {
            for (int j = i + 1; j < positions.Count; j++)
            {
                float distance = Vector3.Distance(positions[i], positions[j]);

                // Update maximum distance and associated objects
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    furthestObj1 = positions[i];
                    furthestObj2 = positions[j];
                }
            }
        }

        // Print results
        return maxDistance;
    }

    public void AddToBoundaryCount()
    {
        boundaryCollisionCount++;
    }
}
