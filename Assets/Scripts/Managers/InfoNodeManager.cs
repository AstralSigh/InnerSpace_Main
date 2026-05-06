using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoNodeManager : MonoBehaviour
{
    public static InfoNodeManager Instance { get; private set; }

    private List<InfoNodeHeadV2> _curNexusInfoNodesList = new List<InfoNodeHeadV2>();
    private List<InfoNodeHeadV2> _currConsInfoNodesList = new List<InfoNodeHeadV2>();

    public delegate void CurrConsInfoNodesChangeEvent(List<InfoNodeHeadV2> nodes);
    public event CurrConsInfoNodesChangeEvent OnChangeCurrConsInfoNodes;
    public List<InfoNodeHeadV2> CurrConsInfoNodesList
    {
        get { return this._currConsInfoNodesList; }
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        WIAC_Manager.Instance.OnChangeNexus += OnChangeNexus;
        WIAC_Manager.Instance.OnToggleNexusTour += OnToggleNexusTour;
        SelectedConstituentManager.Instance.OnConstituentSelect += OnConstituentSelect;
    }

    private void OnDestroy()
    {
        WIAC_Manager.Instance.OnChangeNexus -= OnChangeNexus;
        WIAC_Manager.Instance.OnToggleNexusTour -= OnToggleNexusTour;
        SelectedConstituentManager.Instance.OnConstituentSelect -= OnConstituentSelect;
    }

    public void OnChangeNexus(Nexus_Data.eNexusType currentNexus)
    {
        _curNexusInfoNodesList.Clear();
        WIAC_Manager.Instance.currentNexusManager.GetComponentsInChildren(true, _curNexusInfoNodesList);
        OnConstituentSelect(null);
    }


    public void OnConstituentSelect(Constituent constituent)
    {
        if (_isNexusTourActive) {
            return;
        }
        _currConsInfoNodesList.Clear();

        foreach (InfoNodeHeadV2 infoNode in _curNexusInfoNodesList)
        {
            infoNode.gameObject.SetActive(infoNode.alwaysActive || (constituent != null && infoNode.HasConType(constituent.GetData().eConType)));
            if (infoNode.gameObject.activeSelf) { _currConsInfoNodesList.Add(infoNode); }
        }

        _currConsInfoNodesList.Sort(
            delegate (InfoNodeHeadV2 a, InfoNodeHeadV2 b) {
                return a.panelPresentHierarchy.CompareTo(b.panelPresentHierarchy);
            });

        if (OnChangeCurrConsInfoNodes!= null)
        {
            OnChangeCurrConsInfoNodes.Invoke(_currConsInfoNodesList); 
        }
    }

    // When we toggle nexus tour, disable info nodes.
    private bool _isNexusTourActive;
    private void OnToggleNexusTour(bool nexusTourActive)
    {
        _isNexusTourActive = nexusTourActive;
        foreach (InfoNodeHeadV2 infoNode in _curNexusInfoNodesList)
        {
            infoNode.gameObject.SetActive(false);
        }
    }

    public List<InfoNodeHeadV2> GetInfoNodesByCons(ConstituentData cons)
    {
        List<InfoNodeHeadV2> nodes = new List<InfoNodeHeadV2>();
        foreach (var n in _curNexusInfoNodesList)
        {
            if (n.HasConType(cons.eConType)) { nodes.Add(n); }
        }
        return nodes;
    }

}
