using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroMembrane : MonoBehaviour
{
    //LOCAL TRANSFORMATIONS
    public Vector3 position;
    public Quaternion rotation;
    private Vector3 membraneOffset;
    private Vector3 up;
    private Vector3 dirToHole;
    private Transform wholeCell; 
    private Transform heroGLP1R; //TARGET
    private float wholeCellRadius;
    private Vector3 startForward;

    public IntroMembrane(Transform wholeCell, float wholeCellRadius, Transform heroGLP1R, Vector3 membraneOffset)
    {
        //ASSGIN PRIVATE VARIABLES 
        this.wholeCell = wholeCell;
        this.wholeCellRadius = wholeCellRadius;
        this.heroGLP1R = heroGLP1R;
        this.membraneOffset = membraneOffset;
        this.startForward = wholeCell.forward;
        
        //GET POSITION 
        float angleToRotate = Vector3.Angle(wholeCell.forward, Vector3.down); 
        Vector3 crossAxis = Vector3.Cross(wholeCell.forward, Vector3.down);
        position = wholeCell.position + (Quaternion.AngleAxis(-angleToRotate, crossAxis) * membraneOffset);
        
        //GET ROTATION 
        up = (position - wholeCell.position).normalized;
        Vector3 centerOfFusion = wholeCell.position + (heroGLP1R.position - wholeCell.position).normalized * wholeCellRadius;
        dirToHole = (centerOfFusion - position).normalized;
        rotation = Quaternion.LookRotation(dirToHole, up);
    }

    public void UpdatePosition()
    {
        float angleToRotate = Vector3.Angle(startForward, heroGLP1R.position - wholeCell.position);
        Vector3 crossAxis = Vector3.Cross(startForward, heroGLP1R.position - wholeCell.position);
        position = wholeCell.position + (Quaternion.AngleAxis(angleToRotate, crossAxis) * membraneOffset);

        //CHECKS IF PLANE HAS INTERSECTED SPHERE
        Plane cellP = new Plane(heroGLP1R.forward, heroGLP1R.position);
        Vector3 pointOnPlane = cellP.ClosestPointOnPlane(position);

        //GET ROTATION 
        up = (position - wholeCell.position).normalized;
        Vector3 centerOfFusion = wholeCell.position + (heroGLP1R.position - wholeCell.position).normalized * wholeCellRadius;
        dirToHole = (centerOfFusion - position).normalized;
        rotation = Quaternion.LookRotation(dirToHole, up);  
        
    }
}
