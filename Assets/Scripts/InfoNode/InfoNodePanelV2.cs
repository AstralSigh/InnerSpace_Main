using UnityEngine;
using UnityEngine.UI;
using BNG;
using TMPro;
using System;
using UnityEngine.XR.OpenXR.Features;

public class InfoNodePanelV2 : MonoBehaviour
{
    private InfoNodeHeadV2 infoNodeHead;
    [Tooltip("if false, panel will use the expand button to manage expand or hide text")]
    public bool autoExpandtext = true;

    [Header("UI Settings")]
    [SerializeField]
    private RectTransform _bgBlur;
    [SerializeField]
    private RectTransform _tabBasic;
    [SerializeField]
    private RectTransform _tabBasicShowing;
    [SerializeField]
    private RectTransform _tabBasicHiding;
    // TODO: We areusing Colliders to detect laser interactions and recaclulating anytime an UI element changes size, but We may be able to remove them and interact with UI Elements directly.
    [SerializeField]
    private BoxCollider _boxColBasic;
    [SerializeField]
    private RectTransform _tabIntermediate;
    [SerializeField]
    private RectTransform _tabIntermediateShowing;
    [SerializeField]
    private RectTransform _tabIntermediateHiding;
    [SerializeField]
    private BoxCollider _boxColIntermediate;
    [SerializeField]
    private RectTransform _tabAdvanced;
    [SerializeField]
    private RectTransform _tabAdvancedShowing;
    [SerializeField]
    private RectTransform _tabAdvancedHiding;
    [SerializeField]
    private BoxCollider _boxColAdvanced;
    [SerializeField]
    private Text _textTitle;
    [SerializeField]
    private TextMeshProUGUI _textDetail;
    [SerializeField]
    private GameObject _btnAdd;
    [SerializeField]
    private RectTransform _btnCloseAnchor;
    [SerializeField]
    private RectTransform _btnExpandAnchor;
    [SerializeField]
    private RectTransform _btnExpandIconToExpand;
    [SerializeField]
    private RectTransform _btnExpandIconToCollapse;
    [SerializeField]
    private LineToTransform _lineToParent;
    [SerializeField]
    private LineToTransform _lineToTarget;
    public GameObject topAnchor;
    public GameObject leftAnchor;
    public GameObject rightAnchor;
    public GameObject bottomAnchor;
    public GameObject centerAnchor;

    private InfoNodeDataV2.NodeTextData _data;

    private TabClass _showingTab = TabClass.Basic;

    private float _textDetailOrgHeight = 220;
    private float _textDetailExpHeight = 220;
    private float _bgBlurOrgHeight = 540;
    private bool _textDetailCollapsed = true;

    [Serializable]
    public enum TabClass : int
    {
        Basic = 0,
        Intermediate = 1,
        Advanced = 2
    }

