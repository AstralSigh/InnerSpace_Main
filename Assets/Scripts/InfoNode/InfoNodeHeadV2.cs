using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class InfoNodeHeadV2 : MonoBehaviour
{
    public InfoNodeType nodeType = InfoNodeType.Structure;
    public bool typeOverrideByData = false;
    public GameObject infoNodePanelPrefab;
    public InfoNodeDataV2 infoNodeData;
    public bool alwaysActive = false;
    public bool setTimer = false;
    [Range(0f, 1f)]
    public float setTimerTo = 0f;
    public float teleportPosGap = 1f;
    public float panelPresentHierarchy = 0f;
    // TODO: Can use Custom Editor script to make this set up much better
    [Tooltip("To make the 'n''th node panel link to [GameObjectA], set 'n'th element in this list to [GameObjectA]")]
    public List<GameObject> nodeLinkTargetList = new List<GameObject>();

    List<GameObject> infoNodeList = new List<GameObject>();

    Quaternion startRotation;
    [Space(5)]
    [Header("UI Settings")]
    [SerializeField] Transform _infoChainStartLocation;
    [SerializeField] GameObject _previewGO;
    [SerializeField] Image _previewIcon;
    [SerializeField] Text _previewTypeText;
    [SerializeField] Text _previewTitleText;
    [SerializeField] LaserButton _laserButton;
    public LaserButton laserButton { get{return _laserButton;} }
    [SerializeField] Camera _previewCamera;
    [Space(5)]
    [Header("Sprite Settings")]
    public Image headImg;
    public Sprite headNormalStructure;
    public Sprite headHoverStructure;
    public Sprite headOpenStructure;
    public Sprite headNormalInteraction;
    public Sprite headHoverInteraction;
    public Sprite headOpenInteraction;
    public Sprite headNormalFunction;
    public Sprite headHoverFunction;
    public Sprite headOpenFunction;

    public Sprite previewIconStructure;
    public Sprite previewIconInteraction;
    public Sprite previewIconFunction;

    public Image borderImg;
    public Sprite borderUnViewed;
    public Sprite borderViewed;

    public Sprite headNormal
    {
        get
        {
            switch (nodeType)
            {
                case InfoNodeType.Structure:
                    return headNormalStructure;
                case InfoNodeType.Interaction:
                    return headNormalInteraction;
                case InfoNodeType.Function:
                    return headNormalFunction;
                default:
                    return headNormalStructure;
            }
        }
    }

    public Sprite headHover
    {
        get
        {
            switch (nodeType)
            {
                case InfoNodeType.Structure:
                    return headHoverStructure;
                case InfoNodeType.Interaction:
                    return headHoverInteraction;
                case InfoNodeType.Function:
                    return headHoverFunction;
                default:
                    return headHoverStructure;
            }
        }
    }

    public Sprite headOpen
    {
        get
        {
            switch (nodeType)
            {
                case InfoNodeType.Structure:
                    return headOpenStructure;
                case InfoNodeType.Interaction:
                    return headOpenInteraction;
                case InfoNodeType.Function:
                    return headOpenFunction;
                default:
                    return headOpenStructure;
            }
        }
    }

    public Sprite previewIcon
    {
        get
        {
            switch (nodeType)
            {
                case InfoNodeType.Structure:
                    return previewIconStructure;
                case InfoNodeType.Interaction:
                    return previewIconInteraction;
                case InfoNodeType.Function:
                    return previewIconFunction;
                default:
                    return previewIconStructure;
            }
        }
    }

    public string nodeTypeString { 
        get
        {
            switch (nodeType)
            {
                case InfoNodeType.Structure:
                    return "Structure";
                case InfoNodeType.Function:
                    return "Function";
                case InfoNodeType.Interaction:
                    return "Interaction";
                default:
                    return "Structure";
            }
         }
    }

    private bool _isviewing = false;


    void Awake(){
        startRotation = transform.rotation;
    }

    private void OnEnable()
    {
        this.ManagerEventsBind();
    }

    void Start()
    {
        this.ManagerEventsBind();
        // UI Set Up
        if (this.headImg)
        {
            this.headImg.sprite = this.headNormal;
        }
        if (this.borderImg)
        {
            this.borderImg.sprite = this.borderUnViewed;
        }
        if (this._previewGO)
        {
            this._previewGO.SetActive(false);
        }
        if (this.infoNodeData)
        {
            if (typeOverrideByData)
            {
                nodeType = infoNodeData.nodeType;
            }
            if (this._previewIcon)
            {
                this._previewIcon.sprite = previewIcon;
            }
            if (this._previewTypeText)
            {
                this._previewTypeText.text = nodeTypeString;
            }
            if (this._previewTitleText)
            {
                this._previewTitleText.text = this.infoNodeData.nodeName;
            }
        }
        if (this._previewCamera)
        {
            this._previewCamera.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        this.ManagerEventsUnBind();
    }

    private void ManagerEventsBind()
    {
        // TODO: Currently doing this cause we only have one main scene and is not "creating" and "destroying" nodes.
        // So I am using OnEnable and OnDisable to do this and needs to check null
        // But all this can be moved to Start and OnDestroy and simplized in the future if we split levels to other scenes.
        if (SelectedConstituentManager.Instance)
        {
            SelectedConstituentManager.Instance.OnConstituentSelect -= OnConstituentSelect;
            SelectedConstituentManager.Instance.OnConstituentSelect += OnConstituentSelect;
        }
        if (WIAC_Manager.Instance)
        {
            WIAC_Manager.Instance.OnChangeNexus -= OnChangeNexus;
            WIAC_Manager.Instance.OnChangeNexus += OnChangeNexus;
        }
        if (WIAC_Manager.Instance)
        {
            WIAC_Manager.Instance.OnToggleNexusTour -= OnToggleNexusTour;
            WIAC_Manager.Instance.OnToggleNexusTour += OnToggleNexusTour;
        }
    }
    private void ManagerEventsUnBind()
    {
        SelectedConstituentManager.Instance.OnConstituentSelect -= OnConstituentSelect;
        WIAC_Manager.Instance.OnChangeNexus -= OnChangeNexus;
        WIAC_Manager.Instance.OnToggleNexusTour -= OnToggleNexusTour;
    }

    public void StartChain()
    {
        this._isviewing = true;
        if (this._previewGO)
        {
            this._previewGO.SetActive(false);
        }
        if (infoNodeList.Count == 0)
        {
            if (infoNodeData.nodeTextLayers.Count > 0)
            {
                GameObject newInfoNode = Instantiate(infoNodePanelPrefab);
                newInfoNode.transform.position = _infoChainStartLocation.position;
                infoNodeList.Add(newInfoNode);
                InfoNodePanelV2 nodePanel = newInfoNode.GetComponent<InfoNodePanelV2>();
                var textDate = infoNodeData.nodeTextLayers[0];
                bool hasAdd = infoNodeData.nodeTextLayers.Count > 1;
                nodePanel.InitializeInfoNodePanel(textDate, this.transform, hasAdd, this); 

                nodePanel.SwitchToParentLineSide(InfoNodePanelV2.AnchorSide.Top);
                if (nodeLinkTargetList.Count > 0 && nodeLinkTargetList[0] != null)
                {
                    nodePanel.SetLineTarget(nodeLinkTargetList[0].transform);
                }

                FMODUnity.RuntimeManager.PlayOneShot2D("event:/UI Events/text_popup"); // TODO: These kind of string FMOD calls are fragile.
            }
            else
            {
                Debug.Log("InfoNodeData was not filled out");
            }
        }
        else
        {
            Debug.Log("InfoNodes were not cleared out. Cannot start new chain");
        }
    }

    public void AddToChain(Transform spawnLocation) 
    {
        if (infoNodeList.Count < 1) { return; }

        GameObject newInfoNode = Instantiate(infoNodePanelPrefab);
        newInfoNode.transform.position = spawnLocation.position;
        infoNodeList.Add(newInfoNode);
        int nodeIdx = infoNodeList.Count - 1;

        InfoNodePanelV2 nodePanel = newInfoNode.GetComponent<InfoNodePanelV2>();
        InfoNodePanelV2 lastPanel = infoNodeList[nodeIdx-1].GetComponent<InfoNodePanelV2>();
        InfoNodeDataV2.NodeTextData data = infoNodeData.nodeTextLayers[infoNodeList.Count - 1];
        bool hasAddBtn = infoNodeList.Count < infoNodeData.nodeTextLayers.Count;

        nodePanel.InitializeInfoNodePanel(data, lastPanel.rightAnchor.transform, hasAddBtn, this);

        nodePanel.SwitchToParentLineSide(InfoNodePanelV2.AnchorSide.Left);
        if (nodeLinkTargetList.Count > nodeIdx && nodeLinkTargetList[nodeIdx] != null)
        {
            nodePanel.SetLineTarget(nodeLinkTargetList[nodeIdx].transform);
        }

        FMODUnity.RuntimeManager.PlayOneShot2D("event:/UI Events/text_popup");
    }

    //Called from Collapse Button gameobject, InfoNodeButtons script
    public void CollapseChain()
    {
        this._isviewing = false;
        StartCoroutine(CollapseAnimation());
    }

    IEnumerator CollapseAnimation()
    {
        float collapseDuration = 0.25f;

        //PERFORM LERP ANIMATIONS (EXCLUDE FINAL PANEL) 
        for(int i = infoNodeList.Count -1; i > 0; i--)
        {
            FMODUnity.RuntimeManager.PlayOneShot2D("event:/UI Events/text_close");
            yield return StartCoroutine(Lerp(infoNodeList[i], infoNodeList[i].transform.position, infoNodeList[i - 1].transform.position, collapseDuration));
            infoNodeList[i].SetActive(false);
        }

        //PERFORM LERP ON FINAL PANEL
        FMODUnity.RuntimeManager.PlayOneShot2D("event:/UI Events/text_close");
        yield return StartCoroutine(Lerp(infoNodeList[0], infoNodeList[0].transform.position, this.transform.position, collapseDuration));
        infoNodeList[0].SetActive(false);
        
        foreach (GameObject panel in infoNodeList)
        {
            Destroy(panel);
        }
        infoNodeList.Clear();
    }

    IEnumerator Lerp(GameObject lerpedObject, Vector3 start, Vector3 end, float duration)
    {
        float time = 0;
        while(time < duration)
        {
            time += Time.deltaTime;
            lerpedObject.transform.localScale = Vector3.Lerp(new Vector3(1,1,1), new Vector3(0.01f, 0.01f, 0.01f), Mathf.Sin((time / duration) * (Mathf.PI) / 2f));
            lerpedObject.transform.position = Vector3.Lerp(start, end, Mathf.Sin( (time / duration) * (Mathf.PI)/2));
            yield return new WaitForEndOfFrame();
        }
    }

    public Quaternion GetStartRotation(){
        return startRotation;
    }

    public void OnConstituentSelect(Constituent constituent)
    {
        DestroyPanels();
    }

    public void OnChangeNexus(Nexus_Data.eNexusType currentNexus)
    {
        DestroyPanels();
    }

    public void OnToggleNexusTour(bool nexusTourActive)
    {
        if(nexusTourActive)
        {
            DestroyPanels();
        }
    }

    private void DestroyPanels()
    {
        foreach(GameObject infoNodePanel in infoNodeList)
        {
            Destroy(infoNodePanel);
        }

        infoNodeList = new List<GameObject>();
    }

    public bool HasConType(ConstituentData.ConstituentType type)
    {
        if (!this.infoNodeData)
        {
            return false;
        }
        return this.infoNodeData.nodeConstituents.Contains(type);
    }

    #region UI Related
    public void SetHover(bool hovering)
    {
        if (this.headImg)
        {
            this.headImg.sprite = hovering ? this.headHover : (_isviewing ? this.headOpen : this.headNormal);
        }
        if (this._previewGO)
        {
            this._previewGO.SetActive(hovering && !_isviewing);
        }
    }
    public void SetViewed()
    {
        if (this.borderImg)
        {
            this.borderImg.sprite = this.borderViewed;
        }
    }

    public void OnClicked()
    {
        if (_isviewing)
        {
            this.CollapseChain();
        }
        else
        {
            this.StartChain();
            TeleportTo();
            if (setTimer) {
                setTimerTo = Mathf.Clamp(setTimerTo, 0, 1);
                TimeManager.Instance.SetTime(setTimerTo * TimeManager.Instance.GetCurrentDuration()); 
            }
        }
    }

    /// <summary>
    /// Teleport is called from OnClicked(), we are not using LaserButtons 'TeleportToButton' because the logic for end position is not good. //TODO: REMOVE 'TeleportToButton' from laser button
    /// </summary>
    public void TeleportTo()
    {
        Vector3 teleportDest = this.transform.position - ((this.GetStartRotation() * Vector3.forward).normalized * teleportPosGap);
        PlayerManager.Instance.TeleportPlayer(teleportDest, this.GetStartRotation());
    }

    public void EnablePreview()
    {
        if (this._previewCamera)
        {
            this._previewCamera.gameObject.SetActive(true);
        }
    }

    public void DisablePreview()
    {
        if (this._previewCamera)
        {
            this._previewCamera.gameObject.SetActive(false);
        }
    }
    #endregion
}
