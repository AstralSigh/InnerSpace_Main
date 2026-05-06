using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedConstituentManager : MonoBehaviour
{
    public static SelectedConstituentManager Instance { get; private set; }

    Constituent currentConstituent;

    public delegate void ConChangeEvent(Constituent selectedConstituent);
    public event ConChangeEvent OnConstituentSelect;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        WIAC_Manager.Instance.OnChangeNexus += DeselectConstituent;
        WIAC_Manager.Instance.OnToggleNexusTour += DeselectConstituent;
    }

    private void OnDestroy()
    {
        WIAC_Manager.Instance.OnChangeNexus -= DeselectConstituent;
        WIAC_Manager.Instance.OnToggleNexusTour -= DeselectConstituent;
    }

    public void SetConstituent(Constituent constituent, bool highlight)
    {
        // If current constituent is the same as the new constituent, do nothing.
        if(constituent == currentConstituent) return;

        if(highlight) // NOTE: The following highlight functionality requires that the highlight component is a sibling of the Constituent component.
        {
            // Turn off highlight for the previous constituent.
            if(currentConstituent)
            {
                if (currentConstituent.gameObject.GetComponent<HighlightPlus.HighlightEffect>() != null)
                {
                    currentConstituent.gameObject.GetComponent<HighlightPlus.HighlightEffect>().highlighted = false;
                }
            }
                
            // Turn on highlight for new constituent.
            constituent.gameObject.GetComponent<HighlightPlus.HighlightEffect>().highlighted = true;
        }

        // Otherwise, set the new constituent to be the current one.
        currentConstituent = constituent;

        // Fire constituent selection event.
        if (OnConstituentSelect != null) OnConstituentSelect(currentConstituent);
    }

    public void DeselectConstituent<T>(T eventData)
    {
        if(currentConstituent) currentConstituent.gameObject.GetComponent<HighlightPlus.HighlightEffect>().highlighted = false;
        currentConstituent = null;

        // Should also inform no Constitent is selected
         OnConstituentSelect(null);
    }

    public ConstituentData GetCurrentData()
    {
        return currentConstituent.GetData();
    }
}
