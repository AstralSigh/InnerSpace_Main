using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDisableReferencedObjectAction : BaseTutorialAction_Prototyping
{
    [SerializeField] List<TutorialInputs.ObjectName> tutorialObjects;
    public override void Run(TutorialBeat_Prototyping beat)
    {
        foreach (var o in tutorialObjects)
        {
            TutorialManager_Prototyping.Instance.inputManager.GetReferencedObject(o).SetActive(false);
        }
    }
}
