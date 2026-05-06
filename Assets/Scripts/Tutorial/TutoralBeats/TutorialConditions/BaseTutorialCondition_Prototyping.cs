using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTutorialCondition_Prototyping : MonoBehaviour
{
    public abstract bool CheckCondition();

    public virtual void EnableCondition() {}

    public virtual void DisableCondition() {}
}