    [Serializable]
    public enum AnchorSide : int
    {
        Top = 0,
        Bottom = 1,
        Left = 2,
        Right = 3
    }

    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        // Reset Box size
        if (this._boxColBasic) { this._boxColBasic.size = this._tabBasicHiding.rect.size; }
        if (this._boxColIntermediate) { this._boxColIntermediate.size = this._tabIntermediateHiding.rect.size; }
        if (this._boxColAdvanced) { this._boxColAdvanced.size = this._tabAdvancedHiding.rect.size; }
        // Get Expand info 
        if (this._bgBlur) { this._bgBlurOrgHeight = this._bgBlur.rect.height; }
        if (this._btnExpandAnchor && autoExpandtext)
        {
            this._btnExpandAnchor.gameObject.SetActive(false); 
        }
    }

    public void InitializeInfoNodePanel( InfoNodeDataV2.NodeTextData data, Transform parentAnchor, bool hasAddButton, InfoNodeHeadV2 infoNodeHead)
    {
        this._data = data;
        // Set title
        if (this._textTitle) { this._textTitle.text = this._data.title; }
        // Check all tabs required
        this.InitializeTabs();
        // Set Text
        if (this._textDetail)
        {
            this._textDetailOrgHeight = this._textDetail.textBounds.size.y;
            this._textDetailOrgHeight = this._textDetail.rectTransform.rect.height;
        }
        this.RefreshTextDetail();
        // Set line to the parent node
        if (this._lineToParent) { this._lineToParent.ConnectTo = parentAnchor; }
        // Set Add Button
        if (this._btnAdd) { this._btnAdd.SetActive(hasAddButton); }
        // Set InfoNodeHead Ref
        this.infoNodeHead = infoNodeHead;
    }

    /// <summary>
    /// Based on data, decide which tabs should be shown
    /// </summary>
    private void InitializeTabs()
    {
        bool hasTabShowed = false;
        if (!string.IsNullOrEmpty(this._data.basic))
        {
            if (this._tabBasic) { this._tabBasic.gameObject.SetActive(true); }
            if (this._tabBasicShowing) { this._tabBasicShowing.gameObject.SetActive(true); }
            if (this._tabBasicHiding) { this._tabBasicHiding.gameObject.SetActive(false); }
            _showingTab = TabClass.Basic;
            hasTabShowed = true;
        }
        else
        {
            if (this._tabBasic) { this._tabBasic.gameObject.SetActive(false); }
        }
        if (!string.IsNullOrEmpty(this._data.intermediate))
        {
            if (this._tabIntermediate) { this._tabIntermediate.gameObject.SetActive(true); }
            if (this._tabIntermediateShowing) { this._tabIntermediateShowing.gameObject.SetActive(hasTabShowed ? false : true); }
            if (this._tabIntermediateHiding) { this._tabIntermediateHiding.gameObject.SetActive(hasTabShowed ? true : false); }
            if (!hasTabShowed) { _showingTab = TabClass.Intermediate; }
            hasTabShowed = true;
        }
        else
        {
            if (this._tabIntermediate) { this._tabIntermediate.gameObject.SetActive(false); }
        }
        if (!string.IsNullOrEmpty(this._data.advanced))
        {
            if (this._tabAdvanced) { this._tabAdvanced.gameObject.SetActive(true); }
            if (this._tabAdvancedShowing) { this._tabAdvancedShowing.gameObject.SetActive(hasTabShowed ? false : true); }
            if (this._tabAdvancedHiding) { this._tabAdvancedHiding.gameObject.SetActive(hasTabShowed ? true : false); }
            if (!hasTabShowed) { _showingTab = TabClass.Advanced; }
            hasTabShowed = true;
        }
        else
        {
            if (this._tabAdvanced) { this._tabAdvanced.gameObject.SetActive(false); }
        }

        Canvas.ForceUpdateCanvases();
        // Reset Box size
        if (this._boxColBasic) { this._boxColBasic.size = this._tabBasicHiding.rect.size; }
        if (this._boxColIntermediate) { this._boxColIntermediate.size = this._tabIntermediateHiding.rect.size; }
        if (this._boxColAdvanced) { this._boxColAdvanced.size = this._tabAdvancedHiding.rect.size; }
    }

    /// <summary>
    /// Based on showing tab, refresh the text in detail
    /// </summary>
    private void RefreshTextDetail()
    {
        if (!this._textDetail) { return; }
        switch (this._showingTab)
        {
            case TabClass.Basic:
                this._textDetail.text = this._data.basic;
                break;
            case TabClass.Intermediate:
                this._textDetail.text = this._data.intermediate;
                break;
            case TabClass.Advanced:
                this._textDetail.text = this._data.advanced;
                break;
        }

        if (!this._textDetail) {
            return; 
        }
        this._textDetail.overflowMode = TextOverflowModes.Overflow;
        this._textDetail.ForceMeshUpdate();
        this._textDetailExpHeight = this._textDetail.textBounds.size.y;
        bool textExpandAvailable = this._textDetailExpHeight > this._textDetailOrgHeight;

        if (autoExpandtext)
        {
            if (this._bgBlur) { this._bgBlur.sizeDelta = new Vector2(this._bgBlur.sizeDelta.x, textExpandAvailable ? (this._bgBlurOrgHeight + this._textDetailExpHeight - this._textDetailOrgHeight) : this._bgBlurOrgHeight); }
        }
        else
        {
            this._textDetail.overflowMode = TextOverflowModes.Ellipsis;
            this._textDetail.ForceMeshUpdate();
            if (this._btnExpandAnchor) { this._btnExpandAnchor.gameObject.SetActive(textExpandAvailable); }
            if (this._btnExpandIconToExpand) { this._btnExpandIconToExpand.gameObject.SetActive(true); }
            if (this._btnExpandIconToCollapse) { this._btnExpandIconToCollapse.gameObject.SetActive(false); }
        }
    }



    // Combine functions in InfoNodeButtons to here
    public void CollapseChain()
    {
        if (this.infoNodeHead)
        {
            this.infoNodeHead.CollapseChain();
        }
    }
    public void AddNextNode(Transform spawnLocation)
    {
        if (this._btnAdd) { this._btnAdd.gameObject.SetActive(false); }
        if (this._btnCloseAnchor) { this._btnCloseAnchor.gameObject.SetActive(false); }
        if (this.infoNodeHead) { this.infoNodeHead.AddToChain(spawnLocation); }
        PointerManager.Instance.DisablePointer();
    }

    public void ExpandBtnClicked()
    {
        //expand text
        if (this._bgBlur) { this._bgBlur.sizeDelta = new Vector2(this._bgBlur.sizeDelta.x, this._textDetailCollapsed ? (this._bgBlurOrgHeight + this._textDetailExpHeight - this._textDetailOrgHeight) : this._bgBlurOrgHeight); }
        //if (this._bgBlur) { this._bgBlur.rect.Set(this._bgBlur.rect.x, this._bgBlur.rect.y, this._bgBlur.rect.width, this._textDetailCollapsed ? (this._bgBlurOrgHeight + this._textDetailExpHeight - this._textDetailOrgHeight) : this._bgBlurOrgHeight); }
        Canvas.ForceUpdateCanvases();
        if (this._btnExpandIconToExpand) { this._btnExpandIconToExpand.gameObject.SetActive(!this._textDetailCollapsed); }
        if (this._btnExpandIconToCollapse) { this._btnExpandIconToCollapse.gameObject.SetActive(this._textDetailCollapsed); }
        this._textDetailCollapsed = !this._textDetailCollapsed;

    }


    public void OnTabClicked(int tabNum)
    {
        this._showingTab = (TabClass) tabNum;
        this.SwitchTab();
    }

