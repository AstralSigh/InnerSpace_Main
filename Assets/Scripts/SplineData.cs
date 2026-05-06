using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;




[ExecuteInEditMode]
public class SplineData : MonoBehaviour
{
    [SerializeField] 
    private SplineContainer spline;
    [SerializeField]
    private List<TourStop_Prototyping> tourStops;    

    public List<TourStop_Prototyping> GetTourStops(){
        return tourStops;
    }

}