using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WIAC_Manager : MonoBehaviour
{
    public static WIAC_Manager Instance { get; private set; }

    public GameObject rootNexusMap; //Root transform of Map + Lobby space
    public Nexus_Manager currentNexusManager; //current active Nexus manager
    [SerializeField] Transform titleScreenInitialPosition;
    public GameObject lobby;
    
    [Header("Debug")]
    [SerializeField] bool startInNexus;
    [SerializeField] Nexus_Manager startingNexus;
    public Nexus_Manager insulinFormation; //Jank reference for exiting Crystal Mini Game
    public Nexus_Manager insulinRelease; //Jank reference for quest build

    // Events
    public delegate void NexusChangeEvent(Nexus_Data.eNexusType currentNexus);
    public event NexusChangeEvent OnChangeNexus;
    public delegate void InfoNodeEvent(int infoNodeCount); // TODO: This event is a bit of a hack to allow the info node menu to update.
    public delegate void NexusTourEvent(bool nexusTourActive);
    public event NexusTourEvent OnToggleNexusTour;
    public delegate void NexusTourStopEvent(TourStop_Prototyping stop);
    public event NexusTourStopEvent OnNexusTourStopChanged;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //PlayerManager.Instance.TeleportPlayer(titleScreenInitialPosition.position, Quaternion.identity);
        
        StartCoroutine(LateStart());

        
    }
    
    //SHORT TERM FIX  
    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        if (LoadingData.nexiToLoad == 8)
        {
            PlayerManager.Instance.UpdatePlayerState(PlayerManager.PlayerState.Nexus);
            Debug.Log("start starting at " + SceneManager.GetActiveScene().name);
            insulinFormation.skipIntro = true;
            EnterNexus(insulinFormation);
            StopCoroutine(LateStart());
        }

        if (startInNexus)
        {
            EnterNexus(startingNexus);
            StopCoroutine(LateStart());
        }
    }

    // Called as a Unity event from Action Menu map buttons. - Michael 7/5/2023
    public void EnterNexus(Nexus_Manager nexusManager)
    {
        if(nexusManager == insulinRelease)
        {
            #if UNITY_ANDROID
            return;
            #endif  
        }
        lobby.SetActive(false);

        currentNexusManager = nexusManager;

        AudioManager.Instance.PlaySceneTransition();

        rootNexusMap.SetActive(false); //TODO: Why are we hardcoding map to be set inactive instead of properly managing action menu states. - Nick 11/09/2023
        
        PlayerManager.Instance.UpdatePlayerState(PlayerManager.PlayerState.Nexus);

        // Fire change nexus event.
        OnChangeNexus?.Invoke(nexusManager.nexusData.nexusType);

        
    }

    //Called from Action Menu and NexusTourIndicator object
    public void ToggleNexusTour(bool nexusTourActive)
    {
        if (OnToggleNexusTour != null) OnToggleNexusTour(nexusTourActive);
    }

    public void EnterCrystalMiniGame()
    {
        if (currentNexusManager.nexusData.nexusName == "Insulin Crystallization")
        {
            AudioManager.Instance.EndAmbienceInstance();
            LoadingData.sceneToLoad = "HexamerFormation";
            SceneManager.LoadScene("LoadScreen", LoadSceneMode.Single);
            return;
        }
    }

    public void NotifyNexusTourStopChanged(TourStop_Prototyping newStop) {
        OnNexusTourStopChanged?.Invoke(newStop);
    }
}
