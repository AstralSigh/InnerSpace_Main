using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class TouchControllerAnimation : MonoBehaviour
{
    //45 degrees

    public float triggerMaxMov = 0.0071572f;
    public float rotationMax = 35f;

    //RIGHT TRIGGER
    [SerializeField] private GameObject rTrigger;
    Vector3 rTriggerStartPos;

    //LEFT TRIGGER
    [SerializeField] private GameObject lTrigger;
    Vector3 lTriggerStartPos;

    //RIGHT GRIP
    [SerializeField] private GameObject rGrip;
    Vector3 rGripStartPos;

    //LEFT GRIP
    [SerializeField] private GameObject lGrip;
    Vector3 lGripStartPos;

    //RIGHT THUMBSTICK
    [SerializeField] private GameObject rThumbstick;
    Quaternion rThumbstickStartRot;

    //LEFT THUMBSTICK
    [SerializeField] private GameObject lThumbstick;
    Quaternion lThumbstickStartRot;

    public void Start()
    {
        rTriggerStartPos = rTrigger.transform.localPosition;
        lTriggerStartPos = lTrigger.transform.localPosition;
        rGripStartPos = rGrip.transform.localPosition;
        lGripStartPos = lGrip.transform.localPosition;
        rThumbstickStartRot = rThumbstick.transform.localRotation;
        lThumbstickStartRot = lThumbstick.transform.localRotation;
    }

    public void Update()
    {
        //Animates right trigger
        if(InputBridge.Instance.RightTrigger != 0)
        {
            float triggerMeshOffset = Mathf.Lerp(0, triggerMaxMov, InputBridge.Instance.RightTrigger);
            rTrigger.transform.localPosition = rTriggerStartPos - new Vector3(0, 0, triggerMeshOffset);
        }

        //Animates left trigger
        if (InputBridge.Instance.LeftTrigger != 0)
        {
            float triggerMeshOffset = Mathf.Lerp(0, triggerMaxMov, InputBridge.Instance.LeftTrigger);
            lTrigger.transform.localPosition = lTriggerStartPos - new Vector3(0, 0, triggerMeshOffset);
        }

        //Animates right grip
        if (InputBridge.Instance.RightGrip != 0)
        {
            float triggerMeshOffset = Mathf.Lerp(0, triggerMaxMov, InputBridge.Instance.RightGrip);
            rGrip.transform.localPosition = rGripStartPos + new Vector3(triggerMeshOffset, 0, 0);
        }

        //Animates left trigger
        if (InputBridge.Instance.LeftGrip != 0)
        {
            float triggerMeshOffset = Mathf.Lerp(0, triggerMaxMov, InputBridge.Instance.LeftGrip);
            lGrip.transform.localPosition = lGripStartPos - new Vector3(triggerMeshOffset, 0, 0);
        }

        //Animates right thumbstick 
        if (InputBridge.Instance.RightThumbstickAxis.x != 0 || InputBridge.Instance.RightThumbstickAxis.y != 0)
        {
           rThumbstick.transform.localRotation = rThumbstickStartRot * Quaternion.Euler(new Vector3(InputBridge.Instance.RightThumbstickAxis.y * rotationMax, 0, -InputBridge.Instance.RightThumbstickAxis.x * rotationMax));
        }

        //Animates left thumbstick 
        if (InputBridge.Instance.LeftThumbstickAxis.x != 0 || InputBridge.Instance.LeftThumbstickAxis.y != 0)
        {
            lThumbstick.transform.localRotation = lThumbstickStartRot * Quaternion.Euler(new Vector3(InputBridge.Instance.LeftThumbstickAxis.y * rotationMax, 0, -InputBridge.Instance.LeftThumbstickAxis.x * rotationMax));
        }
    }
}