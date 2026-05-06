using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInputCondition : BaseTutorialCondition_Prototyping
{
    [SerializeField] List<TutorialInputs.InputName> triggerCondition;
    bool completed = false;

    System.Action inputAction;

    public override void EnableCondition()
    {
        completed = false;
        inputAction = InputActionEvent;
        foreach (TutorialInputs.InputName i in triggerCondition)
        {
            if (TouchControllerActionEmitter.Instance != null)
            {
                TouchControllerActionEmitter.Instance.SubscribeToEventDown(i, inputAction);
            }
        }
    }

    public override void DisableCondition()
    {
        foreach (TutorialInputs.InputName i in triggerCondition)
        {
            if (TouchControllerActionEmitter.Instance != null)
            {
                TouchControllerActionEmitter.Instance.UnsubscribeFromEventDown(i, inputAction);
            }
        }
        inputAction = null;
    }

    public void InputActionEvent()
    {
        completed = true;
    }

    public override bool CheckCondition()
    {
        if (completed)
        {
            completed = false;
            return true;
        }
        else
        {
            return false;
        }
    }
}
