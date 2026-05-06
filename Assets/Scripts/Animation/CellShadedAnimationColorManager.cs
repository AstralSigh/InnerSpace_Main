using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellShadedAnimationColorManager : MonoBehaviour
{
    [SerializeField]    
    private List<Material> cellMaterials;
    [SerializeField]
    private Color startColor;
    [SerializeField] 
    private List<Color> cellColors;
    [SerializeField] 
    private float duration;

    private void Start()
    {
        foreach(Material m in cellMaterials) 
        { 
            m.color= startColor;
        }
    }

    public void StartAnimation()
    {
        StartCoroutine(LerpColor());
    }

    IEnumerator LerpColor()
    {
        float progress = 0;
        float increment = 1 / duration;

        while (progress < 1)
        {
            for(int x = 0; x < cellMaterials.Count; x++)
            {
                cellMaterials[x].color = Color.Lerp(startColor, cellColors[x], progress);
            }
            progress += increment * Time.deltaTime;
            yield return null;
        }
    }
}
