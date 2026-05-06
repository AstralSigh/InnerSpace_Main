using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class GradualReveal : MonoBehaviour
{
    public List<GameObject> revealTargets;

    public bool onEnable = true;
    public bool onEnableShow = false;

    public bool addChildrenOnce;
   
    private void OnEnable()
    {
        if(addChildrenOnce)
            AddTransformChildren();
     
        if (onEnable)
            StartReveal();

        if (onEnableShow)
            ShowOnly();
    }

    private void OnDisable()
    {
        foreach(GameObject go in revealTargets)
        {
            go.SetActive(false);
        }
    }

    IEnumerator RevealThem()
    {
        for(int i = 0; i < revealTargets.Count; i++)
        {
            revealTargets[i].SetActive(true);
            iTween.ScaleFrom(revealTargets[i], iTween.Hash("scale", Vector3.zero, "time", 1.0f, "easetype", iTween.EaseType.easeOutBack));
            yield return new WaitForSeconds(0.5f);
        }

    }

    IEnumerator ShrinkThem()
    {
        for (int i = 0; i < revealTargets.Count; i++)
        {
            revealTargets[i].SetActive(true);
            iTween.ScaleTo(revealTargets[i], iTween.Hash("scale", Vector3.zero, "time", 1.0f, "easetype", iTween.EaseType.easeOutBack));
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator EnlargeThem()
    {
        for (int i = 0; i < revealTargets.Count; i++)
        {
            revealTargets[i].SetActive(true);
            iTween.ScaleTo(revealTargets[i], iTween.Hash("scale", Vector3.one, "time", 1.0f, "easetype", iTween.EaseType.easeOutBack));
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator JustShowHideThem(bool activeState)
    {
        for (int i = 0; i < revealTargets.Count; i++)
        {
            revealTargets[i].SetActive(activeState);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void StartReveal()
    {
        StartCoroutine(RevealThem());
    }

    public void ShowOnly()
    {
        StartCoroutine(JustShowHideThem(true));
    }

    public void HideOnly()
    {
        StartCoroutine(JustShowHideThem(false));
    }

    public void StartShrink()
    {
        StartCoroutine(ShrinkThem());
    }

    void AddTransformChildren()
    {
        foreach(Transform child in gameObject.transform)
        {
            revealTargets.Add(child.gameObject);
        }
        addChildrenOnce = false;
    }

    void DeactivateTargets()
    {
        foreach(GameObject go in revealTargets)
        {
            go.SetActive(false);
        }
    }
}
