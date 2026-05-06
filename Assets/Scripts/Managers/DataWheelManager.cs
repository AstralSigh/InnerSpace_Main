using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataWheelManager : MonoBehaviour
{
    public WIAC_Manager mainManager;
    
    public Text headerText_Data; //Header text of the data wheel
    public Text subHeaderText_Data; //Subheader text of the data wheel
    public List<Text> textFields_Data; //text fields for Profile constituent data (PDB ID, Dimensions, etc...)
    public GameObject[] textCanvas_Data; //canvas objects
    [SerializeField] GameObject profileCanvas;
    bool profileCanvasActive;
    [SerializeField] Image profileFilled, nodeFilled; //Image sprites for profile and node buttons
    [SerializeField] int characterLimit;
    public GameObject[] desktopDataPanels;

    private void OnEnable()
    {
        if (desktopDataPanels.Length > 0)
            SetDesktopPanels(true);

        textCanvas_Data[1].SetActive(false);
    }

    private void OnDisable()
    {
        if (desktopDataPanels.Length > 0)
            SetDesktopPanels(false);

        SetProfileCanvasState(false);
    }

    // public void SetDataProfileText()
    // {
    //     headerText_Data.text = mainManager.currentConData.conHeader;
    //     subHeaderText_Data.text = mainManager.currentConData.conSubHeader;

    //     textFields_Data[0].text = mainManager.currentConData.conPDBNums;
    //     textFields_Data[1].text = mainManager.currentConData.conDimensions;
    //     textFields_Data[2].text = mainManager.currentConData.conFunction;
    //     textFields_Data[3].text = mainManager.currentConData.conOrigin;
    //     textFields_Data[4].text = mainManager.currentConData.conInfoSources;
    //     if (textFields_Data[4].text.Length > characterLimit)
    //     {
    //         int index = textFields_Data[4].text.LastIndexOf(' ', characterLimit);
    //         textFields_Data[4].text = textFields_Data[4].text.Substring(0, index) + "...";
    //     }
    //     //Additional Line for About field
    //     textFields_Data[5].text = mainManager.currentConData.conDescription;
    // }

    /*
    public void ToggleDataWheelCanvas(int canvasIndex)
    {
        textCanvas_Data[canvasIndex].SetActive(!textCanvas_Data[canvasIndex].activeSelf);
        switch (canvasIndex)
        {
            case 0:
                if (textCanvas_Data[1].activeSelf)
                {
                    textCanvas_Data[1].SetActive(false);
                }
                break;

            case 1:
                if (textCanvas_Data[0].activeSelf)
                {
                    textCanvas_Data[0].SetActive(false);
                }
                break;
        }
    }
    */

    //Toggle Profile canvas + button status
    public void ToggleProfileData()
    {
        switch (profileCanvasActive)
        {
            case true:
                SetProfileCanvasState(false);
                break;

            case false:
                SetProfileCanvasState(true);
                break;
        }
    }

    //Toggle Node Button status
    public void ToggleNodeButton()
    {
        if (nodeFilled.enabled)
        {
            nodeFilled.enabled = false;
        }
        else
        {
            nodeFilled.enabled = true;
        }
    }

    void SetProfileCanvasState(bool state)
    {
        profileCanvas.SetActive(state);
        profileFilled.enabled = state;
        profileCanvasActive = state;
    }

    public void CloseDataWheel()
    {
        // mainManager.ClearConHilite(); //clear highlighted constituents from Manager method
        SetProfileCanvasState(false); //set profile canvas 'off', turn off button, canvas
        nodeFilled.enabled = false; //turn off fill image for nodes
        gameObject.SetActive(false); //turn off datawheel object
    }

    void SetDesktopPanels(bool activeState)
    {
        foreach (GameObject go in desktopDataPanels)
        {
            go.SetActive(activeState);
        }
    }

    
}
