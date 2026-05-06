using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using DG.Tweening;

public class TourBeatButton_Prototyping : MonoBehaviour
{
    //Tour UI Manager will instantiate and assign all the follow values
    public TourWheelManager_Prototyping tourManager;
    public int beatIndex;
    [SerializeField] private TextMeshProUGUI textObject;

    public enum ButtonState { Inactive, Hover, CurrentBeat, Disabled }
    public ButtonState currentState = ButtonState.Inactive;
    private Tweener jumpToButtonTween;
    public void JumpToBeat()
    {
        if(tourManager!=null)
        {
            tourManager.JumpToBeat(beatIndex);
            tourManager.transform.GetComponent<TourWheelUI_Prototyping>().SetBeatButtonsInactive();
            UpdateButtonState(ButtonState.CurrentBeat);
        }
        else
        {
            Debug.LogError("Beat Button hasn't been set up");
        }
    }

    public void Start()
    {
        textObject.text = beatIndex.ToString();
    }

    public void OnHover(bool on)
    {
        if(currentState != ButtonState.CurrentBeat)
        {
            if(on) 
            {
                UpdateButtonState(ButtonState.Hover);
            }
            else
            {
                UpdateButtonState(ButtonState.Inactive);
            }
        }
    }

    public void UpdateButtonState(ButtonState state)
    {
        currentState = state;
        switch (currentState)
        {
            case ButtonState.Inactive: //0      
                //Enable
                transform.GetComponent<Collider>().enabled = true;
                transform.GetComponent<LaserButton>().enabled = true;
                //Anim
                transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().DOColor(new Color(.7f, .7f, .7f), .5f);
                break;
            case ButtonState.Hover: //1
                transform.GetComponent<LaserButton>().enabled = true;
                transform.GetComponent<Collider>().enabled = true;
                transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().DOColor(Color.white, .5f);
                break;
            case ButtonState.CurrentBeat: //2
                //Highlighted but disable function
                //Only called to set the current beat button
                transform.GetComponent<LaserButton>().enabled = false;
                transform.GetComponent<Collider>().enabled = false;
                transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().DOColor(Color.white, .5f);
                break;
            case ButtonState.Disabled: //3
                transform.GetComponent<Collider>().enabled = false;
                transform.GetComponent<LaserButton>().enabled = false;
                break;
        }
    }

}
