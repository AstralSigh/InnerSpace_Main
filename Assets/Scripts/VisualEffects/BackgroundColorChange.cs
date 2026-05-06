using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundColorChange : MonoBehaviour
{
    public float t;
    public Color startColor;
    public Color endColor;
    public Color setColor;
    public List<Camera> cameraList;
    public List<Material> gradSkyboxMaterials;
    public float refTime;
    void Update()
    {
        setColor = Color.Lerp(startColor, endColor, t);
        foreach (Camera c in cameraList)
        {
            c.backgroundColor = setColor;
        }

        RenderSettings.skybox.SetColor("_SkyColor1", setColor);
        RenderSettings.skybox.SetColor("_SkyColor2", setColor);
        RenderSettings.skybox.SetColor("_SkyColor3", setColor);

        
        foreach (Material m in gradSkyboxMaterials)
        {
            m.SetColor("_SkyColor1", setColor);
            m.SetColor("_SkyColor2", setColor);
            m.SetColor("_SkyColor3", setColor);

        }
        
    }
}
