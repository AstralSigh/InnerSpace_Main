using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VantagePointButton : MonoBehaviour
{
    [SerializeField] int buttonIndex;
    [SerializeField] MeshRenderer buttonObject;
    [SerializeField] TextMeshProUGUI buttonText;
    [SerializeField]
    private Color darkGrey;
    [SerializeField]
    private Color idleGrey;
    [SerializeField]
    private Color fullWhite;
    [SerializeField]
    private Color hoverGrey;

    private enum VPColorStates { Idle, OnHover, Selected };

    void Start()
    {
        buttonText.SetText((buttonIndex + 1).ToString());

        VantagePointManager.Instance.OnDeleteVantagePoint += onDeleteVantagePoint;
        WIAC_Manager.Instance.OnChangeNexus += updateHighlightStatus;
    }

    public void ButtonPress()
    {
        if(VantagePointManager.Instance.VantagePointIsActive(buttonIndex))
        {
            // If vantage point is active, teleport.
            VantagePointManager.Instance.TeleportPlayerToVantagePoint(buttonIndex);
        }
        else
        {
            // If vantage point is not active, enable it.
            VantagePointManager.Instance.EnableVantagePoint(buttonIndex);
            UpdateColor(VPColorStates.Selected);
        }
    }

    private void onDeleteVantagePoint(int index)
    {
        if(index == buttonIndex)
        {
            UpdateColor(VPColorStates.Idle);
        }
    }

    private void updateHighlightStatus(Nexus_Data.eNexusType nexusType)
    {
        if(VantagePointManager.Instance.VantagePointIsActive(buttonIndex))
        {
            UpdateColor(VPColorStates.Selected);
        }
        else
        {
            UpdateColor(VPColorStates.Idle);
        }
    }

    public void OnHover()
    {
        UpdateColor(VPColorStates.OnHover);
    }

    public void OnExit()
    {
        if (VantagePointManager.Instance.VantagePointIsActive(buttonIndex))
        {
            UpdateColor(VPColorStates.Selected);
        }
        else
        {
            UpdateColor(VPColorStates.Idle);
        }
    }

    private void UpdateColor(VPColorStates Color)
    {
        switch (Color)
        {
            case VPColorStates.Idle:
                buttonObject.material.color = darkGrey;
                buttonText.color = idleGrey;
                break;
            case VPColorStates.OnHover:
                buttonObject.material.color = fullWhite;
                buttonText.color = hoverGrey;
                break;
            case VPColorStates.Selected:
                buttonObject.material.color = hoverGrey;
                buttonText.color = fullWhite;
                break;
        }
 
    }

}
