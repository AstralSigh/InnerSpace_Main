using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightPlus;

//UI POINTER WILL CALL THIS SCRIPT DURING LASERING. CURRENTLY NOT THE BEST WAY OF DOING THINGS.

public class InsulinFormationLaserManager : MonoBehaviour
{
    [SerializeField] private List<Transform> monomers = new List<Transform>();
    List<Transform> highlightCache = new List<Transform>();
    [SerializeField] private InsulinSavedEventTimes insulinEventTimes; // Scriptable gameObject
    [SerializeField] private float startOffset = 10f;
    [SerializeField] private float duration = 77.05f;
    [SerializeField] Constituent monomerConstituent;
    [SerializeField] Constituent dimerConstituent;
    [SerializeField] Constituent hexamerConstituent;

    private float FindMonomerTime(int inputNumber){
        foreach(InsulinSavedEventTimes.audioEvent bt in insulinEventTimes.bindingEventTimes){
            if (bt._locationName.Split('_')[0] == inputNumber.ToString())
            {
                if(bt._bindingType == InsulinSavedEventTimes.audioEvent.type.monomer){
                    return bt._time;                    
                }
            }
        }
        return 0;
    }

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
        int monomerIndex = int.Parse(parts[1]);
        float monomerBindTime = FindMonomerTime(hexamerIndex);
        float dimerBindTime = FindDimerTime(hexamerIndex);
        float currentTime = TimeManager.Instance.GetCurrentTime();
        
        //ERASE PREVIOUS HIGHLIGHTS 
        foreach(Transform hc in highlightCache){
            hc.GetComponent<HighlightEffect>().highlighted = false;
        }
        highlightCache.Clear();

        //Debug.Log("This is " + constituent.name + "with a calculated time of " + (duration + startOffset - currentTime) +"current time of " + currentTime+ "dimer binding time of " + dimerBindTime + "monomer binding time of " + monomerBindTime);

        //IF THIS IS NOT HERO HEXAMER
        if(hexamerIndex >0){
            //DO LASERING AND HIGHLIGHTING  
            if((duration + startOffset - currentTime) > monomerBindTime){
                //LASER AS MONOMER 
                constituent.GetComponent<HighlightEffect>().highlighted = true;
                highlightCache.Add(constituent);
                // mainManager.SetCurrentCon(7);
                monomerConstituent.SelectConstituentWithoutHighlight();
            }
            else if((duration + startOffset - currentTime) < monomerBindTime &&
                (duration + startOffset - currentTime) > dimerBindTime){
                //LASER AS DIMER
                if(monomerIndex % 2 == 0){
                    monomers[hexamerIndex*6 + monomerIndex].GetComponent<HighlightEffect>().highlighted = true;
                    monomers[hexamerIndex*6 + monomerIndex +1].GetComponent<HighlightEffect>().highlighted = true;
                    highlightCache.Add(monomers[hexamerIndex*6 + monomerIndex]);
                    highlightCache.Add(monomers[hexamerIndex*6 + monomerIndex+1]);
                }else{
                    monomers[hexamerIndex*6 + monomerIndex].GetComponent<HighlightEffect>().highlighted = true;
                    monomers[hexamerIndex*6 + monomerIndex -1].GetComponent<HighlightEffect>().highlighted = true;
                    highlightCache.Add(monomers[hexamerIndex*6 + monomerIndex]);
                    highlightCache.Add(monomers[hexamerIndex*6 + monomerIndex-1]);
                }
                dimerConstituent.SelectConstituentWithoutHighlight();
            } 
            else{
                //LASER AS HEXAMER
                monomers[hexamerIndex*6].GetComponent<HighlightEffect>().highlighted = true;
                monomers[hexamerIndex*6 + 1].GetComponent<HighlightEffect>().highlighted = true;
                monomers[hexamerIndex*6 + 2].GetComponent<HighlightEffect>().highlighted = true;
                monomers[hexamerIndex*6 + 3].GetComponent<HighlightEffect>().highlighted = true;
                monomers[hexamerIndex*6 + 4].GetComponent<HighlightEffect>().highlighted = true;
                monomers[hexamerIndex*6 + 5].GetComponent<HighlightEffect>().highlighted = true;
                highlightCache.Add(monomers[hexamerIndex*6]);
                highlightCache.Add(monomers[hexamerIndex*6+1]);
                highlightCache.Add(monomers[hexamerIndex*6+2]);
                highlightCache.Add(monomers[hexamerIndex*6+3]);
                highlightCache.Add(monomers[hexamerIndex*6+4]);
                highlightCache.Add(monomers[hexamerIndex*6+5]);
                hexamerConstituent.SelectConstituentWithoutHighlight();
            }
        
        }

        //IF THIS IS HERO HEXAMER
        else{
            if(currentTime < 4.4f){
                constituent.GetComponent<HighlightEffect>().highlighted = true;
                highlightCache.Add(constituent);
                monomerConstituent.SelectConstituentWithoutHighlight();
            }
            else if(currentTime > 4.4f && currentTime < 10){
                if(monomerIndex % 2 == 0){
                    monomers[hexamerIndex*6 + monomerIndex].GetComponent<HighlightEffect>().highlighted = true;
                    monomers[hexamerIndex*6 + monomerIndex +1].GetComponent<HighlightEffect>().highlighted = true;
                    highlightCache.Add(monomers[hexamerIndex*6 + monomerIndex]);
                    highlightCache.Add(monomers[hexamerIndex*6 + monomerIndex+1]);
                }else{
                    monomers[hexamerIndex*6 + monomerIndex].GetComponent<HighlightEffect>().highlighted = true;
                    monomers[hexamerIndex*6 + monomerIndex -1].GetComponent<HighlightEffect>().highlighted = true;
                    highlightCache.Add(monomers[hexamerIndex*6 + monomerIndex]);
                    highlightCache.Add(monomers[hexamerIndex*6 + monomerIndex-1]);
                    dimerConstituent.SelectConstituentWithoutHighlight();
                }
            }
            else{
                monomers[hexamerIndex*6].GetComponent<HighlightEffect>().highlighted = true;
                monomers[hexamerIndex*6 + 1].GetComponent<HighlightEffect>().highlighted = true;
                monomers[hexamerIndex*6 + 2].GetComponent<HighlightEffect>().highlighted = true;
                monomers[hexamerIndex*6 + 3].GetComponent<HighlightEffect>().highlighted = true;
                monomers[hexamerIndex*6 + 4].GetComponent<HighlightEffect>().highlighted = true;
                monomers[hexamerIndex*6 + 5].GetComponent<HighlightEffect>().highlighted = true;
                highlightCache.Add(monomers[hexamerIndex*6]);
                highlightCache.Add(monomers[hexamerIndex*6+1]);
                highlightCache.Add(monomers[hexamerIndex*6+2]);
                highlightCache.Add(monomers[hexamerIndex*6+3]);
                highlightCache.Add(monomers[hexamerIndex*6+4]);
                highlightCache.Add(monomers[hexamerIndex*6+5]);
                hexamerConstituent.SelectConstituentWithoutHighlight();
            }
        } 
    }
}
