using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGrabbable : MonoBehaviour
{
    [SerializeField] Transform lockToPlane;  // none/null for no lock to plane
    Transform originalParentTransform;
    Vector3 originalLocalPosition;
    [SerializeField] bool returnToOriginalPosition;

    bool isGrabbing;
    
    protected void Awake()
    {
        BNG.PointerEvents events = GetComponent<BNG.PointerEvents>();
        if(events)
        {
            events.OnPointerClickEvent.AddListener(PointerClickEvent);
            events.OnPointerEnterEvent.AddListener(PointerEnterEvent);
            events.OnPointerExitEvent.AddListener(PointerExitEvent);
            events.OnPointerDownEvent.AddListener(PointerDownEvent);
            events.OnPointerUpEvent.AddListener(PointerUpEvent);
        }
    }

    void Start()
    {
        // We remember the original parent transform of this object so that we can reset.
        originalParentTransform = this.gameObject.transform.parent;
        originalLocalPosition = this.gameObject.transform.localPosition;
        PointerManager.Instance.OnDisablePointer += OnPointerDisable;
    }

    void LateUpdate() {
        if (isGrabbing && lockToPlane) {
            Plane lockPlane = new(lockToPlane.forward, lockToPlane.position);
            Vector3 handHoldDir = transform.parent.forward;
            Ray holdRay = new(transform.position, handHoldDir);
            float t;
            if (lockPlane.Raycast(holdRay, out t)) {
                transform.position += t*handHoldDir;
            } else if (lockPlane.Raycast(new Ray(holdRay.origin, -holdRay.direction), out t)) {
                transform.position -= t*handHoldDir;
            }
        }
    }

    protected virtual void PointerClickEvent(UnityEngine.EventSystems.PointerEventData data)
    {
        
    }

    protected virtual void PointerEnterEvent(UnityEngine.EventSystems.PointerEventData data)
    {

    }

    protected virtual void PointerExitEvent(UnityEngine.EventSystems.PointerEventData data)
    {

    }

    protected virtual void PointerDownEvent(UnityEngine.EventSystems.PointerEventData data)
    {
        if (ActionMenuManager_Prototype.Instance.GetCurrentLaserHand() == ControllerHand.Left)
        {
            LaserGrabbableManager.Instance.LockToLeftHand(this.gameObject);
            isGrabbing = true;
        }
        else if (ActionMenuManager_Prototype.Instance.GetCurrentLaserHand() == ControllerHand.Right)
        {
            LaserGrabbableManager.Instance.LockToRightHand(this.gameObject);
            isGrabbing = true;
        }
    }

    protected virtual void PointerUpEvent(UnityEngine.EventSystems.PointerEventData data)
    {
        // Reset the parent transform to unlock from hand.
        Reset();
    }

    void OnPointerDisable()
    {
        // Reset the parent transform to unlock from hand.
        Reset();
    }

    void Reset()
    {
        if(!this) return; // TODO: Why do we need to do this? We get bugs if we don't.
        isGrabbing = false;
        LaserGrabbableManager.Instance.UnlockFromHands(this.gameObject, originalParentTransform);
        if(returnToOriginalPosition) this.gameObject.transform.localPosition = originalLocalPosition;
    }
}
