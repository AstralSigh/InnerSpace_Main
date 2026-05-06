using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionMenuButton : MonoBehaviour
{
    public enum Feature { Time, Data, Tour, Map, Game, Location };
    public Feature feature;
    public enum ButtonState { Disabled, Idle, OnHover, OnSelect };
    public ButtonState currentButtonState;

    public UnityEvent OnItemClick;
    public UnityEvent OnItemDeselect;
    
    public List<Nexus_Data.eNexusType> allowedNexuses; // The nexuses where this menu item will be enabled.
    public Texture[] buttonTextures;
    
    public ActionMenuButton(Texture[] buttonTextures)
    {
        this.buttonTextures = buttonTextures;
    }

    public void SetButtonState(ButtonState currentButtonState)
    {
        this.currentButtonState = currentButtonState;
        MeshRenderer renderer = transform.GetComponent<MeshRenderer>();
        renderer.material.SetTexture("_BaseMap", buttonTextures[(int)currentButtonState]);
    }

    public void ResetMenu(Nexus_Data.eNexusType currentNexus)
    {
        if(currentButtonState == ButtonState.OnSelect)
        {
            Deselect();
        }
        if(allowedNexuses.Contains(currentNexus))
        {
            SetButtonState(ButtonState.Idle);
        }
        else
        {
            SetButtonState(ButtonState.Disabled);
        }
    }

    public void OnPointerSelect()
    {
        if(currentButtonState == ButtonState.Disabled)
        {
            return;
        }

        ActionMenu_NickPrototype.Instance.DeselectMenu();
        
        SetButtonState(ButtonState.OnSelect);
        OnItemClick.Invoke();
    }

    public void OnPointerHover()
    {
        if(currentButtonState == ButtonState.Idle)
        {
            SetButtonState(ButtonState.OnHover);
        }
    }

    public void OnPointerExit()
    {
        if(currentButtonState == ButtonState.OnHover)
        {
            SetButtonState(ButtonState.Idle);
        }
    }

    public void Deselect()
    {
        if(currentButtonState == ButtonState.OnSelect)
        {
            OnItemDeselect.Invoke();
            SetButtonState(ButtonState.Idle);
        }
    }
}
