using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewOrbitControl : MonoBehaviour
{
    private HorizontalControls _horizontalControlActions;

    private void Awake()
    {
        _horizontalControlActions = new HorizontalControls();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }
}
