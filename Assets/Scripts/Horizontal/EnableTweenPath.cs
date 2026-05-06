using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTweenPath : MonoBehaviour
{
    public Transform[] pathRefs;
    public GameObject propOnPath;
    public float pathProgress = 0.0f;
    public float pathSpeed = 0.001f;
    public bool methodTriggerOnly;

    void Start()
    {
        if(!methodTriggerOnly)
            StartCoroutine(PathAdvance());
    }


    void AdvanceOnPath(float pathProg)
    {
        if(propOnPath)
            iTween.PutOnPath(propOnPath, pathRefs, pathProg);
    }

    IEnumerator PathAdvance()
    {
        for (float i = 0.0f; i < 1.0; i += pathSpeed)
        {
            AdvanceOnPath(i);
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void PlayPathTween()
    {
        StartCoroutine(PathAdvance());
    }
}
