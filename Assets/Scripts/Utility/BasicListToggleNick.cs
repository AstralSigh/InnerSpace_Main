using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicListToggleNick : MonoBehaviour
{
    public bool activeState = false;
    public List<GameObject> openState;
    public List<GameObject> closedState;

    public int activeIndex = 0;

    private void Start()
    {
        setActiveStates();
    }

    public void ToggleSequenceForward()
    {
        activeState = !activeState;
        setActiveStates();
    }

    private void setActiveStates()
    {
        foreach (GameObject item in openState)
        {
            item.SetActive(activeState);
        }

        foreach (GameObject item in closedState)
        {
            item.SetActive(!activeState);
        }
    }
}
