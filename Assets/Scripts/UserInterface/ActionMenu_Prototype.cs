using BNG;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ActionMenuItem
{
    public enum ActionMenuItemStatus { Disabled, Inactive, Hovered, Active };  //RN only uses Inactive, hovered, and active
    public ActionMenuItemStatus status;
    public SpriteRenderer displayedSprite;
    public Sprite[] sprites;
    public UnityEvent OnItemClick;
    public UnityEvent OnItemDeselect;
    public List<Nexus_Data.eNexusType> allowedNexuses; // The nexuses where this menu item will be enabled.
}

/// <summary>
/// Menu function and UI stuff, while ActionMenuManager controls the whole action menu (not specifc functions)
/// </summary>
public class ActionMenu_Prototype : MonoBehaviour
{
    [Header("VARIABLES")]
    [SerializeField] private List<ActionMenuItem> actionMenuItems;
    [SerializeField] private Color itemInactiveSpriteColor;
    [SerializeField] private Color itemNotHoveredSpriteColor;  //When other items highlighted
    [SerializeField] private Color itemHoveredSpriteColor;  //Highlight
    [SerializeField] private Color itemDisabledSpriteColor;
    [SerializeField] private float itemHoverTweenDuration;
    private Sequence hoverTweenSequence;
    private ActionMenuItem currentActivateItem;

    [Header("REFERENCES")]
    [SerializeField] private BNG.HapticsManager hapticsManager;
    [SerializeField] private ActionMenuManager_Prototype actionMenuManager;
    [SerializeField] private TextMeshPro placehodlerText; // TODO: Remove this. Not necessary.
    [SerializeField] private ActionMenuDataManager actionMenuDataManager;

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to events.
        WIAC_Manager.Instance.OnChangeNexus += ChangeNexus;
        SelectedConstituentManager.Instance.OnConstituentSelect += DisplayData;
    }

    private void ChangeNexus(Nexus_Data.eNexusType currentNexus)
    {
        ResetMenu(currentNexus);

        // Deselect all menu items to clear the action menu.
        foreach(ActionMenuItem menuItem in actionMenuItems)
        {
            menuItem.OnItemDeselect.Invoke();
        }
    }

    public void ResetMenu(Nexus_Data.eNexusType currentNexus)
    {
        for(int i=0; i < actionMenuItems.Count; i++)
        {
            ActionMenuItem menuItem = actionMenuItems[i];
            menuItem.displayedSprite.sprite = menuItem.sprites[1];

            // Enable/disable menu buttons based on current nexus (possibly None).
            if(menuItem.allowedNexuses.Contains(currentNexus))
            {
                menuItem.status = ActionMenuItem.ActionMenuItemStatus.Inactive;
            }
            else
            {
                menuItem.status = ActionMenuItem.ActionMenuItemStatus.Disabled;
            }
        }
        ResetMenuItemTween();
    }

    public void OnPointerHoverItem(Transform hoveredItem)
    {
        var itemRoot = hoveredItem.parent;
        var index = itemRoot.GetSiblingIndex();
        ActionMenuItem menuItem = actionMenuItems[index];    

        if(menuItem.status == ActionMenuItem.ActionMenuItemStatus.Inactive)
        {
            menuItem.status = ActionMenuItem.ActionMenuItemStatus.Hovered;
            menuItem.displayedSprite.sprite = menuItem.sprites[2];
            HighlightHoveredMenuItemTween();

            HapticsManager.Instance.OneShotVibration(HapticsManager.VibrationPreset.OnLaser);
        }
    }

    public void OnPointerClickItem(Transform clickedItem)
    {
        var itemRoot = clickedItem.parent;
        var index = itemRoot.GetSiblingIndex();
        ActionMenuItem menuItem = actionMenuItems[index];
        if(menuItem.status == ActionMenuItem.ActionMenuItemStatus.Disabled)
        {
            return;
        }
    
        if (currentActivateItem != null)
        {
            currentActivateItem.status = ActionMenuItem.ActionMenuItemStatus.Inactive;
            currentActivateItem.displayedSprite.sprite = currentActivateItem.sprites[1];
            currentActivateItem.OnItemDeselect.Invoke();
        }

        //New Clicked Item
        menuItem.status = ActionMenuItem.ActionMenuItemStatus.Active;
        currentActivateItem = menuItem;
        menuItem.displayedSprite.sprite = menuItem.sprites[3];
        menuItem.OnItemClick.Invoke();

        //Animation
        menuItem.displayedSprite.transform.DOShakeScale(.2f, new Vector3(0, 0.005f, 0));
        ResetMenuItemTween();

        //Haptic
        HapticsManager.Instance.OneShotVibration(HapticsManager.VibrationPreset.OnGrab);
    }

    public void OnPointerExitItem(Transform exitedItem)
    {
        var itemRoot = exitedItem.parent;
        var index = itemRoot.GetSiblingIndex();
        ActionMenuItem menuItem = actionMenuItems[index];

        //Exit an hovered item
        if(menuItem.status == ActionMenuItem.ActionMenuItemStatus.Hovered)
        {
            menuItem.status = ActionMenuItem.ActionMenuItemStatus.Inactive;
            menuItem.displayedSprite.sprite = menuItem.sprites[1];
            ResetMenuItemTween();
        }
    }

    public void HighlightHoveredMenuItemTween()
    {
        if (hoverTweenSequence.IsActive()) hoverTweenSequence.Kill();
        hoverTweenSequence = DOTween.Sequence();
        foreach(ActionMenuItem item in actionMenuItems)
        {
            if(item.status == ActionMenuItem.ActionMenuItemStatus.Hovered)
            {
                hoverTweenSequence.Join(item.displayedSprite.DOColor(itemHoveredSpriteColor, itemHoverTweenDuration)).SetEase(Ease.InOutSine);
            }
            if(item.status == ActionMenuItem.ActionMenuItemStatus.Inactive)
            {
                hoverTweenSequence.Join(item.displayedSprite.DOColor(itemNotHoveredSpriteColor, itemHoverTweenDuration)).SetEase(Ease.InOutSine);
            }
        }
    }

    public void ResetMenuItemTween()
    {
        if (hoverTweenSequence.IsActive()) hoverTweenSequence.Kill();
        hoverTweenSequence = DOTween.Sequence();
        foreach (ActionMenuItem item in actionMenuItems)
        {
            if(item.status == ActionMenuItem.ActionMenuItemStatus.Inactive)
            {
                var targetColor = currentActivateItem == null ? itemInactiveSpriteColor : itemNotHoveredSpriteColor;
                hoverTweenSequence.Join(item.displayedSprite.DOColor(targetColor, itemHoverTweenDuration)).SetEase(Ease.InOutSine);
            }
            if(item.status == ActionMenuItem.ActionMenuItemStatus.Hovered)
            {
                hoverTweenSequence.Join(item.displayedSprite.DOColor(itemHoveredSpriteColor, itemHoverTweenDuration)).SetEase(Ease.InOutSine);
            }
            if(item.status == ActionMenuItem.ActionMenuItemStatus.Active)
            {
                hoverTweenSequence.Join(item.displayedSprite.DOColor(itemHoveredSpriteColor, itemHoverTweenDuration)).SetEase(Ease.InOutSine);
            }
            if(item.status == ActionMenuItem.ActionMenuItemStatus.Disabled)
            {
                item.displayedSprite.color = itemDisabledSpriteColor;
            }
        }
    }

    private void DisplayData(Constituent constituent)
    {
        // If is unselect, do nothing
        if (constituent == null) {  return; }

        OnPointerClickItem(actionMenuItems[1].displayedSprite.transform);
    }

}


