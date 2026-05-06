using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrascapeToggle : MonoBehaviour
{
    public GameObject legacyMesh;
    public GameObject tetrascapeMesh;


    private void OnEnable()
    {
        if( Horizontal_Manager.Instance != null )
        {
            Horizontal_Manager.Instance.ToggleTetras += ToggleTetraMeshes;

        }
    }

    private void OnDisable()
    {
        if (Horizontal_Manager.Instance != null)
        {
            Horizontal_Manager.Instance.ToggleTetras -= ToggleTetraMeshes;

        }
    }

    void ToggleTetraMeshes()
    {
        legacyMesh.SetActive(!legacyMesh.activeSelf);
        tetrascapeMesh.SetActive(!tetrascapeMesh.activeSelf);
    }

    void SetTetrascapeMesh()
    {
        legacyMesh.SetActive(false);
        tetrascapeMesh.SetActive(true);
    }

    void SetLegacyMesh()
    {
        legacyMesh.SetActive(true);
        tetrascapeMesh.SetActive(false);
    }
}
