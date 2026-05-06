using BNG;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActionMenuManager_Prototype : MonoBehaviour
{
    public static ActionMenuManager_Prototype Instance { get; private set; }

    [Header("REFERENCES")]
    [SerializeField] private Transform leftHandControllerAnchor;  //Anchor of the action menu
    [SerializeField] private Transform rightHandControllerAnchor;  //Anchor of the action menu
    [SerializeField] private Transform actionMenuRoot;

    [Header("VARIABLES")]
    //[SerializeField] private bool isMenuDisplayed;
    [SerializeField] private bool isTransformTweening;
    [SerializeField] private float menuTweenDuration;
    public bool menuLocked = false;
    public bool laserAllowed = true;
    //[SerializeField] private List<Transform> clickableItems;
    //public bool isOpen = false;Sequence mySequence = DOTween.Sequence();

    //MENU STATES NICK REFACTOR - 07/18/2024
    public ControllerHand currentMenuOrientation;
    public ControllerHand currentLaserHand;
    
    public enum MenuType { ActionMenu, NexusTour, GameMenu, None };
    public MenuType currentMenuType;

    public Transform rightPointer;
    public Transform leftPointer;
    public float maxDistance = 100f;
    public LayerMask layerMask;

    private void Awake()
    {
        Instance = this;
        actionMenuRoot.parent = leftHandControllerAnchor;
        actionMenuRoot.transform.localScale = Vector3.zero;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to events.
        SelectedConstituentManager.Instance.OnConstituentSelect += OnConSelect;
        PlayerManager.Instance.OnPlayerTeleported += OnPlayerTeleport;
    }

    // Update is called once per frame
    void Update()
    {
        if (!menuLocked)
        {
            if (InputBridge.Instance.AButtonDown)
            {
                MenuButtonSelected(ControllerHand.Right);
            }
            if (InputBridge.Instance.XButtonDown)
            {
                MenuButtonSelected(ControllerHand.Left);
            }
        }

        if (currentMenuOrientation == ControllerHand.None && laserAllowed)
        {
            EnableDominantHand();
        }
        
    }

    //DESIGNERS CONTROL MENU WITH THIS
    public void OverrideMenu(ControllerHand ControllerHand, MenuType menuType, bool menuLocked, bool laserAllowed)
    {
        switch (menuType)
        {
            case MenuType.ActionMenu:
                foreach(Transform child in actionMenuRoot.GetChild(0))
                {
                    child.gameObject.SetActive(true);
                }
                actionMenuRoot.GetChild(1).GetChild(0).gameObject.SetActive(false);
                actionMenuRoot.GetChild(2).GetChild(0).gameObject.SetActive(false);
                break;

            case MenuType.NexusTour:
                foreach (Transform child in actionMenuRoot.GetChild(0))
                {
                    child.gameObject.SetActive(false);
                }
                actionMenuRoot.GetChild(1).GetChild(0).gameObject.SetActive(true);
                actionMenuRoot.GetChild(2).GetChild(0).gameObject.SetActive(false);
                break;

            case MenuType.GameMenu:
                foreach (Transform child in actionMenuRoot.GetChild(0))
                {
                    child.gameObject.SetActive(false);
                }
                actionMenuRoot.GetChild(1).GetChild(0).gameObject.SetActive(false);
                actionMenuRoot.GetChild(2).GetChild(0).gameObject.SetActive(true);
                break;
        }

        switch (ControllerHand)
        {
            case ControllerHand.Left:
                OpenMenuTween(ControllerHand.Left);
                break;
            case ControllerHand.Right:
                OpenMenuTween(ControllerHand.Right);
                break;
            case ControllerHand.None:
                if (currentMenuOrientation != ControllerHand.None)
                {
                    CloseMenu();
                }
                break;
        }

        this.menuLocked = menuLocked;
        this.laserAllowed = laserAllowed;

        ManageLasers();
    }

    public void ManageLasers()
    {
        if (laserAllowed)
        {
            switch (currentMenuOrientation)
            {
                case ControllerHand.Left:
                    PointerManager.Instance.EnableRight();
                    PointerManager.Instance.EnableShortLaserRight();
                    currentLaserHand = ControllerHand.Right;
                    break;
                case ControllerHand.Right:
                    
                    PointerManager.Instance.EnableLeft();
                    PointerManager.Instance.EnableShortLaserLeft();
                    currentLaserHand = ControllerHand.Left;
                    break;
                case ControllerHand.None:

                    EnableDominantHand();

                    PointerManager.Instance.EnableBothShortLasers();
                    break;
            }
        }
        else
        {
            PointerManager.Instance.HideBothPointers();
            PointerManager.Instance.HideBothShortLasers();
        }
    }

    void MenuButtonSelected(ControllerHand input)
    {
        if (isTransformTweening || menuLocked)
        {
            return;
        }

        switch (input)
        {
            case ControllerHand.Left:
                if (currentMenuOrientation == ControllerHand.Left)
                {
                    CloseMenu();
                }
                if (currentMenuOrientation == ControllerHand.Right)
                {
                    SwtichHandTween(ControllerHand.Left);
                }
                else if (currentMenuOrientation == ControllerHand.None) 
                {
                    OpenMenuTween(ControllerHand.Left);
                }
                break;

            case ControllerHand.Right:
                if (currentMenuOrientation == ControllerHand.Left)
                {
                    SwtichHandTween(ControllerHand.Right);
                }
                if (currentMenuOrientation == ControllerHand.Right)
                {
                    CloseMenu();
                }
                else if (currentMenuOrientation == ControllerHand.None)
                {
                    OpenMenuTween(ControllerHand.Right);
                }
                break;
        }
        
    }

    private void OpenMenuTween(ControllerHand targetHand)
    {
        if (isTransformTweening)
        {
            return;
        }

        if(targetHand == ControllerHand.Left)
        {
            actionMenuRoot.parent = leftHandControllerAnchor;
        }
        else if(targetHand == ControllerHand.Right)
        {
            actionMenuRoot.parent = rightHandControllerAnchor;
        }

        // actionMenuRoot.gameObject.SetActive(true);
        var controllerPos = actionMenuRoot.parent.parent.position;
        actionMenuRoot.position = controllerPos;
        actionMenuRoot.localScale = Vector3.zero;
        isTransformTweening = true;

        // Move from the controller pos to the anchor pos
        actionMenuRoot.DOLocalMove(Vector3.zero, menuTweenDuration).SetEase(Ease.InOutSine).OnComplete(() => { 
            isTransformTweening = false;
            currentMenuOrientation = targetHand;
            ManageLasers();

        }); 
        actionMenuRoot.localRotation = Quaternion.identity;
        actionMenuRoot.DOScale(Vector3.one, menuTweenDuration).SetEase(Ease.InOutSine);        
    }
    public void CloseMenu()
    {
        if (isTransformTweening)
        {
            return;
        }

        var controllerPos = actionMenuRoot.parent.parent.position;
        isTransformTweening = true;

        // Move from the anchor pos to the controller pos
        actionMenuRoot.DOMove(controllerPos, menuTweenDuration).SetEase(Ease.InOutSine).OnComplete(() => {
            isTransformTweening = false;
            currentMenuOrientation = ControllerHand.None;
            ManageLasers();
        });

        actionMenuRoot.localRotation = Quaternion.identity;
        actionMenuRoot.DOScale(Vector3.zero, menuTweenDuration).SetEase(Ease.InOutSine);
        
    }

    public void SwtichHandTween(ControllerHand targetHand)
    {
        if(isTransformTweening)
        {
            return;
        }

        if (targetHand == ControllerHand.Left)
        {
            actionMenuRoot.parent = leftHandControllerAnchor;
        }
        else if (targetHand == ControllerHand.Right)
        {
            actionMenuRoot.parent = rightHandControllerAnchor;
        }

        isTransformTweening = true;

            // Move from one anchor pos to another one
            actionMenuRoot.DOLocalMove(Vector3.zero, menuTweenDuration).SetEase(Ease.InOutSine).OnComplete(() => {
                isTransformTweening = false;
                currentMenuOrientation = targetHand;
                ManageLasers();

            });
            actionMenuRoot.DOLocalRotate(Vector3.zero, menuTweenDuration).SetEase(Ease.InOutSine);
    }

    private void OnConSelect(Constituent constituent)
    {
        if (constituent == null) { return; }
        if (currentMenuOrientation == ControllerHand.None)
        {
            if(GetDominantHand() == ControllerHand.Left)
            {
                OpenMenuTween(ControllerHand.Right);
            }
            else if(GetDominantHand() == ControllerHand.Right)
            {
                OpenMenuTween(ControllerHand.Left);
            }

            
        }
        //Small future optimziation: Get dominant laser hand
        //GET DOMINANT HAND

    }

    //USE THIS WHEN BOTH LASERS ACTIVE
    private ControllerHand GetDominantHand()
    {
        RaycastHit[] hits = Physics.RaycastAll(leftPointer.position, leftPointer.forward, maxDistance, layerMask);

        foreach (RaycastHit h in hits)
        {
            if (h.collider.transform.GetComponent<LaserButton>() != null || h.collider.transform.GetComponent<PointerEvents>() != null)
            {
                return ControllerHand.Left; 
            }
        }

        hits = Physics.RaycastAll(rightPointer.position, rightPointer.forward, maxDistance, layerMask);

        foreach (RaycastHit h in hits)
        {
            if (h.collider.transform.GetComponent<LaserButton>() != null || h.collider.transform.GetComponent<PointerEvents>() != null)
            {
                return ControllerHand.Right; 
            }
        }

        return ControllerHand.None;
    }

    public void EnableDominantHand()
    {
        ControllerHand targetHand = GetDominantHand();

        if (targetHand == ControllerHand.Left)
        {
            currentLaserHand = ControllerHand.Left;
            PointerManager.Instance.EnableLeft();
        }
        if (targetHand == ControllerHand.Right)
        {
            currentLaserHand = ControllerHand.Right;
            PointerManager.Instance.EnableRight();
        }
    }
    public void OnPlayerTeleport()
    {
        if(currentMenuOrientation != ControllerHand.None)
        {
            CloseMenu();
        } 
    }

    public ControllerHand GetCurrentLaserHand()
    {
        return currentLaserHand;
    }
}
