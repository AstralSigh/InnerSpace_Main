using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class BindingPocketRenderer_PROTOTYPING : MonoBehaviour
{
    public LineRenderer lineRenderer;
    [SerializeField] private List<Transform> rGroupPositions;
    

    //BASIC LINE RENDERING
    void Update()
    {
        lineRenderer.positionCount = rGroupPositions.Count;
        for(int i = 0; i < rGroupPositions.Count; i++){
            lineRenderer.SetPosition(i, rGroupPositions[i].GetComponent<SkinnedMeshRenderer>().bounds.center);
        }
    }
}
