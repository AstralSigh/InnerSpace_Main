using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBeat_Prototyping : MonoBehaviour
{
    private int nextIndex = -1;
    private bool active;
    public BaseTutorialCondition_Prototyping beatCondition;

    public List<BaseTutorialAction_Prototyping> beginActions;
    public List<BaseTutorialDependentBeat_Prototyping> dependentBeats;
    public List<BaseTutorialAction_Prototyping> endActions;

    public virtual void BeginBeat()
    {
        active = true;
        beatCondition.EnableCondition();

        foreach (BaseTutorialAction_Prototyping a in beginActions)
        {
            a.Run(this);
        }

        foreach (BaseTutorialDependentBeat_Prototyping b in dependentBeats)
        {
            b.BeginBeat(this);
        }
    }

    public virtual void EndBeat()
    {
        foreach (BaseTutorialAction_Prototyping a in endActions)
        {
            a.Run(this);
        }

        foreach (BaseTutorialDependentBeat_Prototyping b in dependentBeats)
        {
            b.EndBeat(this);
        }

        beatCondition.DisableCondition();
    }

    public virtual bool CheckBeat(ref int nextBeatIndex)
    {
        foreach (BaseTutorialDependentBeat_Prototyping d in dependentBeats)
        {
            d.OnBeatUpdate();
        }

        if (!active)
        {
            nextBeatIndex = nextIndex;
            return true;
        }
        else if (beatCondition.CheckCondition())
        {
            nextBeatIndex++;
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void CancelBeat(int nextIndex)
    {
        this.nextIndex = nextIndex;
        active = false;
    }
}
