using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This controls slots that are generated and managed by the CrystalManager 
/// </summary>

[System.Serializable]
public class Slot : MonoBehaviour
{
    //SETTINGS 
    [Tooltip("Number of seconds between each seperation check")] 
    [SerializeField] private float checkFrequency = 20;
    [Tooltip("Alpha value of slots")]
    [SerializeField] private float slotAlphaIntesity = 0.3f;
    [Tooltip("Alpha value of highlighted slots")]
    [SerializeField] private float highlightAlphaIntensity = 0.6f;
 
    //STATE MACHINE 
    public enum SlotState { Unavailable, Available, Weak, Ideal}
    public SlotState currentSlotState = SlotState.Unavailable;

    //PRIVATE VARIABLES
    private int index; 
    private int layer;
    private bool filled = false;
    private Hexamer_MiniGame attachedHexamer;
    private bool isOriginalHexamer = false;
    private CrystalManager parent;
    private List<Slot> neighbors = new List<Slot>();
    private float seperationProbability = 0;
    private float time;

    //SETTING UP 
    public void Initialize(int layer, int index, CrystalManager parent, bool meshState)
    {
        this.layer = layer;
        this.index = index;
        this.parent = parent;
        SetMeshRenderer(meshState);
    }

    public void FillOnInitialize(Hexamer_MiniGame attachedHexamer)
    {
        filled = true;
        this.attachedHexamer = attachedHexamer;
        isOriginalHexamer = true;
        currentSlotState = SlotState.Ideal;
        ToggleColor(true);
    }

    public void SetupNeighbors()
    {
        for (int x = 0; x < 6; x++)
        {
            float angle = ((Mathf.PI * x) / 3f) + (Mathf.PI / 6f);
            Vector3 offsetPosition = new Vector3(Mathf.Cos(angle) * parent.radius, 0, Mathf.Sin(angle) * parent.radius);
            Vector3 targetPosition = offsetPosition + transform.position;
            Vector3 targetPosUp = targetPosition + new Vector3(0, parent.yOffset, 0);
            Vector3 targetPosDown = targetPosition + new Vector3(0, -parent.yOffset, 0);
            
                Collider[] items = Physics.OverlapSphere(targetPosUp, 0.01f);

                foreach (Collider item in items)
                {
                    if (item.transform.gameObject.CompareTag("Slot"))
                    {
                  neighbors.Add(item.transform.gameObject.GetComponent<Slot>());
                        break;
                    }
                }
            
                Collider[] items2 = Physics.OverlapSphere(targetPosDown, 0.01f);

                foreach (Collider item in items2)
                {
                    if (item.transform.gameObject.CompareTag("Slot"))
                    {
                        neighbors.Add(item.transform.gameObject.GetComponent<Slot>());
                        break;
                    }
                }
                
        }
        Collider[] items3 = Physics.OverlapSphere(transform.position + new Vector3(0, parent.yOffset * 3, 0), 0.01f);
        foreach (Collider item in items3)
        {
            if (item.transform.gameObject.CompareTag("Slot"))
            {
                neighbors.Add(item.transform.gameObject.GetComponent<Slot>());
                break;
            }
        }
        Collider[] items4 = Physics.OverlapSphere(transform.position + new Vector3(0, -parent.yOffset * 3, 0), 0.01f);
        foreach (Collider item in items4)
        {
            if (item.transform.gameObject.CompareTag("Slot"))
            {
                neighbors.Add(item.transform.gameObject.GetComponent<Slot>());
                break;
            }
        }
    }

    public void ToggleSelection(bool state)
    {
        if(currentSlotState == SlotState.Available){
            transform.GetChild(1).GetComponent<MeshRenderer>().enabled = state;

            if(layer == CrystalManager.Instance.currentLayer){ //TO DO FIX THIS LATER
                 transform.GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(106/255f, 183/255f,41/255f, 255/255f);
            }
            else{
                transform.GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(255/255f, 167/255f,167/255f, 255/255f);
            }
        }
    }

    public void ToggleSuggested(bool state)
    {
        transform.GetChild(0).GetComponent<MeshRenderer>().enabled = state;
    }

