using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using BNG;

// NOTE: The skeleton of this class was copied from BNG.PointerEvents
public class LaserButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

    [Header("Maximium Distance")]
    [Tooltip("Maximum Distance this object can be from the UIPointer to be considered valid and receive events")]
    public float MaxDistance = 100f;

    [Header("Teleportation")]
    [Tooltip("For distances greater than this, the player will be teleported if teleportation is enabled")]
    [SerializeField] float teleportMinDistance = 3f;
    [SerializeField] bool teleportToButton = false;

    [Header("Enable Events")]
    [Tooltip("If True then the Unity Events below will be sent. Set to False if you need to disable sending pointer events.")]
    public bool Enabled = true;

    [Header("Unity Events : ")]
    public PointerEventDataEvent OnPointerClickEvent;
    public PointerEventDataEvent OnPointerEnterEvent;
    public PointerEventDataEvent OnPointerExitEvent;
    public PointerEventDataEvent OnPointerDownEvent;
    public PointerEventDataEvent OnPointerUpEvent;

    // Player reference for teleportation.
    GameObject player;

    public void Start()
    {
        player = GameObject.Find("PlayerController"); // Note: Unfortunately this is probably the easiest way to do this.
    }

    public virtual void OnPointerClick(PointerEventData eventData) {
        // Don't call events if exceeded distance
        if(DistanceExceeded(eventData)) {
            return;
        }
        OnPointerClickEvent?.Invoke(eventData);
        //Play sound 
        FMODUnity.RuntimeManager.PlayOneShot2D("event:/UI Events/laser_confirm");
    }

    public virtual void OnPointerEnter(PointerEventData eventData) {
        // Don't call events if exceeded distance
        if (DistanceExceeded(eventData)) {
            return;
        }
        OnPointerEnterEvent?.Invoke(eventData);

        HapticsManager.Instance.OneShotVibration(HapticsManager.VibrationPreset.OnLaser);
    }

    public virtual void OnPointerExit(PointerEventData eventData) {
        // Can call OnPointerExit events even if exceeded distance
        OnPointerExitEvent?.Invoke(eventData);
    }


    public virtual void OnPointerDown(PointerEventData eventData) {
        // Don't call events if exceeded distance
        if (DistanceExceeded(eventData)) {
            return;
        }

        HapticsManager.Instance.OneShotVibration(HapticsManager.VibrationPreset.OnLaser);


        if (teleportToButton && eventData.pointerCurrentRaycast.distance > teleportMinDistance)
        {
            // Determine teleportation point.
            Vector3 playerPosition = player.transform.position;
            Vector3 buttonPosition = transform.position;
            Vector3 playerButtonHorizontalDiff = new Vector3(playerPosition.x - buttonPosition.x, buttonPosition.y, playerPosition.z - buttonPosition.z);
            Vector3 teleportLocation = buttonPosition + playerButtonHorizontalDiff.normalized;

            // Teleport the player.
            player.GetComponent<PlayerTeleport>().TeleportPlayer(teleportLocation, player.transform.rotation);
        }
        else{
            OnPointerDownEvent?.Invoke(eventData);
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData) {
        // Can call OnPointerUp events even if exceeded distance
        OnPointerUpEvent?.Invoke(eventData);
    }

    public virtual bool DistanceExceeded(PointerEventData eventData) {

        if(eventData == null) {
            return false;
        }

        if(eventData.pointerCurrentRaycast.distance > MaxDistance) {
            return true;
        }

        return false;
    }

}
