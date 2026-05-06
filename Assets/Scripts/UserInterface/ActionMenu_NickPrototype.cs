using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Menu function and UI stuff, while ActionMenuManager controls the whole action menu (not specifc functions)
/// </summary>
public class ActionMenu_NickPrototype : MonoBehaviour
{
    public static ActionMenu_NickPrototype Instance { get; private set; }

    [Header("VARIABLES")]
    [SerializeField] private List<ActionMenuButton> ActionMenuButtons;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize menu for no current nexus.
        ResetMenu(Nexus_Data.eNexusType.None);

        // Subscribe to events.
        WIAC_Manager.Instance.OnChangeNexus += ChangeNexus;
        WIAC_Manager.Instance.OnToggleNexusTour += ToggleTour;
        SelectedConstituentManager.Instance.OnConstituentSelect += DisplayData;
    }

    void ToggleTour(bool nexusTourActive)
    {
        if (!nexusTourActive)
        {
            ResetMenu(Nexus_Data.eNexusType.GCluster);
        }
    }

    private void ChangeNexus(Nexus_Data.eNexusType currentNexus)
    {
        ResetMenu(currentNexus);
    }

    public void ResetMenu(Nexus_Data.eNexusType currentNexus)
    {
        foreach(ActionMenuButton button in ActionMenuButtons)
        {
            button.ResetMenu(currentNexus);
        }
    }

    public void DeselectMenu()
    {
        foreach (ActionMenuButton button in ActionMenuButtons)
        {
            button.Deselect();
        }
    }

    //Manually calls to open specific menu button. If button is disabled. This will not do anything. 
    private void OverrideSelection(ActionMenuButton.Feature feature)
    {
        DeselectMenu();

        foreach (ActionMenuButton button in ActionMenuButtons)
        {
            if(button.feature == feature)
            {
                button.OnPointerSelect();
            }
        }
    }

    private void DisplayData(Constituent constituent)
    {
        // If is unselect, do nothing
        if (constituent == null) { return; }

        OverrideSelection(ActionMenuButton.Feature.Data);
    }



}


