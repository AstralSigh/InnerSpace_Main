using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;
using UnityEngine.Splines;
using UnityEngine.Rendering;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [SerializeField] NewHandJet[] handJets;
    [SerializeField] SplineAnimate splineAnimator;
    PlayerTeleport playerTeleport;
    [SerializeField] float startingJetForce;

    public delegate void PlayerTeleportEvent();
    public event PlayerTeleportEvent OnPlayerTeleported;
    public enum PlayerState { Nexus, NexusTour, Cutscene, Lobby, CrystalMiniGame }
    public PlayerState currentPlayerState = PlayerState.Lobby;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdatePlayerState(currentPlayerState);
        SetJetForce(startingJetForce);
        playerTeleport = GetComponent<PlayerTeleport>();
        if(!playerTeleport) Debug.LogError("PlayerManager must have a PlayerTeleport component");
    }

    public void TeleportPlayer(Vector3 destination, Quaternion rotation)
    {
        playerTeleport.TeleportPlayer(destination, rotation);
        OnPlayerTeleported?.Invoke();
    }

    public void SetJetForce(float forceAmount)
    {
        if (handJets.Length > 0)
        {
            foreach (NewHandJet jetScript in handJets)
            {
                jetScript.JetForce = forceAmount;
            }
        }
    }

    public void SplineAnimateEnabled(bool enabled)
    {
        splineAnimator.enabled = enabled;
    }

    public void UpdatePlayerState(PlayerState targetState)
    {
        currentPlayerState = targetState;

        switch (targetState)
        {
            case PlayerState.Lobby:
                ActionMenuManager_Prototype.Instance.OverrideMenu(ControllerHand.None, ActionMenuManager_Prototype.MenuType.ActionMenu, false, true);
                break;

            case PlayerState.Nexus:
                ActionMenuManager_Prototype.Instance.OverrideMenu(ControllerHand.None, ActionMenuManager_Prototype.MenuType.ActionMenu, false, true);
                break;

            case PlayerState.Cutscene:
                ActionMenuManager_Prototype.Instance.OverrideMenu(ControllerHand.None, ActionMenuManager_Prototype.MenuType.None, true, false);
                break;

            case PlayerState.NexusTour:
                ActionMenuManager_Prototype.Instance.OverrideMenu(ControllerHand.Left, ActionMenuManager_Prototype.MenuType.NexusTour, true, true);
                Debug.Log("PlayerManager is calling ActionMenuManager with NexusTour");
                break;

            case PlayerState.CrystalMiniGame:
                ActionMenuManager_Prototype.Instance.OverrideMenu(ControllerHand.None, ActionMenuManager_Prototype.MenuType.GameMenu, false, true);
                break;
        }
    }

 
}
