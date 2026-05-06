using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTimeCondition : BaseTutorialCondition_Prototyping
{
    [SerializeField] float time;
    bool completed = false;

    public override void EnableCondition()
    {
        completed = false;
        Invoke("MarkAsCompleted", time);
    }

    public override void DisableCondition()
    {

    }

    public override bool CheckCondition() {return completed;}

    void MarkAsCompleted()
    {
        completed = true;
    }
    
}
