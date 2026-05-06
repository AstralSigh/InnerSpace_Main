using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDisableMainTextAction : BaseTutorialAction_Prototyping
{
    public override void Run(TutorialBeat_Prototyping beat)
    {
        TutorialManager_Prototyping.Instance.tutorialText.enabled = false;
    }
}
