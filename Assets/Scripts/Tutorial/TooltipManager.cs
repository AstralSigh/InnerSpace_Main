using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public List<GameObject> tooltipReferences;

    public void AddTooltip(TutorialInputs.TooltipName toAdd)
    {
        if ((int)toAdd < 8) //Index 8 means there is no tooltip
        {
            tooltipReferences[(int)toAdd].SetActive(true);
        }
    }

    public void RemoveTooltip(TutorialInputs.TooltipName toRemove)
    {
        if ((int)toRemove < 8) //Index 8 means there is no tooltip
        {
            tooltipReferences[(int)toRemove].SetActive(false);
        }
    }

    public void RemoveAllTooltips(){
        for(int i = 0; i < 8; i++){
            tooltipReferences[i].SetActive(false);
        }
    }
}
