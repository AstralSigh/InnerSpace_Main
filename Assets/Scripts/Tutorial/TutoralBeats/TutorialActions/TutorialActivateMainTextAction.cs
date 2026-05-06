using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialActivateMainTextAction : BaseTutorialAction_Prototyping
{
    [SerializeField] string assignedText;
    public override void Run(TutorialBeat_Prototyping beat)
    {
        TutorialManager_Prototyping.Instance.tutorialText.enabled = true;
        TutorialManager_Prototyping.Instance.tutorialText.text = assignedText;
    }
}