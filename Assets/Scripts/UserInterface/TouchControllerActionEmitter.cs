using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
using System;
using System.Linq;
using UnityEngine.InputSystem;

public class TouchControllerActionEmitter : MonoBehaviour
{
    class InputInfo
    {
        public Action downAction;
        public Action upAction;
        public bool lastFrame;
    };

    public static TouchControllerActionEmitter Instance;

    bool skipToGCluster = false;
    Dictionary<TutorialInputs.InputName, InputInfo> inputActionMap;

    private void Awake()
    {
        if (Instance == null) {Instance = this; }
        else {Destroy(this);}

        inputActionMap = new Dictionary<TutorialInputs.InputName, InputInfo>();

        foreach (TutorialInputs.InputName e in Enum.GetValues(typeof(TutorialInputs.InputName)))
        {
            inputActionMap.Add(e, new InputInfo());
        }
    }

    private void Start()
    {

    }
    public void Update()
    {
        TryInput(TutorialInputs.InputName.rightTrigger, InputBridge.Instance.RightTrigger > 0.0f);
        TryInput(TutorialInputs.InputName.leftTrigger, InputBridge.Instance.LeftTrigger > 0.0f);
        TryInput(TutorialInputs.InputName.rightGrip, InputBridge.Instance.RightGrip > 0.0f);
        TryInput(TutorialInputs.InputName.leftGrip, InputBridge.Instance.LeftGrip > 0.0f);
        TryInput(TutorialInputs.InputName.rightThumbstick, InputBridge.Instance.RightThumbstickAxis.x != 0.0f || InputBridge.Instance.RightThumbstickAxis.y != 0.0f);
        TryInput(TutorialInputs.InputName.leftThumbstick, InputBridge.Instance.LeftThumbstickAxis.x != 0.0f || InputBridge.Instance.LeftThumbstickAxis.y != 0.0f);

        if(InputBridge.Instance.AButton && InputBridge.Instance.YButton && !skipToGCluster)
        {
            skipToGCluster = true;
            OnDownAction(TutorialInputs.InputName.skip);
        }

        if (InputBridge.Instance.LeftThumbstickDown && InputBridge.Instance.RightThumbstick)
        {
            OnDownAction(TutorialInputs.InputName.skipOneBeat);
        }
    }

    private void TryInput(TutorialInputs.InputName inputName, bool condition)
    {
        if (!inputActionMap[inputName].lastFrame && condition)
        {
            OnDownAction(inputName);
        }
        else if (inputActionMap[inputName].lastFrame && !condition)
        {
            OnUpAction(inputName);
        }

        inputActionMap[inputName].lastFrame = condition;
    }

    public void SubscribeToEventDown(TutorialInputs.InputName inputName, Action action)
    {
        if (inputName == TutorialInputs.InputName.anyButton)
        {
            inputActionMap[TutorialInputs.InputName.leftTrigger].downAction += action;
            inputActionMap[TutorialInputs.InputName.rightTrigger].downAction += action;
            inputActionMap[TutorialInputs.InputName.leftGrip].downAction += action;
            inputActionMap[TutorialInputs.InputName.rightGrip].downAction += action;
            inputActionMap[TutorialInputs.InputName.leftThumbstick].downAction += action;
            inputActionMap[TutorialInputs.InputName.rightThumbstick].downAction += action;
        }
        else
        {
            inputActionMap[inputName].downAction += action;
        }
    }

    public void UnsubscribeFromEventDown(TutorialInputs.InputName inputName, Action action)
    {
        if (inputName == TutorialInputs.InputName.anyButton)
        {
            inputActionMap[TutorialInputs.InputName.leftTrigger].downAction -= action;
            inputActionMap[TutorialInputs.InputName.rightTrigger].downAction -= action;
            inputActionMap[TutorialInputs.InputName.leftGrip].downAction -= action;
            inputActionMap[TutorialInputs.InputName.rightGrip].downAction -= action;
            inputActionMap[TutorialInputs.InputName.leftThumbstick].downAction -= action;
            inputActionMap[TutorialInputs.InputName.rightThumbstick].downAction -= action;
        }
        else
        {
            inputActionMap[inputName].downAction -= action;
        }
    }

    public void SubscribeToEventUp(TutorialInputs.InputName inputName, Action action)
    {
        if (inputName == TutorialInputs.InputName.anyButton)
        {
            inputActionMap[TutorialInputs.InputName.leftTrigger].upAction += action;
            inputActionMap[TutorialInputs.InputName.rightTrigger].upAction += action;
            inputActionMap[TutorialInputs.InputName.leftGrip].upAction += action;
            inputActionMap[TutorialInputs.InputName.rightGrip].upAction += action;
            inputActionMap[TutorialInputs.InputName.leftThumbstick].upAction += action;
            inputActionMap[TutorialInputs.InputName.rightThumbstick].upAction += action;
        }
        else
        {
            inputActionMap[inputName].upAction += action;
        }
    }

    public void UnsubscribeFromEventUp(TutorialInputs.InputName inputName, Action action)
    {
        if (inputName == TutorialInputs.InputName.anyButton)
        {
            inputActionMap[TutorialInputs.InputName.leftTrigger].upAction -= action;
            inputActionMap[TutorialInputs.InputName.rightTrigger].upAction -= action;
            inputActionMap[TutorialInputs.InputName.leftGrip].upAction -= action;
            inputActionMap[TutorialInputs.InputName.rightGrip].upAction -= action;
            inputActionMap[TutorialInputs.InputName.leftThumbstick].upAction -= action;
            inputActionMap[TutorialInputs.InputName.rightThumbstick].upAction -= action;
        }
        else
        {
            inputActionMap[inputName].upAction -= action;
        }
    }

    public void OnDownAction(TutorialInputs.InputName inputName)
    {
        if (inputActionMap[inputName] != null && inputActionMap[inputName].downAction != null) { inputActionMap[inputName].downAction(); }
    }

    public void OnUpAction(TutorialInputs.InputName inputName)
    {
        if (inputActionMap[inputName] != null && inputActionMap[inputName].upAction != null) { inputActionMap[inputName].upAction(); }
    }
}
