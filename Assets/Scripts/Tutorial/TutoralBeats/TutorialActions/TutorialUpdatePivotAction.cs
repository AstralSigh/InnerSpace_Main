using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUpdatePivotAction : BaseTutorialAction_Prototyping
{
    [SerializeField] TutorialInputs.TPPivot pivot;
    public override void Run(TutorialBeat_Prototyping beat)
    {
        TutorialManager_Prototyping.Instance.pivotManager.UpdatePivot(pivot);
    }
}
