using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
public class Spline_Prototyping : MonoBehaviour
{
    [SerializeField]SplineAnimate spline;
    bool test = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        if(!test){
            //spline.Play();
            //spline.MaxSpeed = 0.1f;
            test = true;
            spline.NormalizedTime = 0.5f;
        }
        
        Debug.Log(spline.NormalizedTime);
        if(spline.NormalizedTime > 0){
            spline.Pause();
            spline.NormalizedTime -= Time.deltaTime * 0.1f;
        }
    }
}
