using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NexusTourManager : MonoBehaviour
{
    [SerializeField] GameObject nexusTourContent;

    // Start is called before the first frame update
    void Start()
    {
        WIAC_Manager.Instance.OnToggleNexusTour += ShowHideContent;
    }

    void ShowHideContent(bool nexusTourActive)
    {
        //Debug.Log("Nexus tour content visibile: " + nexusTourActive);
        nexusTourContent.SetActive(nexusTourActive);
    }
}