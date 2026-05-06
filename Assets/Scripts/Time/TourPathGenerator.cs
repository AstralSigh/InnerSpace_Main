using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TourPathGenerator : MonoBehaviour
{
    public Transform splineTarget;
    public Spline spline;
    public float t;

    void Start(){
        spline = splineTarget.GetComponent<Spline>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = SplineUtility.EvaluatePosition(spline, t);
        transform.forward = SplineUtility.EvaluateTangent(spline, t);
    }
}
