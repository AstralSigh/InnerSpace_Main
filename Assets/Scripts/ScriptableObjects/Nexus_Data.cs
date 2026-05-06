using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/NexusData")]
public class Nexus_Data : ScriptableObject
{
    public string nexusName;

    public string nexusFullName;

    public enum eNexusType
    {
        None,
        GCluster,
        ACConversion, // FUTURE IMPLEMENTATION (AS OF 5/22/23)
        PKARelay, // FUTURE IMPLEMENTATION (AS OF 5/22/23)
        Nucleus, // FUTURE IMPLEMENTATION (AS OF 5/22/23)
        RibosomeTranslation, // FUTURE IMPLEMENTATION (AS OF 5/22/23)
        VesicleTraffic, // FUTURE IMPLEMENTATION (AS OF 5/22/23)
        Golgi, // FUTURE IMPLEMENTATION (AS OF 5/22/23)
        ImmatureCrystal,
        InsulinRelease,
        Tutorial
    }

    public eNexusType nexusType;

    [TextArea(5,5)]
    public string nexusDescription;

    public ConstituentData[] constituentDatas;
}
