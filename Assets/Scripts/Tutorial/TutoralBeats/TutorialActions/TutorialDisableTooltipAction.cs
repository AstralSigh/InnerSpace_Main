using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDisableTooltipAction : BaseTutorialAction_Prototyping
{
    [SerializeField] List<TutorialInputs.TooltipName> tooltipTypes;

    public override void Run(TutorialBeat_Prototyping beat)
    {
        foreach (var t in tooltipTypes)
        {
            TutorialManager_Prototyping.Instance.tooltipManager.RemoveTooltip(t);
        }
    }
}
