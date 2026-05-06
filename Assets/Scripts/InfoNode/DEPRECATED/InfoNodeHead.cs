using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("InfoNodeHead is deprecated, please use InfoNodeHeadV2 instead.")]
public class InfoNodeHead : MonoBehaviour
{
    public ConstituentData.ConstituentType conType;
    public GameObject infoNodePanelPrefab;
    public InfoNodeData infoNodeData;
    [SerializeField] Transform infoChainStartLocation;
    List<GameObject> infoNodeList = new List<GameObject>();
    Quaternion startRotation;

    void Awake(){
        startRotation = transform.rotation;
    }

    void Start()
    {
        SelectedConstituentManager.Instance.OnConstituentSelect += OnConstituentSelect;
        WIAC_Manager.Instance.OnChangeNexus += OnChangeNexus;
        WIAC_Manager.Instance.OnToggleNexusTour += OnToggleNexusTour;
    }

    public void StartChain()
    {
        if(infoNodeList.Count == 0)
        {
            if (infoNodeData.nodeTextLayers.Count > 0)
            {
                GameObject newInfoNode = Instantiate(infoNodePanelPrefab);
                newInfoNode.transform.position = infoChainStartLocation.position;
                infoNodeList.Add(newInfoNode);
                infoNodeList[0].GetComponent<InfoNodePanel>().InitializeInfoNodePanel(infoNodeData.nodeTextLayers[0], this.transform, true, this); // TODO: jesus fucking christ this line.

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
        GameObject newInfoNode = Instantiate(infoNodePanelPrefab);
        newInfoNode.transform.position = spawnLocation.position;
        infoNodeList.Add(newInfoNode);

        if (infoNodeList.Count < infoNodeData.nodeTextLayers.Count)
        {
            infoNodeList[infoNodeList.Count - 1].GetComponent<InfoNodePanel>().InitializeInfoNodePanel(infoNodeData.nodeTextLayers[infoNodeList.Count - 1], infoNodeList[infoNodeList.Count - 2].GetComponent<InfoNodePanel>().bottomAnchor.transform, true, this);
        }
        else
        {
            infoNodeList[infoNodeList.Count - 1].GetComponent<InfoNodePanel>().InitializeInfoNodePanel(infoNodeData.nodeTextLayers[infoNodeList.Count - 1], infoNodeList[infoNodeList.Count - 2].GetComponent<InfoNodePanel>().bottomAnchor.transform, false, this);
        }

        infoNodeList[infoNodeList.Count - 2].GetComponent<InfoNodePanel>().OnNewPanelAdded();
        FMODUnity.RuntimeManager.PlayOneShot2D("event:/UI Events/text_popup");
    }

    //Called from Collapse Button gameobject, InfoNodeButtons script
    public void CollapseChain()
    {
        StartCoroutine(CollapseAnimation());
    }

    IEnumerator CollapseAnimation()
    {
        float collapseDuration = 0.25f;

        //PERFORM LERP ANIMATIONS (EXCLUDE FINAL PANEL) 
        for(int i = infoNodeList.Count -1; i > 0; i--)
        {
            FMODUnity.RuntimeManager.PlayOneShot2D("event:/UI Events/text_close");
            StartCoroutine(Lerp(infoNodeList[i], infoNodeList[i].transform.position, infoNodeList[i - 1].transform.position, collapseDuration));
            yield return new WaitForSeconds(collapseDuration);
            infoNodeList[i].SetActive(false);
        }

        //PERFORM LERP ON FINAL PANEL
        FMODUnity.RuntimeManager.PlayOneShot2D("event:/UI Events/text_close");
        StartCoroutine(Lerp(infoNodeList[0], infoNodeList[0].transform.position, this.transform.position, collapseDuration));
        yield return new WaitForSeconds(collapseDuration);
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
}
