using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Can safely delete. NexusManager now controls this.
public class BackgroundSkyboxManager : MonoBehaviour
{
    
    [SerializeField]
    List<Material> skyboxMaterials;

    public void SetNexusBackground(Nexus_Data.eNexusType nexusType)
    {
        //RenderSettings.skybox = null;

        switch (nexusType)
        {
            case Nexus_Data.eNexusType.None:
                RenderSettings.skybox = skyboxMaterials[0];

                break;

            case Nexus_Data.eNexusType.GCluster:
                RenderSettings.skybox = skyboxMaterials[1];

                break;

            case Nexus_Data.eNexusType.ImmatureCrystal:
                RenderSettings.skybox = skyboxMaterials[2];

                break;

            case Nexus_Data.eNexusType.InsulinRelease:
                RenderSettings.skybox = skyboxMaterials[3];

                break;
        }

    }

    public void SetDefaultBackground()
    {
        RenderSettings.skybox = skyboxMaterials[skyboxMaterials.Count-1];
    }
}
