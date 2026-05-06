using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BNG;
using UnityEngine.Events;
using System;

[Obsolete("InfoNodePanel is deprecated, please use InfoNodePanelV2 instead.")]
public class InfoNodePanel : MonoBehaviour
{
    private Text panelText;
    public GameObject topAnchor;
    public GameObject bottomAnchor;
    private GameObject collapseButton;
    private GameObject addButton;
    private RectTransform textCanvas;
    private InfoNodeHead infoNodeHead;

    // Player references. 
    private bool hasAddButton;
    int panelHeight = 90;
    
    public void InitializeInfoNodePanel(string inputText, Transform previousBottomAnchor, bool hasAddButton, InfoNodeHead infoNodeHead)
    {
        //Get references
        // TODO: We must kill all these Find()s.
        panelText = transform.Find("ToolTip").transform.Find("TextCanvas").transform.Find("Text").GetComponent<Text>();
        topAnchor = transform.Find("ToolTip").transform.Find("TextCanvas").transform.Find("TopAnchorWithLineRenderer").gameObject;
        bottomAnchor = transform.Find("ToolTip").transform.Find("TextCanvas").transform.Find("BottomAnchor").gameObject;
        collapseButton = transform.Find("ToolTip").transform.Find("TextCanvas").transform.Find("CollapseButton").gameObject;
        addButton = transform.Find("ToolTip").transform.Find("TextCanvas").transform.Find("AddButton").gameObject;
        textCanvas = transform.Find("ToolTip").transform.Find("TextCanvas").GetComponent<RectTransform>();

        //Assign variables
        panelText.text = inputText;
        SetPanelHeight();
        topAnchor.GetComponent<LineToTransform>().ConnectTo = previousBottomAnchor;
        addButton.GetComponent<InfoNodeButtons>().SetInfoNodeHead(infoNodeHead);
        collapseButton.GetComponent<InfoNodeButtons>().SetInfoNodeHead(infoNodeHead);
        this.infoNodeHead = infoNodeHead;
        this.hasAddButton = hasAddButton;

        // Michael added, date 6/6. Need to refactor this function.
        // TODO: REFACTOR THIS NIGHTMARE.
        collapseButton.SetActive(true);
        if (hasAddButton)
        {
            addButton.transform.localPosition = new Vector3(0, -panelHeight - 30, 0);
            addButton.SetActive(true);
        }
        else{
            addButton.SetActive(false);
        }
    }

    //Called by InfoNodeHead
    public void OnNewPanelAdded()
    {
        collapseButton.SetActive(false);
        addButton.SetActive(false);
    }

    public void SetPanelHeight()
    {
        //Sets panel height based on word count.
        if (panelText.text.Length > 60)
        {
            panelHeight += (((panelText.text.Length - 60) / 20) * 20);
        }
      
        textCanvas.sizeDelta = new Vector2(250, panelHeight);
        topAnchor.transform.localPosition = new Vector3(0, panelHeight / 2f, 0);
        bottomAnchor.transform.localPosition = new Vector3(0, -panelHeight / 2f, 0);
        collapseButton.transform.localPosition = new Vector3(125, panelHeight/2f, 0);
    }
}
