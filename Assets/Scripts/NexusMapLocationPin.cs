using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NexusMapLocationPin : MonoBehaviour
{
    public GameObject locationPin;

    public enum HorzScene
    {
        Scene0_GlucoseReception,
        Scene1_SignalReception,
        Scene2_SignalRelay,
        Scene3_InsulinRelease,
        Scene4_InsulinReception,
        Scene5_NucleusResponse,
        Scene6_TransportPacking
    }
    public HorzScene currentHorzScene;

    public List<Transform> SceneStops_Glucose, SceneStops_Signal, SceneStops_Relay, SceneStops_Release, SceneStops_InReceptor, 
        SceneStops_Translation, SceneStops_Transport;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSceneStop(int sceneStop)
    {
        switch (currentHorzScene)
        {
            case HorzScene.Scene0_GlucoseReception:
                if (sceneStop < SceneStops_Glucose.Count)
                    MoveLocationPin(SceneStops_Glucose[sceneStop]);
                break;

            case HorzScene.Scene1_SignalReception:
                if (sceneStop < SceneStops_Signal.Count)
                    MoveLocationPin(SceneStops_Signal[sceneStop]);
                break;

            case HorzScene.Scene2_SignalRelay:
                if (sceneStop < SceneStops_Relay.Count)
                    MoveLocationPin(SceneStops_Relay[sceneStop]);
                break;

            case HorzScene.Scene3_InsulinRelease:
                if (sceneStop < SceneStops_Release.Count)
                    MoveLocationPin(SceneStops_Release[sceneStop]);
                break;

            case HorzScene.Scene4_InsulinReception:
                if (sceneStop < SceneStops_InReceptor.Count)
                    MoveLocationPin(SceneStops_InReceptor[sceneStop]);
                break;

            case HorzScene.Scene5_NucleusResponse:
                if (sceneStop < SceneStops_Translation.Count)
                    MoveLocationPin(SceneStops_Translation[sceneStop]);
                break;

            case HorzScene.Scene6_TransportPacking:
                if (sceneStop < SceneStops_Transport.Count)
                    MoveLocationPin(SceneStops_Transport[sceneStop]);
                break;
        }
    }

    public void MoveLocationPin(Transform destRef)
    {
        locationPin.transform.position = destRef.position;
    }
}
