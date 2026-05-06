using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialJumpBeat_Prototyping : BaseTutorialDependentBeat_Prototyping
{
    [SerializeField] int jumpToBeatNumber;
    public BaseTutorialCondition_Prototyping jumpCondition;

    public override void BeginBeat(TutorialBeat_Prototyping beat)
    {
        base.BeginBeat(beat);
        jumpCondition.EnableCondition();
    }

    public override void EndBeat(TutorialBeat_Prototyping beat)
    {
        base.EndBeat(beat);
        jumpCondition.DisableCondition();
    }
    public override void OnBeatUpdate()
    {
        base.OnBeatUpdate();
        if (jumpCondition.CheckCondition())
        {
            parentBeat.CancelBeat(jumpToBeatNumber);
        }
    }
}
