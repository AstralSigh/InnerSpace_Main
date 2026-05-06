using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfoNodeSlot : MonoBehaviour
{
    public bool enableRotateToInfoNodeHead = false;

    [Header("Component Settings")]
    public RadialChild radialChild;
    public TextMeshProUGUI nodeIdxText;
    public MeshRenderer nodeSlotObject; 


    private InfoNodeHeadV2 _targetInfoNode;

    [SerializeField]
    private float START_ANG = 90f;
    [SerializeField]
    private float GAP_ANG = -25f;


    [SerializeField]
    private Color objectIdle;
    [SerializeField]
    private Color textIdle;
    [SerializeField]
    private Color objectHover;
    [SerializeField]
    private Color textHover;

    // Update is called once per frame
    void Update()
    {
        if (enableRotateToInfoNodeHead)  {
            RefreshRadialToHead();
        }
    }

    private void OnDestroy()
    {
        if (_targetInfoNode)
        {
            _targetInfoNode.laserButton.OnPointerEnterEvent.RemoveListener(ChangeHoverColor);
            _targetInfoNode.laserButton.OnPointerEnterEvent.RemoveListener(PreviewNode);
            _targetInfoNode.laserButton.OnPointerExitEvent.RemoveListener(ChangeNormalColor);
            _targetInfoNode.laserButton.OnPointerExitEvent.RemoveListener(CancelPreviewNode);
        }
    }
    public void initialize( InfoNodeHeadV2 targetInfoNode, int currentIdx)
    {
        // Update letter with nodeIndex
        _targetInfoNode = targetInfoNode;
        RefreshNodeText(currentIdx);
        if (enableRotateToInfoNodeHead)
        {
            RefreshRadialToHead();
        }
        else
        {
            RefreshRadialByIdx(currentIdx);
        }
        _targetInfoNode.laserButton.OnPointerEnterEvent.AddListener(ChangeHoverColor);
        _targetInfoNode.laserButton.OnPointerEnterEvent.AddListener(PreviewNode);
        _targetInfoNode.laserButton.OnPointerExitEvent.AddListener(ChangeNormalColor);
        _targetInfoNode.laserButton.OnPointerExitEvent.AddListener(CancelPreviewNode);
    }

    public void TeleportTo()
    {
        if (this._targetInfoNode)
        {
            this._targetInfoNode.TeleportTo();
        }
    }

    void RefreshNodeText(int currIdx)
    {
        if (nodeIdxText == null) { return; }
        nodeIdxText.text = enableRotateToInfoNodeHead ? "" : "" + (currIdx + 1);
    }

    void RefreshRadialToHead()
    {
        if (_targetInfoNode == null) { return; }
        if (radialChild == null) { return; }

        Vector3 dir = _targetInfoNode.transform.position - this.transform.parent.position;

        Vector3 dirP = dir - (Vector3.Dot(dir, this.transform.parent.forward) * this.transform.parent.forward);

        if (dirP == Vector3.zero) { return; }

        float angle = Vector3.Angle(dirP, this.transform.parent.right);

        if (Vector3.Dot(dirP, this.transform.parent.up) < 0)
        {
            angle = 360 - angle;
        }

        radialChild.fAngle = angle;

    }

    void RefreshRadialByIdx(int currIdx)
    {
        if (radialChild == null) { return; }

        radialChild.fAngle = START_ANG + currIdx * GAP_ANG;

    }

    public void ChangeHoverColor(PointerEventData data)
    {
        nodeSlotObject.material.SetColor("_BaseColor", objectHover);
        nodeIdxText.color = textHover;
    }
    public void ChangeNormalColor(PointerEventData data)
    {
        nodeSlotObject.material.SetColor("_BaseColor", objectIdle);
        nodeIdxText.color = textIdle;
    }

    public void PreviewNode(PointerEventData data)
    {
        if (_targetInfoNode == null) { return; }
        _targetInfoNode.EnablePreview();
        ActionMenuDataManager.instance.PreviewInfoNode(this._targetInfoNode.infoNodeData);
    }
    public void CancelPreviewNode(PointerEventData data)
    {
        if (_targetInfoNode == null) { return; }
        _targetInfoNode.DisablePreview();
        ActionMenuDataManager.instance.PreviewInfoNode(null);
    }
    public void OnPointerClick()
    {
        TeleportTo();
    }
}
