using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VesicleMembraneFusionInstancer : MonoBehaviour
{
    public class Membrane{
        //LOCAL TRANSFORMATIONS 
        public bool releaseUp;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 startPosition;
        private Vector3 startRight;
        public Vector3 startUp;
        private Vector3 startForward;
        private Quaternion startRotation;

        //REFERENCES
        private Transform spawnLocation;
        private Transform instancer;

        //PROPERTIES
        private float rotationSpeed = 90;
        private float forwardMultiplier = 10;
        private float upMultiplier = 1;
        private float distanceToPlane;

        public Membrane(Vector3 startPosition, Vector3 startForward, Vector3 startUp, Transform spawnLocation, Transform instancer, bool releaseUp)
        {
            this.startPosition = startPosition;
            this.startForward = startForward;
            this.startUp = startUp;
            if (releaseUp)
            {
                startUp = -startUp;
            }
            this.spawnLocation = spawnLocation;
            this.instancer = instancer;
            this.releaseUp = releaseUp;
            startRight = Vector3.Cross(startUp, startForward);
            startRotation = Quaternion.LookRotation(startForward, startUp);
            rotation = startRotation;
            
        }

        public void UpdateProperties(float rotationSpeed, float forwardMultiplier, float upMultiplier)
        {
            this.rotationSpeed = rotationSpeed;
            this.forwardMultiplier = forwardMultiplier;
            this.upMultiplier = upMultiplier;
        }

        public void UpdatePosition(float fusionAmount, Vector3 positionOffset)
        {
            Vector3 dirToCenter = (instancer.position - spawnLocation.position);
            Plane cellPlane = new Plane(dirToCenter, spawnLocation.position + dirToCenter.normalized * fusionAmount);

            //CHECKS IF PLANE HAS INTERSECTED MEMBRANE
            if (Vector3.Dot(startUp, cellPlane.ClosestPointOnPlane(startPosition) - startPosition) < 0)
            {
                distanceToPlane = Mathf.Abs(cellPlane.GetDistanceToPoint(startPosition));
                position = startPosition + startForward * -distanceToPlane * forwardMultiplier + startUp * distanceToPlane * upMultiplier + positionOffset;

                Vector3 localForward = Quaternion.AngleAxis(-Mathf.Clamp(distanceToPlane * rotationSpeed, 0, 180), startRight) * startForward;
                rotation = Quaternion.LookRotation(localForward, Vector3.Cross(localForward, startRight));


                //Debug.DrawRay(position, localForward, Color.green);
                //Debug.DrawRay(position, startRight, Color.red);
            }
            else
            {
                position = startPosition + positionOffset;
                rotation = startRotation;               
            }
        }

        public void UpdateForwardAxis(Vector3 forward)
        {
            this.startForward = forward;
            startRotation = Quaternion.LookRotation(startForward, startUp);
            startRight = Vector3.Cross(startUp, startForward);
        }
    }

    //MATH
    float goldenRatio;
    float angleIncrement;

    //INSTANCING 
    public int membraneCount;
    public float radius = 125;
    public bool releaseUp;
    public bool customizeOffset; 
    public Mesh membrane;
    public Material membraneMaterial;
    List<Membrane> membraneList = new List<Membrane>();
    List<List<Matrix4x4>> renderBatches = new List<List<Matrix4x4>>();
    public int batchCount = 0;
    public bool resetMembrane = false;
    public bool resetProperties = false;
    public int subMeshIndex = 0;
    public bool isRift;

    //MEMBRANE PROPERTIES
    public float rotationSpeed;
    public float upMultiplier;
    public float forwardMultiplier;
    public float spawnRadius;
    private Vector3 cellMembranePositionOffset;

    //REFRENCES 
    //public Transform cellMembrane;
    public Transform spawnPoint;
    public Transform cellMembranePositionOffsetPoint;
    public float fusionAmount;
    public float r1;

    void Start()
    {
        //SET UP MATH
        goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        angleIncrement = Mathf.PI * 2 * goldenRatio;

        //SET UP MEMBRANE
        CreateMembrane();
        UpdateProperties();
        //ApplyMovementAndUpdateRenderBatches

        //AUTOMATICALLY SETS TARGET IN MATERIAL. RIGHT NOW THERE IS A WEIRD OFFSET WHERE _TARGET IS NOT PERFECTLY CENTERED TO MEMBRANE MOTION. SO PLUGGING THE VALUE MANUALLY
        //transform.GetComponent<Renderer>().sharedMaterial.SetVector("_Target", transform.position + (spawnPoint.position - transform.position).normalized * radius);
    }

    private void Update()
    {
        //RESET MEMBRANE DURING RUNTIME
        if (resetMembrane)
        {
            membraneList.Clear();
            CreateMembrane();
            UpdateProperties();
            resetMembrane = false;
        }

        //RESET PROPERTIES DURING RUNTIME
        if (resetProperties)
        {
            UpdateProperties();
            resetProperties = false;
        }

        ApplyMovementAndUpdateRenderBatches();

        //RENDER ALL BATCHES
        RenderAllBatches();

        //UPDATE MATERIAL PROPERTIES AT RUNTIME TO CONTROL SIZE OF HOLE IN MEMBRANE 
        //if(!isRift){
            transform.GetComponent<Renderer>().sharedMaterial.SetFloat("_HoleRadius", r1);
        //}

        //DEBUG
        //DrawPlane(spawnPoint.position + dirToCenter.normalized * fusionAmount, dirToCenter);
    }

    public void ApplyMovementAndUpdateRenderBatches()
    {
        batchCount = 0;
        renderBatches.Clear();
        renderBatches.Add(new List<Matrix4x4>());
        
        if(customizeOffset){
            float offsetDistance = Mathf.Clamp(( Vector3.Distance(Vector3.zero, cellMembranePositionOffsetPoint.position) - Vector3.Distance(Vector3.zero, spawnPoint.position)), 0, float.MaxValue);
            Vector3 offsetAngle = spawnPoint.position.normalized;
            cellMembranePositionOffset =  offsetAngle * offsetDistance;
        }
        
        foreach(Membrane m in membraneList)
        {
            if(customizeOffset){ //if cell membrane
                m.UpdatePosition(fusionAmount - (radius - 137), cellMembranePositionOffset); 
            }
            else{
                m.UpdatePosition(fusionAmount - (radius - 137), Vector3.zero);  
            }

            if (renderBatches[batchCount].Count >= 999)
            {
                batchCount++;
                renderBatches.Add(new List<Matrix4x4>()); 
            }
            renderBatches[batchCount].Add(Matrix4x4.TRS(m.position, m.rotation, Vector3.one));
        }
    }

    public void RenderAllBatches()
    {
        foreach(List<Matrix4x4> batch in renderBatches)
        {
            for(int x = 0; x < membrane.subMeshCount; x++)
            {
                Graphics.DrawMeshInstanced(membrane, x, membraneMaterial, batch);
            }
        }
    }

    public void DrawPlane(Vector3 position, Vector3 normal)
    {
        Vector3 v3;
        if (normal.normalized != Vector3.forward)
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        else
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude; ;
        var corner0 = position + v3;
        var corner2 = position - v3;
        var q = Quaternion.AngleAxis(90.0f, normal);
        v3 = q * v3;
        var corner1 = position + v3;
        var corner3 = position - v3;
        Debug.DrawLine(corner0, corner2, Color.green);
        Debug.DrawLine(corner1, corner3, Color.green);
        Debug.DrawLine(corner0, corner1, Color.green);
        Debug.DrawLine(corner1, corner2, Color.green);
        Debug.DrawLine(corner2, corner3, Color.green);
        Debug.DrawLine(corner3, corner0, Color.green);
        Debug.DrawRay(position, normal, Color.red);
    }

    void CreateMembrane()
    {
        for (int i = 0; i < membraneCount; i++)
        {
            //COVERTS SPAWN RADIUS INTO RATIO FOR SPHERE INSTANTIATION
            float spawnRatioOfSphere = (Mathf.PI * Mathf.Pow(spawnRadius, 2)) / (4 * Mathf.PI * Mathf.Pow(radius, 2));

            //GENERATES POINTS ON A SPHERE
            float t = ((float)i / membraneCount) * spawnRatioOfSphere;
            float inclination = Mathf.Acos(1 - 2 * t);
            float azimuth = angleIncrement * i;
            float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            float z = Mathf.Cos(inclination);

            //OFFSETS POINTS ON SPHERE TO THE DIRECTION OF SPAWN POINT 
            Vector3 offsetToSpawnPoint = (spawnPoint.position - transform.position);
            Vector3 position = transform.position + new Vector3(x, y, z) * radius;
            Vector3 crossAxis = Vector3.Cross(transform.forward, offsetToSpawnPoint);
            Vector3 startPosition = Quaternion.AngleAxis(Vector3.Angle(transform.forward, offsetToSpawnPoint), crossAxis) * (position - transform.position) + transform.position;

            //CALCULATES DIRECTION VECTORS BASED ON CELL MEMBRANE
            Vector3 startUp = (startPosition - transform.position).normalized;
            Plane membranePlane = new Plane(startUp, startPosition);
            Vector3 pointOnMembranePlane = membranePlane.ClosestPointOnPlane(spawnPoint.position);
            Vector3 startForward = (pointOnMembranePlane - startPosition);

            //CREATE MEMBRANE IF FORWARD IS NOT 0
            if (Vector3.Distance(startForward, Vector3.zero) > 0.1f)
            {
                membraneList.Add(new Membrane(startPosition, startForward.normalized, startUp, spawnPoint, this.transform, releaseUp));
            }
            //Debug.Log(Vector3.Angle(spawnPoint.position - transform.position, transform.forward));
        }
    }

    //UPDATES THE FORWARD AXIS OF MEMBRANE BASED ON CELL MEMBRANE POSITION
    void UpdateForwardAxis()
    {
        foreach(Membrane m in membraneList)
        {
            Plane membranePlane = new Plane(m.startUp, m.startPosition);
            Vector3 pointOnMembranePlane = membranePlane.ClosestPointOnPlane(spawnPoint.position);
            Vector3 startForward = (pointOnMembranePlane - m.startPosition).normalized;
            m.UpdateForwardAxis(startForward);
        }
    }

    //UPDATES PROPERTIES OF MEMBRANE
    void UpdateProperties()
    {
        foreach (Membrane m in membraneList)
        {
            m.UpdateProperties(rotationSpeed, forwardMultiplier, upMultiplier);
        }
    }

}
