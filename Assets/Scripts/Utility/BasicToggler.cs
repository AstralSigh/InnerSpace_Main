using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BasicToggler : MonoBehaviour
{
    public List<ToggleItem> ToggleList;
    public int toggleIndex;
    public Text toggleNameLabel;
    public Text toggleNumberLabel;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current[Key.Digit9].wasPressedThisFrame)
        {
            ViewPrev();
        }

        if (Keyboard.current[Key.Digit0].wasPressedThisFrame)
        {
            ViewNext();
        }

    }

    public void ViewNext()
    {
        ClearView();
        if(toggleIndex >= ToggleList.Count - 1)
        {
            toggleIndex = 0;
        }
        else
        {
            toggleIndex++;
        }

        ShowCurrentItem();

    }

    public void ViewPrev()
    {
        ClearView();

        if (toggleIndex <= 0)
        {
            toggleIndex = ToggleList.Count -1;
        }
        else
        {
            toggleIndex--;
        }

        ShowCurrentItem();
      
    }

    public void ClearView()
    {
        ToggleList[toggleIndex].itemObject.SetActive(false);
        toggleNameLabel.text = null;
        toggleNumberLabel.text = null;
    }

    public void ShowCurrentItem()
    {
        ToggleList[toggleIndex].itemObject.SetActive(true);
        toggleNameLabel.text = ToggleList[toggleIndex].itemName;
        toggleNumberLabel.text = ToggleList[toggleIndex].itemIDNum;
    }
}

[System.Serializable]
public class ToggleItem
{
    public string itemName;

    public string itemIDNum;

    public GameObject itemObject;
}
