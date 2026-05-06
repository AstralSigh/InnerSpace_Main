using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightPlus;

public class BasicColorChange : MonoBehaviour
{
    public Color targetColor;
    public Color defaultColor;
    public List<GameObject> targetMeshes;

    public float changeTime = 1.0f;
    public iTween.EaseType animEasing = iTween.EaseType.easeInCubic;
    public bool colorSharesActiveState;
    public bool defaultColorActive;
    public bool highlightEffect;

    public bool addChildMeshes;

    private void Start()
    {
        if (addChildMeshes)
            GetChildMeshes();

        if (colorSharesActiveState)TargetColorChange();

        if (defaultColorActive) DefaultColorChange();
    }

    private void OnEnable()
    {
        if (colorSharesActiveState)
            TargetColorChange();
    }

    private void OnDisable()
    {
        if (colorSharesActiveState)
            DefaultColorChange();
    }

    public void TargetColorChange()
    {
        foreach(GameObject go in targetMeshes)
        {
            Material targetMaterial = go.GetComponent<Renderer>().material;

            //iTween.ColorTo(go, iTween.Hash("color", targetColor, "time", changeTime, "easetype", animEasing));
            SmoothColorChange(targetMaterial);

            if (highlightEffect)
                go.GetComponent<HighlightEffect>().highlighted = true;
        }
    }

    public void DefaultColorChange()
    {
        foreach (GameObject go in targetMeshes)
        {
            iTween.ColorTo(go, iTween.Hash("color", defaultColor, "time", changeTime, "easetype", animEasing));
            if (highlightEffect)
                go.GetComponent<HighlightEffect>().highlighted = false; ;
        }
    }

    public void GetChildMeshes()
    {
        MeshRenderer[] meshRends = this.gameObject.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer go in meshRends)
        {
            targetMeshes.Add(go.gameObject);
        }
    }

    void SmoothColorChange(Material targetMaterial)
    {
        StartCoroutine(SmoothTargetColorChange(targetMaterial, 1.0f));
    }

    private IEnumerator SmoothTargetColorChange(Material goMaterial, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            Color newColor = Color.Lerp(defaultColor, targetColor, t);
            goMaterial.color = newColor;
            yield return null; // Wait for the next frame
        }
        goMaterial.color = targetColor; // Ensure the final color is set
    }

    private IEnumerator SmoothDefaultColorChange(Material goMaterial, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            Color newColor = Color.Lerp(targetColor, defaultColor, t);
            goMaterial.color = newColor;
            yield return null; // Wait for the next frame
        }
        goMaterial.color = defaultColor; // Ensure the final color is set
    }
}
