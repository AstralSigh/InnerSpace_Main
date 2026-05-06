using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class IntroManager : MonoBehaviour
{
    [SerializeField] PlayableDirector introTimeline;
    [SerializeField] Nexus_Manager startingNexus;
    [SerializeField] GameObject introRoot;
    [SerializeField] GameObject lobbyRoot;
    [SerializeField] TourWheelManager_Prototyping tourWheel; //This reference is burried in player hierarchy
    [SerializeField] int audioRegion = 5;

    //ACTIVATED WHEN YOU LASER INNER SPACE LOGO 
    public void PlayIntro()
    {
        lobbyRoot.SetActive(false);
        introRoot.SetActive(true);
        introTimeline.Play();
        PlayerManager.Instance.UpdatePlayerState(PlayerManager.PlayerState.Cutscene);
        PointerManager.Instance.HideBothPointers();
        AudioManager.Instance.SetAudioRegion(audioRegion);
    }

    public void EndIntro()
    {
        introRoot.SetActive(false);
        WIAC_Manager.Instance.EnterNexus(startingNexus);
    }
}
