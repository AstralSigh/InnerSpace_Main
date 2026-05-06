using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightPlus;
public class InsulinReleaseLaserManager : MonoBehaviour
{
    [SerializeField] private List<Transform> dimers = new List<Transform>();
    List<Transform> highlightCache = new List<Transform>();
    [SerializeField] private InsulinSavedEventTimes insulinEventTimes; // Scriptable gameObject
    [SerializeField] private float startOffset = 26.06f;
    [SerializeField] Constituent dimerConstituent;
    [SerializeField] Constituent hexamerConstituent;

    private float FindDimerTime(int inputNumber){
        foreach(InsulinSavedEventTimes.audioEvent bt in insulinEventTimes.bindingEventTimes){
            if (bt._locationName.Split('_')[0] == inputNumber.ToString())
            {
                if(bt._bindingType == InsulinSavedEventTimes.audioEvent.type.dimer){
                    return bt._time;                    
                }
            }
        }
        return float.MaxValue;
    }

    public void HighlightConstituent(Transform constituent){
        //PARSE NAME FOR INDEX'S 
        string input = constituent.gameObject.name;
        string[] parts = input.Split('_');
        int hexamerIndex = int.Parse(parts[0]);
        float dimerBindTime = FindDimerTime(hexamerIndex);
        float currentTime = TimeManager.Instance.GetCurrentTime();
        
        //ERASE PREVIOUS HIGHLIGHTS 
        foreach(Transform hc in highlightCache){
            hc.GetComponent<HighlightEffect>().highlighted = false;
        }
        highlightCache.Clear();

        //DO LASERING AND HIGHLIGHTING  
        if(currentTime - startOffset < dimerBindTime / 2){
            //LASER AS HEXAMER 
            dimers[hexamerIndex*3].GetComponent<HighlightEffect>().highlighted = true;
            dimers[hexamerIndex*3 + 1].GetComponent<HighlightEffect>().highlighted = true;
            dimers[hexamerIndex*3 + 2].GetComponent<HighlightEffect>().highlighted = true;

            highlightCache.Add(dimers[hexamerIndex*3]);
            highlightCache.Add(dimers[hexamerIndex*3+1]);
            highlightCache.Add(dimers[hexamerIndex*3+2]);
            hexamerConstituent.SelectConstituentWithoutHighlight();
            
        }
        else{
            //LASER AS DIMER 
            constituent.GetComponent<HighlightEffect>().highlighted = true;
            highlightCache.Add(constituent);
            dimerConstituent.SelectConstituentWithoutHighlight();
        } 
    }
}
