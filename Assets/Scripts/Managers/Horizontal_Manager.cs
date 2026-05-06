using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Horizontal_Manager : MonoBehaviour
{
    public static Horizontal_Manager Instance { get; private set; }

    public delegate void ToggleTetrasEvent();
    public event ToggleTetrasEvent ToggleTetras;

    public GameObject nexusMap;

    public GameObject fillAgents, coverAgents;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            ToggleTetras?.Invoke();
        }

        if (Keyboard.current.mKey.wasPressedThisFrame)
            nexusMap.SetActive(!nexusMap.activeSelf);

        if (Keyboard.current.iKey.wasPressedThisFrame)
            fillAgents.SetActive(!fillAgents.activeSelf);

        if (Keyboard.current.oKey.wasPressedThisFrame)
            coverAgents.SetActive(!coverAgents.activeSelf);

    }
}