    public void ToggleColor(bool state){
        if(state){
            transform.GetChild(0).GetComponent<MeshRenderer>().material.color = new Color(106/255f, 183/255f,41/255f, 255/255f);
        }
        else{
            transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hexamer") && other.GetComponent<Hexamer_MiniGame>().currentHexamerState != Hexamer_MiniGame.HexamerState.Placed && currentSlotState != SlotState.Unavailable && !filled)
        {
            ToggleSelection(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Hexamer") && other.GetComponent<Hexamer_MiniGame>().currentHexamerState != Hexamer_MiniGame.HexamerState.Placed && currentSlotState != SlotState.Unavailable)
        {
            ToggleSelection(false);
        }
    }

    public void RecalculateSlot(int layer)
    {
        if (isOriginalHexamer)
        {
            if (CheckNeighborsFill() == 1)
            {
                attachedHexamer.GetComponent<Hexamer_MiniGame>().SetColor(Hexamer_MiniGame.HexamerColor.Desaturated);
                //attachedHexamer.GetComponent<Hexamer_MiniGame>().SetColor(Hexamer_MiniGame.HexamerColor.BW);
                seperationProbability = 0.20f;
            }
            else if (CheckNeighborsFill() == 2)
            {
                attachedHexamer.GetComponent<Hexamer_MiniGame>().SetColor(Hexamer_MiniGame.HexamerColor.Desaturated);
                seperationProbability = 0.10f;
            }
            else
            {
                attachedHexamer.GetComponent<Hexamer_MiniGame>().SetColor(Hexamer_MiniGame.HexamerColor.Normal);
            }
            return;
        }

        if (!filled)
        {
            if (CheckNeighborsFill() == 0)
            {
                transform.GetComponent<MeshRenderer>().enabled = false;
                currentSlotState = SlotState.Unavailable;
            }
            else
            {
                transform.GetComponent<MeshRenderer>().enabled = true;
                currentSlotState = SlotState.Available;
            }
        }
        else
        {
            transform.GetComponent<MeshRenderer>().enabled = false;
            if (CheckNeighborsFill() == 1)
            {
                //attachedHexamer.GetComponent<Hexamer_MiniGame>().SetColor(Hexamer_MiniGame.HexamerColor.BW);
                attachedHexamer.GetComponent<Hexamer_MiniGame>().SetColor(Hexamer_MiniGame.HexamerColor.Desaturated);
                seperationProbability = 0.20f;
                currentSlotState = SlotState.Weak;
            }
            else if (CheckNeighborsFill() == 2)
            {
                attachedHexamer.GetComponent<Hexamer_MiniGame>().SetColor(Hexamer_MiniGame.HexamerColor.Desaturated);
                seperationProbability = 0.10f;
                currentSlotState = SlotState.Weak;
            }
            else
            {
                attachedHexamer.GetComponent<Hexamer_MiniGame>().SetColor(Hexamer_MiniGame.HexamerColor.Normal);
                currentSlotState = SlotState.Ideal;
            }
        }

        if (this.layer == layer)
        {
            ToggleSuggested(true);
            if(filled)
            {
                ToggleColor(true);
            }
            else
            {
                ToggleColor(false);
            }
            
        }
        else
        {
            ToggleSuggested(false);
        }
    }

    //METHODS 
    public void SetMeshRenderer(bool state)
    {
        transform.GetComponent<MeshRenderer>().enabled = state;
    }

    public bool IsFilled()
    {
        return filled;
    }

    //FROM hexamerPrefab
    public void AttachHexamer(Hexamer_MiniGame hexamer)
    {
        SetMeshRenderer(false);
        ToggleSelection(false);
        ToggleSuggested(false);
        filled = true;
        attachedHexamer = hexamer;
        if(CrystalManager.Instance.currentLayer == layer){
            FMODUnity.RuntimeManager.PlayOneShot("event:/Object_Emitters/hexamer_binding", this.transform.parent.position);
        }
        else{
            FMODUnity.RuntimeManager.PlayOneShot("event:/Object_Emitters/hexamer_binding_imperfect", this.transform.parent.position);
        }

        parent.RecalculateBoard();
    }

    public void DetatchHexamer()
    {
        filled = false;
        attachedHexamer.DetatchHexamer();
        attachedHexamer = null;
        parent.RecalculateBoard();

    }

    public int GetLayer()
    {
        return layer;
    }

    public int CheckNeighborsFill()
    {
        int result = 0;
        foreach(Slot neighbor in neighbors)
        {
            if (neighbor.IsFilled())
            {
                result ++;
            }
        }
        return result;
    }
}
