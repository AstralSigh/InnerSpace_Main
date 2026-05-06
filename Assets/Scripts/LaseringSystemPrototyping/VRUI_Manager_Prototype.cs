using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class VRUI_Manager_Prototype : MonoBehaviour
{
    public float maxDistance = 100f;
    public LayerMask layerMask;
    public Transform leftPointer;
    public Transform rightPointer;
    public bool itemSelected = false;
    public bool nexusTourStatus = false;

    [SerializeField] private Transform dominantHand;

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

 
        /*
        dominantHand = DoRaycast();

        if(dominantHand == rightPointer && itemSelected == false)
        {
            PointerManager.Instance.EnableRight();
            itemSelected= true;
        }
        else if(dominantHand == leftPointer && itemSelected == false)
        {
            PointerManager.Instance.EnableLeft();
            itemSelected= true;
        }
        else if(dominantHand == null && itemSelected == true)
        {
            PointerManager.Instance.HideBothPointers();
            itemSelected= false;   
        }
        */
    }
    private Transform DoRaycast()
    {
        RaycastHit[] hits = Physics.RaycastAll(leftPointer.position, leftPointer.forward, maxDistance, layerMask);

        foreach(RaycastHit h in hits) 
        {
            if(h.collider.transform.GetComponent<LaserButton>() != null || h.collider.transform.GetComponent<PointerEvents>() != null)
            {
                return leftPointer;
            }
        }

        hits = Physics.RaycastAll(rightPointer.position, rightPointer.forward, maxDistance, layerMask);

        foreach (RaycastHit h in hits)
        {
            if (h.collider.transform.GetComponent<LaserButton>() != null || h.collider.transform.GetComponent<PointerEvents>() != null)
            {
                return rightPointer;
            }
        }

        return null;
    }
}
