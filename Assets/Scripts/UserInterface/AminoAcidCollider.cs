using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AminoAcidCollider : MonoBehaviour
{
    protected BNG.PointerEvents pointerEvent;
    public AminoAcidLabel acidLabelScript;
    
    [TextArea(3,3)]
    public string localAminoAcidName;


    void Start()
    {
        pointerEvent = transform.GetComponent<BNG.PointerEvents>();
        pointerEvent.OnPointerEnterEvent.AddListener(PointerClickEvent);
        pointerEvent.OnPointerExitEvent.AddListener(PointerExitEvent);
    }

    protected virtual void PointerClickEvent(UnityEngine.EventSystems.PointerEventData data)
    {
        acidLabelScript.SummonAcidLabel(this.gameObject.transform.position, localAminoAcidName);
        FMODUnity.RuntimeManager.PlayOneShot2D("event:/UI Events/laser_confirm");
    }

    protected virtual void PointerExitEvent(UnityEngine.EventSystems.PointerEventData data)
    {
        acidLabelScript.CloseAcidLabel();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Hand")
        {
            acidLabelScript.SummonAcidLabel(this.gameObject.transform.position, localAminoAcidName);
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Hand")
        {
            acidLabelScript.CloseAcidLabel();
        }
    }

    private void OnMouseUp()
    {
        acidLabelScript.SummonAcidLabel(this.gameObject.transform.position, localAminoAcidName);
    }

    private void OnMouseExit()
    {
        acidLabelScript.CloseAcidLabel();
    }
}
