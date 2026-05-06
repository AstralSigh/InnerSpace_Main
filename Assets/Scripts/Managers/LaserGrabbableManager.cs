using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGrabbableManager : MonoBehaviour
{
    public static LaserGrabbableManager Instance { get; private set; }

    [SerializeField] public GameObject leftHand;
    [SerializeField] public GameObject rightHand;
    bool isGrabbingLeft;
    bool isGrabbingRight;

    void Awake()
    {
        Instance = this;
    }

    public void LockToLeftHand(GameObject grabbedObject)
    {
        if(!isGrabbingLeft)
        {
            grabbedObject.transform.parent = leftHand.transform;
            isGrabbingLeft = true;
        }
    }

    public void LockToRightHand(GameObject grabbedObject)
    {
        if(!isGrabbingRight)
        {
            grabbedObject.transform.parent = rightHand.transform;
            isGrabbingRight = true;
        }
    }

    public void UnlockFromHands(GameObject grabbedObject, Transform originalParentTransform)
    {
        grabbedObject.transform.parent = originalParentTransform;
        isGrabbingLeft = false;
        isGrabbingRight = false;
    }
}
