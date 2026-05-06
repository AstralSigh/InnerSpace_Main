using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAtConstituent : MonoBehaviour
{
    [SerializeField] private GameObject localConstituentRoot;

    [SerializeField] private GameObject localProteinStructure;

    // public void SendConReference()
    // {
    //     if (localConstituentRoot)
    //         _mainManager.currentConstituent = localConstituentRoot;

    //     if (localProteinStructure)
    //         _mainManager.currentStructure = localProteinStructure;
    // }
}
