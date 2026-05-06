using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenu_NexusMapDescription : MonoBehaviour
{
    [SerializeField] private Transform titleRoot;
    [SerializeField] private Transform bodyRoot;
    
    [SerializeField] private Text nexusTitle;
    [SerializeField] private Text nexusBody;
    bool hoveringOn;

    [SerializeField] private List<GameObject> MapLines;
    [SerializeField] private List<Transform> AnchorPoints;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        StartCoroutine(RevealName());
        StartCoroutine(RevealDescription());
    }

    private void OnDisable()
    {
        ClearMapDescription();
        StopAllCoroutines();
    }

    public void NexusLineOnHover(Nexus_Manager localNexManager)
    {
        gameObject.SetActive(true);

        nexusTitle.text = localNexManager.nexusData.nexusName;
        nexusBody.text = localNexManager.nexusData.nexusDescription;

        switch (localNexManager.nexusData.nexusType)
        {
            case Nexus_Data.eNexusType.GCluster:
                MapLines[0].SetActive(true);
                gameObject.transform.position = AnchorPoints[0].position;
                break;

            case Nexus_Data.eNexusType.ACConversion:
                MapLines[4].SetActive(true);
                gameObject.transform.position = AnchorPoints[4].position;
                break;

            case Nexus_Data.eNexusType.PKARelay:
                MapLines[5].SetActive(true);
                gameObject.transform.position = AnchorPoints[5].position;
                break;

            case Nexus_Data.eNexusType.RibosomeTranslation:
                MapLines[6].SetActive(true);
                gameObject.transform.position = AnchorPoints[6].position;
                break;

            case Nexus_Data.eNexusType.Golgi:
                //temporarily using this for GLUT nexus until enums for NexusType are updated
                MapLines[3].SetActive(true);
                gameObject.transform.position = AnchorPoints[3].position;

                break;

            case Nexus_Data.eNexusType.ImmatureCrystal:
                MapLines[1].SetActive(true);
                gameObject.transform.position = AnchorPoints[1].position;

                break;

            case Nexus_Data.eNexusType.InsulinRelease:
                MapLines[2].SetActive(true);
                gameObject.transform.position = AnchorPoints[2].position;
                #if UNITY_ANDROID
                nexusTitle.text += " (Under Construction)";
                #endif
                break;

           
        }
    }


    public void ClearMapDescription()
    {
        titleRoot.localScale = new Vector3(1, 0, 1);
        bodyRoot.localScale = new Vector3(1, 0, 1);
        foreach(GameObject line in MapLines)
        {
            line.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    IEnumerator RevealName()
    {
        yield return new WaitForSeconds(0.25f);
        iTween.ScaleTo(titleRoot.gameObject, iTween.Hash("scale", Vector3.one, "time", 0.5f, "easetype", iTween.EaseType.easeInOutCubic));
    }

    IEnumerator RevealDescription()
    {
        yield return new WaitForSeconds(1.0f);
        iTween.ScaleTo(bodyRoot.gameObject, iTween.Hash("scale", Vector3.one, "time", 0.5f, "easetype", iTween.EaseType.easeInOutCubic));

    }

   
}