/// <summary>
/// When tab is changed among Basic, Intermediate and Advanced, The activity state of the objects will be recalculated.
/// </summary>
    private void SwitchTab()
    {
        if (this._tabBasicShowing) { this._tabBasicShowing.gameObject.SetActive(this._showingTab == TabClass.Basic); }
        if (this._tabBasicHiding) { this._tabBasicHiding.gameObject.SetActive(this._showingTab != TabClass.Basic); }
        if (this._tabIntermediateShowing) { this._tabIntermediateShowing.gameObject.SetActive(this._showingTab == TabClass.Intermediate); }
        if (this._tabIntermediateHiding) { this._tabIntermediateHiding.gameObject.SetActive(this._showingTab != TabClass.Intermediate); }
        if (this._tabAdvancedShowing) { this._tabAdvancedShowing.gameObject.SetActive(this._showingTab == TabClass.Advanced); }
        if (this._tabAdvancedHiding) { this._tabAdvancedHiding.gameObject.SetActive(this._showingTab != TabClass.Advanced); }

        //expand text
        if (this._bgBlur) { this._bgBlur.sizeDelta = new Vector2(this._bgBlur.sizeDelta.x, this._bgBlurOrgHeight); }
        this._textDetailCollapsed = true;
        this.RefreshTextDetail();
    }

    public void SwitchToParentLineSide(AnchorSide side)
    {
        if (!_lineToParent) { return; }
        var anchor = side switch
        {
            AnchorSide.Top => topAnchor,
            AnchorSide.Bottom => bottomAnchor,
            AnchorSide.Left => leftAnchor,
            AnchorSide.Right => rightAnchor,
            _ => throw new NotImplementedException("Unexpected AnchorSide? " + side)
        };
        _lineToParent.transform.SetParent(anchor.transform);
        _lineToParent.transform.localPosition = Vector3.zero;
    }

    public void SetLineTarget(Transform tarTran)
    {
        // Set line to target
        if (this._lineToTarget) { this._lineToTarget.ConnectTo = tarTran; }
    }
}
