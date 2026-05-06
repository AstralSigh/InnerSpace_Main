using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnableSmoothLocomotionAction : BaseTutorialAction_Prototyping
{
    public override void Run(TutorialBeat_Prototyping beat)
    {
        TutorialManager_Prototyping.Instance.inputManager.GetReferencedObject(TutorialInputs.ObjectName.player).GetComponent<SmoothLocomotion>().enabled = true;
    }
}
