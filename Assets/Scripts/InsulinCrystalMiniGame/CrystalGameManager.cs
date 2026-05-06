using BNG;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// This is the game manager that initates what happens when a hexamer is created. After three hexamers are generated the tiling game gets initiated. 
/// </summary>

public class CrystalGameManager : MonoBehaviour
{
    public static CrystalGameManager Instance { get; private set; }

    //SETTINGS
    [SerializeField] private float gameDuration;
    [Tooltip("Where the character will be teleported at the end of the crystal tiling sequence")]
    public Transform endPosition;
    [SerializeField] private MonomerMaster_MiniGame02[] monomerMasters;
    [SerializeField] private HexamerFormationColorProfiles colorProfiles;
    [SerializeField] private GameObject crystalManager;
    [SerializeField] private TextMeshPro timerText;
    [SerializeField] private ObjectSpawner objectSpawner;
    [SerializeField] private GameObject spiderGraph;
    [SerializeField] private Text hexamerSubtitle;
    float time;
    private int beatIndex = 0;

    public void Awake(){
        Instance = this;
    }

    public void Start()
    {
        PlayerManager.Instance.UpdatePlayerState(PlayerManager.PlayerState.CrystalMiniGame);
        PlayerManager.Instance.SetJetForce(1);
    }

    public void SkipHexamerFormation()
    {
        monomerMasters[0].transform.parent.gameObject.SetActive(false);
        monomerMasters[1].transform.parent.gameObject.SetActive(false);
        monomerMasters[2].transform.parent.gameObject.SetActive(false);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Object_Emitters/hexamer_binding", this.transform.position);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Object_Emitters/glp1_binding_oneshot", this.transform.position);
        crystalManager.SetActive(true);
        QuestLogManager.Instance.SetBeat(9);
        StartCoroutine(PlayGame(gameDuration));
    }

    public void FinishMonomer()
    {
        if (beatIndex == 0)
        {
            monomerMasters[0].ChangeColor(colorProfiles.hexamer1);
            FMODUnity.RuntimeManager.PlayOneShot2D("event:/UI Events/UI_hexamer_color_change");
        }
        else if (beatIndex == 1)
        {
            monomerMasters[0].ChangeColor(colorProfiles.hexamer2);
            monomerMasters[1].ChangeColor(colorProfiles.hexamer2);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Object_Emitters/hexamer_binding", this.transform.position);
        }
        else if (beatIndex == 2)
        {
            monomerMasters[0].transform.parent.gameObject.SetActive(false);
            monomerMasters[1].transform.parent.gameObject.SetActive(false);
            monomerMasters[2].transform.parent.gameObject.SetActive(false);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Object_Emitters/hexamer_binding", this.transform.position);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Object_Emitters/glp1_binding_oneshot", this.transform.position);
            crystalManager.SetActive(true);
            StartCoroutine(PlayGame(gameDuration));
        }
        beatIndex++;
    }

    IEnumerator PlayGame(float duration)
    {
        hexamerSubtitle.text = "Hexamers\nPlaced";
        timerText.transform.gameObject.SetActive(true);
        time = duration;
        while(time > 0)
        {
            time -= Time.deltaTime;            
            TimeSpan timeB = TimeSpan.FromSeconds((double)time);
            timerText.text = timeB.ToString("mm':'ss");
            yield return new WaitForEndOfFrame();
        }
        PlayerManager.Instance.TeleportPlayer(endPosition.position, endPosition.rotation);
        FMODUnity.RuntimeManager.PlayOneShot2D("event:/UI Events/scene_transition");
        yield return new WaitForSeconds(PlayerManager.Instance.transform.GetComponent<PlayerTeleport>().TeleportFadeSpeed);
        objectSpawner.HideNonBindedHexamers();
        CrystalManager.Instance.TurnOffSlots();
        spiderGraph.SetActive(true);
        timerText.text = "";
        //END STUFF
    }

    public float GetTime(){
        return gameDuration;
    }

    public void RestartGame()
    {
        LoadingData.sceneToLoad = "HexamerFormation";
        SceneManager.LoadScene("LoadScreen", LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        AudioManager.Instance.EndAmbienceInstance();
        LoadingData.sceneToLoad = "Main";
        LoadingData.nexiToLoad = 8;
        SceneManager.LoadScene("LoadScreen", LoadSceneMode.Single);
    }

}
