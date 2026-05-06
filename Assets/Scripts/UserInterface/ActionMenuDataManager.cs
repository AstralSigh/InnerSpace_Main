using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class ActionMenuDataManager : MonoBehaviour
{
    private static ActionMenuDataManager _instance;
    public static ActionMenuDataManager instance { get { return _instance; } }

    private ConstituentData data;

    [SerializeField] RectTransform infoNodeContainer;

    [SerializeField] GameObject constituentInfomation;
    [SerializeField] TextMeshProUGUI headerCons;
    [SerializeField] TextMeshProUGUI subheaderCons;

    [SerializeField] GameObject infoNodePreview;
    [SerializeField] TextMeshProUGUI headerInfoNode;

    [SerializeField] private List<GameObject> nodeSlots;

    [SerializeField] private GameObject nodeSlotPrefab;

    public bool notShowNodesAlwaysActive = true;

    private static string TO_SELECTED_TEXT = "Select a Constituent";

    private void Awake()
    {
        _instance = this;
    }
    void Start()
    {
        SelectedConstituentManager.Instance.OnConstituentSelect += AssignConstituent;
        InfoNodeManager.Instance.OnChangeCurrConsInfoNodes += RefreshNodes;
        headerCons.text = TO_SELECTED_TEXT;
        subheaderCons.text ="";
        constituentInfomation.SetActive(true);
        infoNodePreview.SetActive(false);
    }
    private void OnDestroy()
    {
        SelectedConstituentManager.Instance.OnConstituentSelect -= AssignConstituent;
        InfoNodeManager.Instance.OnChangeCurrConsInfoNodes -= RefreshNodes;
    }


    public void AssignConstituent(Constituent constituent)
    {
        bool unSelected = constituent == null;
        data = unSelected ? null : constituent.GetData();
        headerCons.text = unSelected ? TO_SELECTED_TEXT : data.conHeader;
        subheaderCons.text = unSelected ? "" : data.conSubHeader;
        constituentInfomation.SetActive(true);
        infoNodePreview.SetActive(false) ;
    }

    private void RefreshNodes(List<InfoNodeHeadV2> nodes)
    {
        SetUpNodeSlots();
    }


    public void SetUpNodeSlots()
    {
        if (nodeSlots != null)
        {
            foreach (GameObject ns in nodeSlots)
            {
                Destroy(ns);
            }
            nodeSlots.Clear();
        }
        nodeSlots = new List<GameObject>();

        int count = InfoNodeManager.Instance.CurrConsInfoNodesList.Count;

        for (int x = 0; x < count; x++)
        {
            InfoNodeHeadV2 targetInfoNode = InfoNodeManager.Instance.CurrConsInfoNodesList[x];
            if (notShowNodesAlwaysActive && targetInfoNode.alwaysActive) { continue; }

            GameObject nodeSlot = Instantiate(nodeSlotPrefab, Vector3.zero, Quaternion.identity);
            nodeSlot.transform.SetParent(infoNodeContainer, false);
            nodeSlots.Add(nodeSlot);
            InfoNodeSlot slot = nodeSlot.transform.GetComponent<InfoNodeSlot>();

            if (slot) slot.initialize(targetInfoNode, x);
            nodeSlot.name = "Node Slot " + x;
            nodeSlot.SetActive(true);
        }
    }

    public void PreviewInfoNode(InfoNodeDataV2 data)
    {
        if (data == null)
        {
            constituentInfomation.SetActive(true);
            infoNodePreview.SetActive(false);
        }
        else
        {
            constituentInfomation.SetActive(false);
            infoNodePreview.SetActive(true);
            headerInfoNode.text = data.nodeName;
            
        }
    }
}
