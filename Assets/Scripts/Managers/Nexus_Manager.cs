using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BNG;
using HighlightPlus;
using UnityEngine.Playables;

public class Nexus_Manager : MonoBehaviour
{
    [Header("Intro")]
    public GameObject introRoot;
    public bool skipIntro = false;
    [SerializeField] Material introSkyboxMaterial;
    [Header("General")]
    public Nexus_Data nexusData;
    public Transform initialPosition;
    [SerializeField] Material skyboxMaterial;
    [SerializeField] GameObject content;
    [Tooltip("Start time for timeline")]
    [SerializeField] float startTime = 0;
    [SerializeField] float jetForce = 2f;
    [SerializeField] int audioRegion; // FMOD parameter for setting the ambience audio.


    [Header("Vantage Points")]
    [SerializeField] Transform vantagePointContainer;
    public GameObject[] vantagePoints;

    void Start()
    {
        InitializeVantagePoints();

        WIAC_Manager.Instance.OnChangeNexus += SetupNexus;
    }

    private void SetupNexus(Nexus_Data.eNexusType currentNexus)
    {
        if(currentNexus == nexusData.nexusType) // If we are the current nexus, initialize.
        {
            if (introRoot && !skipIntro) {
                skipIntro = true;
                TimeManager.Instance.SetPlayState(false);
                RenderSettings.skybox = introSkyboxMaterial;
                PlayerManager.Instance.TeleportPlayer(initialPosition.transform.position + new Vector3(0, 1, 0), initialPosition.rotation);
                PlayerManager.Instance.SetJetForce(0);
                PlayerManager.Instance.UpdatePlayerState(PlayerManager.PlayerState.Cutscene);
                PointerManager.Instance.HideBothPointers();
                AudioManager.Instance.SetAudioRegion(audioRegion);

                introRoot.SetActive(true);
                return;
            }
            // Set skybox.
            RenderSettings.skybox = skyboxMaterial;

            // Teleport player to starting position.
            PlayerManager.Instance.transform.position = initialPosition.transform.position + new Vector3(0, 1, 0);

            // Initialize time.
            TimeManager.Instance.SetPlayState(false);
            TimeManager.Instance.SetCurrentDirector(GetComponent<PlayableDirector>());
            TimeManager.Instance.SetTime(startTime);
            TimeManager.Instance.SetPlayState(true);

            // Set audio region.
            AudioManager.Instance.SetAudioRegion(audioRegion);

            // Set movement speed.
            PlayerManager.Instance.SetJetForce(jetForce);

            // Activate content.
            content.SetActive(true);
            vantagePointContainer.gameObject.SetActive(true);

            //AUDIO
            SimulationAudioManager.Instance.OnEnterNexus(currentNexus);

            //UPDATE PLAYER STATE
            PlayerManager.Instance.UpdatePlayerState(PlayerManager.PlayerState.Nexus);
        }
        else // Otherwise, reset.
        {
            content.SetActive(false);
            vantagePointContainer.gameObject.SetActive(false);
        }
    }

    private void InitializeVantagePoints()
    {
        GameObject[] vantagePoints = new GameObject[VantagePointManager.Instance.GetVantagePointCount()];

        for(int i=0; i < vantagePoints.Length; i++)
        {
            GameObject vp = Instantiate(VantagePointManager.Instance.GetVantagePointPrefab(), Vector3.zero, Quaternion.identity);
            vp.name = "VantagePoint" + i;
            vp.transform.parent = vantagePointContainer.transform;
            vp.GetComponent<VantagePoint>().vantagePointIndex = i;
            vp.GetComponent<VantagePoint>().SetText((i+1).ToString());
            vp.SetActive(false); // Initially, vantage points are all disabled (hidden).
            
            vantagePoints[i] = vp;
        }

        this.vantagePoints = vantagePoints;
    }

    public void EnableVantagePoint(int index)
    {
        vantagePoints[index].transform.position = VantagePointManager.Instance.GetSpawnLocation().position;
        vantagePoints[index].SetActive(true);
    }

    public void DeleteVantagePoint(int index)
    {
        vantagePoints[index].SetActive(false);
    }

    public void TeleportToVantagePoint(int index)
    {
        GameObject targetVP = vantagePoints[index];
        PlayerManager.Instance.TeleportPlayer(targetVP.transform.position - (targetVP.transform.forward.normalized * 0.5f), Quaternion.identity);
    }

    public GameObject[] GetVantagePoints()
    {
        return vantagePoints;
    }



    public float GetCurrentJetForce()
    {
        return jetForce;
    }
}
