using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorExploration : MonoBehaviour
{
    [System.Serializable]
    public class Membrane{
        public Vector3 right;
        public Vector3 down;
        public Vector3 forward;
        private Vector3 startingUp;
        private Vector3 startingUp2;
        private Vector3 startingForward;
        public Vector3 position;
        public Vector3 cachedPosition;
        public Vector3 startPosition;
        public Quaternion rotation;
        private Transform target;
        public float localXRotation = 0;
        private Membrane before;
        private Membrane before2;
        private float membraneDensity;
        private float spawnOffset;
        private Plane lockedAxis;
        private Plane positionPlane;
        public Membrane(Vector3 startPosition, Transform target, float membraneDensity){
            this.position = startPosition;
            this.startPosition = startPosition;
            this.target = target;
            forward = (target.position - position).normalized;
            startingForward = forward;
            right = Vector3.Cross(forward, target.up).normalized; 
            startingUp = target.up;
            //Debug.DrawRay(position, forward, Color.red, 15f);
            //Debug.DrawRay(position, target.up, Color.cyan, 15f);
            //Debug.DrawRay(position, Vector3.Cross(forward, -right).normalized, Color.blue, 15f);
            this.membraneDensity = membraneDensity;
            spawnOffset = Random.Range(-.5f,.5f) * membraneDensity;
            lockedAxis = new Plane(right, startPosition);
            positionPlane = new Plane(-startingUp, position);
            rotation = Quaternion.LookRotation(forward, startingUp);   
        }

        public void setUpCellMembrane(){
            forward = -forward.normalized;
        }

        public void manageCellMovment(float radius, Vector3 target)
        {
            rotation = Quaternion.LookRotation(forward, startingUp);
            position = startPosition + forward * radius - startingUp.normalized * positionPlane.GetDistanceToPoint(target);
            position = lockedAxis.ClosestPointOnPlane(position) + (right.normalized*spawnOffset);
        }

        public void manageMovement(){
                        
            if(before!=null)
            {
                //Update forward vector with localXRotation (Rotation to right axis)
                forward = (Quaternion.AngleAxis(localXRotation,right) * before.forward).normalized;

                //Make sure rotation caps at 180 degrees 
                if(Vector3.Dot(forward,  startingUp) < -.001f){
                    forward = -startingForward;
                    //Debug.DrawRay(position, forward, Color.blue);
                    //Debug.DrawRay(position, startingUp, Color.green);
                    //Plane startingSurface = new Plane(startingUp, startPosition);    //used to hardcode distance to other membrane.
                    //Debug.Log(startingSurface.GetDistanceToPoint(position));
                }
                
                position = before.position + forward * membraneDensity;
                position = lockedAxis.ClosestPointOnPlane(position) + (right.normalized*spawnOffset);
            }

        down= Vector3.Cross(forward,right);
        rotation = Quaternion.LookRotation(forward, down);
        //Debug.DrawRay(position, right, Color.red);
        //Debug.DrawRay(position, startingUp, Color.green);
        //Debug.DrawRay(position, forward, Color.blue);
        }        
        
        public void assignBefore(Membrane before, Membrane before2){
            this.before = before;
            this.before2 = before2;
        }
    }

    //GPU INSTANCING 
    public float scale = 1;
    public Mesh membrane;
    public Material membraneMaterial;
    public List<List<Matrix4x4>> batches = new List<List<Matrix4x4>>();
    private int batchIndex = 0;

    //public List<Membrane> spoke = new List<Membrane>();
    [SerializeField]
    public List<List<Membrane>> spokes = new List<List<Membrane>>();
    public List<List<Membrane>> rows = new List<List<Membrane>>();
    public List<List<Membrane>> rowsCell = new List<List<Membrane>>();
    public List<Membrane> rowVisualized = new List<Membrane>();
    public GameObject pivot;
    public int membraneCount = 20;
    public int spokeCount = 20;
    public int rowCount = 20;
    public float membraneDensity = 1f;
    private int rotationIndex = 0;
    public int xRotationCap = 40;
    public bool createNewMembrane = false;
    public float centerRadiusOffset = 0.1f;
    public GameObject membraneBackground;
    public Transform sphereGenerator;
    public float globalRadius;

    void Start()
    {
        CreateMembrane();

    }

    // Update is called once per frame
    void Update()
    {
        if(createNewMembrane)
        {
            CreateMembrane();
            //CreateCellMembrane();
            createNewMembrane = false;
            rotationIndex = 0;
        }

        batches.Clear();
        batchIndex = 0;
        batches.Add(new List<Matrix4x4>());

        //MOVEMENT
            foreach(List<Membrane> row in rows){
                foreach(Membrane m in row)
                {
                    m.manageMovement();
                }   
            }
            
            Plane targetPlane = new Plane(rows[0][0].forward, transform.position);
            float vesicleMembraneRadius = Mathf.Clamp(targetPlane.GetDistanceToPoint(rows[0][0].position) - centerRadiusOffset, 0, float.MaxValue);

            foreach(List<Membrane> row in rowsCell){
                foreach(Membrane m in row)
                {
                    m.manageCellMovment(vesicleMembraneRadius, rows[0][0].position);
                }   
            }

            //membraneBackground.GetComponent<Renderer>().sharedMaterial.SetFloat("_HoleRadius", vesicleMembraneRadius);

            Debug.Log("Movement Managed");
            AddRotation();
        

        //RENDERING
        foreach(List<Membrane> row in rows){
            foreach(Membrane m in row){
                if(batches[batchIndex].Count > 999)
                {
                    batchIndex++;
                    batches.Add(new List<Matrix4x4>());
                }
                batches[batchIndex].Add(item: Matrix4x4.TRS
                    (pos: m.position, m.rotation, Vector3.one * scale));
            }
        }

        foreach(List<Membrane> row in rowsCell){
            foreach(Membrane m in row){
                if(batches[batchIndex].Count > 999)
                {
                    batchIndex++;
                    batches.Add(new List<Matrix4x4>());
                }
                batches[batchIndex].Add(item: Matrix4x4.TRS
                    (pos: m.position, m.rotation, Vector3.one * scale));
            } 
        }

        RenderBatches();
        //rowVisualized = rows[5];
    }

public void CreateCellMembrane()
    {
        rowsCell.Clear();

        //LOOPS THORUGH EVERY ROW
        for(int rowIndex = 0; rowIndex < rowCount; rowIndex++){
            float radius = rowIndex * membraneDensity + centerRadiusOffset;
            int membranePerRow = (int)(2 * Mathf.PI * radius/membraneDensity);
            rowsCell.Add(new List<Membrane>());

            //CREATES MEMBRANE IN ROW
            for(int membraneIndex = 0; membraneIndex < membranePerRow; membraneIndex++){
                float angle = 2 * Mathf.PI * (float)membraneIndex/ (float)membranePerRow;
                
                Vector3 spawnLocation = 
                    transform.position +
                    transform.right.normalized * Mathf.Sin(angle) * radius +
                    transform.up * Mathf.Sin((Mathf.PI/2f) * ((float)rowIndex/ (float)rowCount)) + (pivot.transform.position - transform.position) +
                    transform.forward.normalized * Mathf.Cos(angle) * radius;

                Membrane newMembrane = new Membrane(spawnLocation, pivot.transform, membraneDensity);
                rowsCell[rowIndex].Add(newMembrane);
            }
        }

        for(int rowIndex = rowsCell.Count -1; rowIndex > 0; rowIndex--){
            
            for(int membraneIndex = 0; membraneIndex < rowsCell[rowIndex].Count; membraneIndex++){
                Membrane current = rowsCell[rowIndex][membraneIndex];
                
                rowsCell[rowIndex-1].Sort((a, b) => 
                (Vector3.Distance(a.position, current.position).
                CompareTo(Vector3.Distance(b.position, current.position))));

                current.assignBefore(rowsCell[rowIndex-1][0], rowsCell[rowIndex-1][0]);
                current.setUpCellMembrane();
            }
        } 
    }


    public void CreateMembrane()
    {
        globalRadius = rowCount * membraneDensity + centerRadiusOffset;

        rows.Clear();
        //LOOPS THORUGH EVERY ROW
        for(int rowIndex = 0; rowIndex < rowCount; rowIndex++){
            float radius = rowIndex * membraneDensity + centerRadiusOffset;
            int membranePerRow = (int)(2 * Mathf.PI * radius/membraneDensity);
            rows.Add(new List<Membrane>());

            //CREATES MEMBRANE IN ROW
            for(int membraneIndex = 0; membraneIndex < membranePerRow; membraneIndex++){
                float angle = 2 * Mathf.PI * (float)membraneIndex/ (float)membranePerRow;
                
                Vector3 localVectorA = transform.position - Vector3.right * ((float)rowIndex/ rowCount) * membraneDensity;
                float distanceToSphereCenter = Mathf.Abs(Vector3.Distance(localVectorA, sphereGenerator.position) - 503f);

                Vector3 spawnLocation = 
                    transform.position +
                    transform.right.normalized * Mathf.Sin(angle) * radius -
                    transform.up * distanceToSphereCenter
                    + 
                    transform.forward.normalized * Mathf.Cos(angle) * radius;

                Membrane newMembrane = new Membrane(spawnLocation, this.transform,membraneDensity);



                rows[rowIndex].Add(newMembrane);
            }
        }


        //transform.up * -Mathf.Sin((Mathf.PI/2f) * ((float)rowIndex/ (float)rowCount)) 
        for(int rowIndex = 0; rowIndex < rows.Count -1; rowIndex++){
            
            for(int membraneIndex = 0; membraneIndex < rows[rowIndex].Count; membraneIndex++){
                Membrane current = rows[rowIndex][membraneIndex];
                
                rows[rowIndex+1].Sort((a, b) => 
                (Vector3.Distance(a.position, current.position).
                CompareTo(Vector3.Distance(b.position, current.position))));

                current.assignBefore(rows[rowIndex+1][0], rows[rowIndex+1][1]);
            }
        } 
    }

    public void AddRotation(){        
        if(rotationIndex < rowCount-1){

            foreach(Membrane m in rows[rotationIndex])
            {
                m.localXRotation += Time.deltaTime * 20;
                if(m.localXRotation > xRotationCap){
                    rotationIndex ++;
                    break;
                } 
            }
            
        } 
    }

    void RenderBatches()
    {
        for(int b = 0; b < batches.Count; b++){
            for (int i = 0; i < membrane.subMeshCount; i++)
            {
                Graphics.DrawMeshInstanced(membrane, i, membraneMaterial, batches[b]);
            }
        }
    }
}
