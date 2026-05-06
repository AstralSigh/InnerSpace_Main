using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTutorialDependentBeat_Prototyping : MonoBehaviour
{
    protected TutorialBeat_Prototyping parentBeat;
    public List<BaseTutorialAction_Prototyping> beginActions;
    public List<BaseTutorialAction_Prototyping> endActions;

    public virtual void BeginBeat(TutorialBeat_Prototyping beat)
    {
        parentBeat = beat;

        foreach (BaseTutorialAction_Prototyping a in beginActions)
        {
            a.Run(beat);
        }
    }

    public virtual void EndBeat(TutorialBeat_Prototyping beat)
    {
        parentBeat = beat;
        foreach (BaseTutorialAction_Prototyping a in endActions)
        {
            a.Run(beat);
        }
    }

    public virtual void OnBeatUpdate()
    {

    }
}
