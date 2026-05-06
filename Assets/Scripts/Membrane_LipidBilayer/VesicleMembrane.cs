using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesicleMembrane{
        //LOCAL TRANSFORMATIONS 
        public Vector3 position;
        public Quaternion rotation;
        private Vector3 membraneOffset;
        private Vector3 dirAwayVesicle;
        private Vector3 dirToHole;
        private Transform vesicleSphere; 
        private Transform cellPlane; 
        private float vesicleRadius;

        //PROPERTIES
        private float rotationSpeed;
        private float progressionSpeed;

        public VesicleMembrane(Transform vesicleSphere, float vesicleRadius, Transform cellPlane, Vector3 membraneOffset, float rotationSpeed, float progressionSpeed)
        {
            //ASSGIN PRIVATE VARIABLES 
            this.vesicleSphere = vesicleSphere;
            this.vesicleRadius = vesicleRadius;
            this.cellPlane = cellPlane;
            this.membraneOffset = membraneOffset;
            this.rotationSpeed = rotationSpeed;
            this.progressionSpeed = progressionSpeed;
            
            //GET POSITION 
            position = vesicleSphere.position + vesicleSphere.forward * membraneOffset.z + vesicleSphere.right * membraneOffset.x + vesicleSphere.up * membraneOffset.y;

            //GET ROTATION 
            dirAwayVesicle = (position - vesicleSphere.position).normalized;
            Vector3 centerOfFusion = vesicleSphere.position + (cellPlane.position - vesicleSphere.position).normalized * vesicleRadius;
            dirToHole = (centerOfFusion - position).normalized;
            rotation = Quaternion.LookRotation(dirToHole, dirAwayVesicle);
        }

        public void UpdateProperties(float rotationSpeed, float progressionSpeed)
        {
            this.rotationSpeed = rotationSpeed;
            this.progressionSpeed = progressionSpeed;
        }

        public void UpdatePosition(float fusionIntersectionOffset)
        {
            //GET POSITION 
            position = vesicleSphere.position + vesicleSphere.forward * membraneOffset.z + vesicleSphere.right * membraneOffset.x + vesicleSphere.up * membraneOffset.y;

            //CHECKS IF PLANE HAS INTERSECTED SPHERE
            Plane cellP = new Plane(cellPlane.forward, cellPlane.position);
            Vector3 pointOnPlane = cellP.ClosestPointOnPlane(position);
            float membraneDistance = Vector3.Distance(position, vesicleSphere.position);
            float planeDistance = Vector3.Distance(pointOnPlane, vesicleSphere.position);
            float distance = planeDistance - membraneDistance + fusionIntersectionOffset;
            if (distance < 0) //IF INTERSECTED 
            {                
                distance = Mathf.Abs(distance);
                
                //SET POSITION 
                Vector3 targetDir = (pointOnPlane - cellPlane.position).normalized;
                position = position + targetDir * distance * progressionSpeed;
                
                //SET ROTATION 
                Vector3 right = Vector3.Cross(dirAwayVesicle, dirToHole);
                Vector3 localForward = Quaternion.AngleAxis(-Mathf.Clamp(distance * rotationSpeed, 0, 180), right) * this.dirToHole;
                rotation = Quaternion.LookRotation(localForward, Vector3.Cross(localForward, right));
            }
            else //IF NOT INTERSECTED
            {
                //GET ROTATION 
                dirAwayVesicle = (position - vesicleSphere.position).normalized;
                Vector3 centerOfFusion = vesicleSphere.position + (cellPlane.position - vesicleSphere.position).normalized * vesicleRadius;
                dirToHole = (centerOfFusion - position).normalized;
                rotation = Quaternion.LookRotation(dirToHole, dirAwayVesicle);  
            }
        }
    }