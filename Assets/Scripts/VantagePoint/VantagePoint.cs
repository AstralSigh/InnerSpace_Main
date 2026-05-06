using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class VantagePoint : MonoBehaviour
{
    public int vantagePointIndex;
    [SerializeField] TextMeshProUGUI text;

    private void Start()
    {
        WIAC_Manager.Instance.OnToggleNexusTour += ToggleTour;
    }

    void ToggleTour(bool nexusTourActive)
    {
        if (nexusTourActive)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            this.gameObject.SetActive(true);
        }
    }


    public void DeleteVantagePoint()
    {
        VantagePointManager.Instance.DeleteVantagePoint(vantagePointIndex);
    }

    public void SetText(string t)
    {
        text.text = t;
    }
}
