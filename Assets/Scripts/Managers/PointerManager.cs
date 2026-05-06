using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerManager : MonoBehaviour
{
    public static PointerManager Instance { get; private set; }
    public Transform rightPointer;
    public Transform leftPointer;
    public GameObject shortLaserRight;
    public GameObject shortLaserLeft;
    [SerializeField] bool rightActiveOnStart;
    [SerializeField] bool leftActiveOnStart;
    public float maxDistance = 100f;
    public LayerMask layerMask;
    public delegate void PointerToggleEvent();
    public event PointerToggleEvent OnDisablePointer;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (rightActiveOnStart) EnableRight();
        else if (leftActiveOnStart) EnableLeft();
    }

    public void EnableRight()
    {
        leftPointer.gameObject.SetActive(false);
        rightPointer.gameObject.SetActive(true);
    }

    public void EnableShortLaserRight()
    {
        shortLaserLeft.SetActive(false);
        shortLaserRight.SetActive(true);
    }

    public void EnableLeft()
    {
        leftPointer.gameObject.SetActive(true);
        rightPointer.gameObject.SetActive(false);
    }

    public void EnableShortLaserLeft()
    {
        shortLaserLeft.SetActive(true);
        shortLaserRight.SetActive(false);
    }

    public void HideBothPointers()
    {
        leftPointer.gameObject.SetActive(false);
        rightPointer.gameObject.SetActive(false);
    }

    public void EnableBothShortLasers()
    {
        shortLaserLeft.SetActive(true);
        shortLaserRight.SetActive(true);
    }

    public void HideBothShortLasers()
    {
        shortLaserLeft.SetActive(false);
        shortLaserRight.SetActive(false);
    }

    

    /// <summary>
    /// DisablePointer prevents the pointers from functioning but does NOT hide them.
    /// </summary>
    public void DisablePointer()
    {
        if(OnDisablePointer != null) OnDisablePointer();
    }


    
}