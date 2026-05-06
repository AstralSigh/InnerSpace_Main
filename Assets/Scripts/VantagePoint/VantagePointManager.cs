using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

public class VantagePointManager : MonoBehaviour
{
    public static VantagePointManager Instance { get; private set; }

    [SerializeField] private int vantagePointCount = 5;
    [SerializeField] private Transform spawnLocation;
    [SerializeField] private GameObject vantagePointPrefab;
    [SerializeField] private GameObject player;

    public delegate void VantagePointEvent(int vantagePointIndex);
    public event VantagePointEvent OnDeleteVantagePoint;
    
    void Awake()
    {
        Instance = this;
    }

    public void EnableVantagePoint(int index)
    {
        WIAC_Manager.Instance.currentNexusManager.EnableVantagePoint(index);

    }

    public void TeleportPlayerToVantagePoint(int index)
    {
        WIAC_Manager.Instance.currentNexusManager.TeleportToVantagePoint(index);
    }

    public bool VantagePointIsActive(int index)
    {
        return WIAC_Manager.Instance.currentNexusManager.GetVantagePoints()[index].activeSelf;
    }

    public void DeleteVantagePoint(int index)
    {
        WIAC_Manager.Instance.currentNexusManager.DeleteVantagePoint(index);
        if(OnDeleteVantagePoint != null) OnDeleteVantagePoint(index);
    }

    public int GetVantagePointCount()
    {
        return vantagePointCount;
    }

    public GameObject GetVantagePointPrefab()
    {
        return vantagePointPrefab;
    }

    public Transform GetSpawnLocation()
    {
        return spawnLocation;
    }
}
