using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTriggerActionBeat_Prototyping : BaseTutorialDependentBeat_Prototyping
{
    public List<BaseTutorialAction_Prototyping> triggerActions;
    public BaseTutorialCondition_Prototyping triggerCondition;

    public override void BeginBeat(TutorialBeat_Prototyping beat)
    {
        base.BeginBeat(beat);
        triggerCondition.EnableCondition();
    }

    public override void EndBeat(TutorialBeat_Prototyping beat)
    {
        base.EndBeat(beat);
        triggerCondition.DisableCondition();
    }
    public override void OnBeatUpdate()
    {
        base.OnBeatUpdate();
        if (triggerCondition.CheckCondition())
        {
            TriggerActions();
        }
    }

    protected void TriggerActions()
    {
        foreach (BaseTutorialAction_Prototyping a in triggerActions)
        {
            a.Run(parentBeat);
        }
    }
}
