using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightManager : MonoBehaviour
{
    //Objects to highlight
    public List<GameObject> objectReferences;

    //Durations to input
    public float addHighlightDuration;
    public float RemoveHighlightDuration;

    public void AddHighlight(TutorialInputs.TooltipName objectToEdit)
    {
        if ((int)objectToEdit < 8) //Index 8 means there is no tooltip
        {
            Renderer[] materialsToEdit = objectReferences[(int)objectToEdit].transform.GetComponentsInChildren<Renderer>();
            if (materialsToEdit == null)
            {
                Debug.Log("objectToEdit does not have any materials");
            }
            StartCoroutine(LerpHighlightOn(materialsToEdit));
        } 
    }

    private IEnumerator LerpHighlightOn(Renderer[] materialsToEdit)
    {
        float time = 0;

        while (time < addHighlightDuration)
        {
            float colorValue = (time / addHighlightDuration);

            for(int i = 0; i < materialsToEdit.Length; i++)
            {
                materialsToEdit[i].material.EnableKeyword("_EMISSION");
                materialsToEdit[i].material.SetColor("_EmissionColor", new Color(1, 1, 1) * colorValue);
            }
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public void RemoveHighlight(TutorialInputs.TooltipName objectToEdit)
    {
        if ((int)objectToEdit < 8) //Index 8 means there is no tooltip
        {
            Renderer[] materialsToEdit = objectReferences[(int)objectToEdit].transform.GetComponentsInChildren<Renderer>();
            if (materialsToEdit == null)
            {
                Debug.Log("objectToEdit does not have any materials");
            }
            StartCoroutine(LerpHighlightOff(materialsToEdit));
        }
    }

    public IEnumerator LerpHighlightOff(Renderer[] materialsToEdit)
    {
        float time = 0;

        while (time < RemoveHighlightDuration)
        {
            float colorValue = 1 - ((time / RemoveHighlightDuration));

            for (int i = 0; i < materialsToEdit.Length; i++)
            {
                materialsToEdit[i].material.EnableKeyword("_EMISSION");
                materialsToEdit[i].material.SetColor("_EmissionColor", new Color(1, 1, 1) * colorValue);
            }
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

}
