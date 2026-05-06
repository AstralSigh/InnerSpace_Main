using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotManager : MonoBehaviour
{
    public List<Transform> TPPivots;
    public Transform currentPivot; 

    public void Update()
    {
        transform.position = currentPivot.position;
        transform.rotation = currentPivot.rotation;
    }

    public void UpdatePivot(TutorialInputs.TPPivot pivot)
    {
        currentPivot = TPPivots[(int)pivot];
        transform.localScale = currentPivot.localScale;
    }
}
