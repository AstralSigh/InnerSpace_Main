using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Constituent : MonoBehaviour
{
    [SerializeField] Text tourInfoPanel;
    [SerializeField] ConstituentData data;
    private bool selectable = true;

    void Start()
    {
        WIAC_Manager.Instance.OnToggleNexusTour += OnToggleNexusTour;
        WIAC_Manager.Instance.OnNexusTourStopChanged += OnNexusTourStopChanged;
        if (tourInfoPanel) {
            tourInfoPanel.text += $"label\n{data.conSubHeader}\nPDB nums:{data.conPDBNums}\n{data.conFunction}";
        }
    }

    bool nexusTourActive = false;
    void OnToggleNexusTour(bool nexusTourActive)
    {
        selectable = !nexusTourActive;
        /*
        this.nexusTourActive = nexusTourActive;
        if (!nexusTourActive) {
            selectable = false;
        }
        */
    }

    void OnNexusTourStopChanged(TourStop_Prototyping tourStop) {
        /*
        selectable = tourStop.selectableConstituentTypes.Count == 0 || tourStop.selectableConstituentTypes.Contains(data.eConType);
        if (!selectable) {
            tourInfoPanel?.gameObject?.SetActive(false);
        }
        */
    }

    void DoSelectionWhileOnNexusTour() {
        //tourInfoPanel.gameObject.SetActive(!tourInfoPanel.gameObject.activeSelf);
    }

    /// <summary>
    /// SelectConstituent is called from a LaserCollider pointer event within the constituent's hierarchy. (as of 7/18/2023)
    /// </summary>
    public void SelectConstituent()
    {
        if (nexusTourActive) {
            //DoSelectionWhileOnNexusTour();
            //return;
        }

        if(selectable) SelectedConstituentManager.Instance.SetConstituent(this, true);
    }

    /// <summary>
    /// SelectConstituentWithoutHighlight is called from a LaserCollider pointer event within the constituent's hierarchy. (as of 7/18/2023)
    /// </summary>
    public void SelectConstituentWithoutHighlight()
    {
        if (nexusTourActive) {
            DoSelectionWhileOnNexusTour();
            return;
        }

        if(selectable) SelectedConstituentManager.Instance.SetConstituent(this, false);
    }

    public ConstituentData GetData()
    {
        return data;
    }
}
