using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public enum NexusMapIntroObjects{
        PBCMenu,
        FloatingAminoAcid,
        Map,
        PBCObject
    }

    public enum GClusterIntroObjects{
        Intro,
        Experience,
        Player,
        LWristDock,
        RWristDock
    }

    public List<GameObject> GameObjectReferences;
    public List<GameObject> NexusMapIntroRefrences;
    public List<GameObject> GClusterRefrences;
    public GameObject GetReferencedObject(TutorialInputs.ObjectName reference)
    {
        return GameObjectReferences[(int)reference];
    }

    public GameObject GetNexusMapObject(NexusMapIntroObjects reference)
    {
        return NexusMapIntroRefrences[(int)reference];
    }

    public GameObject GetGClusterObject(GClusterIntroObjects reference)
    {
        return GClusterRefrences[(int)reference];
    }
}
