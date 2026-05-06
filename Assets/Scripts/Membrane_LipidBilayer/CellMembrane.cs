using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellMembrane{
        //LOCAL TRANSFORMATIONS 
        public Vector3 position;
        public Quaternion rotation;
        private Vector3 membraneOffset;
        private Vector3 dirToVesicle;
        private Vector3 dirToHole;

        //PROPERTIES
        private float rotationSpeed;
        private float progressionSpeed;

        public CellMembrane(Transform cellPlane, Vector3 membraneOffset, float rotationSpeed, float progressionSpeed)
        { 
            position = cellPlane.position + cellPlane.right * membraneOffset.x + cellPlane.up * membraneOffset.z;

            dirToVesicle = cellPlane.forward;
            dirToHole = cellPlane.position - position;
            rotation = Quaternion.LookRotation(dirToHole, dirToVesicle);
            this.rotationSpeed = rotationSpeed;
            this.progressionSpeed = progressionSpeed;
            this.membraneOffset = membraneOffset;  
        }

        public void UpdateProperties(float rotationSpeed, float progressionSpeed)
        {
            this.rotationSpeed = rotationSpeed;
            this.progressionSpeed = progressionSpeed;
        }

        public void UpdatePosition(Vector3 vesicleSpherePos, float vesicleRadius, Transform cellPlane, float fusionIntersectionOffset)
        {
            position = cellPlane.position + cellPlane.right * membraneOffset.x + cellPlane.up * membraneOffset.z;
            Vector3 vectorToPlaneCenter = (cellPlane.position - position).normalized;
            rotation = Quaternion.LookRotation(vectorToPlaneCenter, cellPlane.forward);
            
            //CHECK IF SPHERE HAS INTERSECTED PLANE
            float distance = Vector3.Distance(position, vesicleSpherePos) - vesicleRadius + fusionIntersectionOffset;
            if(distance < 0)
            { //IF INTERSECTED 
                distance = Mathf.Abs(distance) ;
                
                //SET POSITION
                position = position - vectorToPlaneCenter * progressionSpeed * distance;

                //SET ROTATION 
                Vector3 right = Vector3.Cross(dirToVesicle, dirToHole);
                Vector3 localForward = Quaternion.AngleAxis(-Mathf.Clamp(distance * rotationSpeed, 0, 180), right) * this.dirToHole;
                rotation = Quaternion.LookRotation(localForward, Vector3.Cross(localForward, right));
            }
            else //IF NOT INTERSECTED
            {  
                //GET ROTATION 
                dirToVesicle = cellPlane.forward;
                dirToHole = cellPlane.position - position;
                rotation = Quaternion.LookRotation(dirToHole, dirToVesicle);
            }
        }
    }
