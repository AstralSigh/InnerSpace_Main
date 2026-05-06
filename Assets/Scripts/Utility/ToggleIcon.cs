using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleIcon : MonoBehaviour
{
    enum ToggleState { on, off};
    [SerializeField] private ToggleState _currentState = ToggleState.off;
    [SerializeField] private Sprite[] _icons;

    private void Start()
    {
        switch (_currentState)
        {
            case ToggleState.off:
                this.transform.GetComponent<Image>().sprite = _icons[0];
                break;
            case ToggleState.on:
                this.transform.GetComponent<Image>().sprite = _icons[1];
                break;
        }
    }

    public void toggleIcon()
    {
        switch (_currentState)
        {
            case ToggleState.off:
                this.transform.GetComponent<Image>().sprite = _icons[1];
                _currentState = ToggleState.on;
                break;
            case ToggleState.on:
                this.transform.GetComponent<Image>().sprite = _icons[0];
                _currentState = ToggleState.off;
                break;
        }
    }
}
