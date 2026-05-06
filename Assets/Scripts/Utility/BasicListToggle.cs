using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicListToggle : MonoBehaviour
{
    public List<GameObject> ToggleTargets;

    public int activeIndex = 0;

    public void ToggleSequenceForward()
    {
        
        
        if (activeIndex >= ToggleTargets.Count - 1)
        {
            activeIndex = 0;
            
        }
        else
        {
            activeIndex++;
        }

        ClearAllTargets();
        ToggleTargets[activeIndex].SetActive(true);
    }

    public void ClearAllTargets()
    {
        foreach(GameObject go in ToggleTargets)
        {
            go.SetActive(false);
        }
    }
}
