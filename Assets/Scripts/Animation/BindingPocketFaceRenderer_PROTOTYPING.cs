using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BindingPocketFaceRenderer_PROTOTYPING : MonoBehaviour
{
    [System.Serializable]
    public class SerializableTriangle{
        public List<Transform> face;
    }

    [SerializeField] 
    private Material faceMaterial;
    [SerializeField] 
    private Material edgeMaterial; 
    [SerializeField]
    private float edgeThickness;

    [SerializeField] 
    private List<SerializableTriangle> incomingTriangles;
    private Vector3[] vertices;
    private int[] triangles;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private LineRenderer lineRenderer;
    private Mesh mesh;
    int totalCount = 0;

    void FillArrays(){
        vertices = new Vector3[totalCount];
        triangles = new int[totalCount];
        int index = 0;
        foreach(SerializableTriangle t in incomingTriangles){
            foreach(Transform v in t.face){
                vertices[index] = v.GetComponent<SkinnedMeshRenderer>().bounds.center;
                triangles[index] = index;
                index++;
            }
        }
    }

    void Start(){
        //CREATE MESH
        foreach(SerializableTriangle t in incomingTriangles){
            foreach(Transform v in t.face){
                totalCount++;
            }
        }
        mesh = new Mesh();
        FillArrays();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        CreateFaces();
        CreateEdges();
    }

    public void CreateFaces(){
        GameObject bindingPocketMesh = new GameObject("bindPocketMesh");
        bindingPocketMesh.transform.parent = transform;
        meshFilter = bindingPocketMesh.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        meshRenderer = bindingPocketMesh.AddComponent<MeshRenderer>();
        meshRenderer.material = faceMaterial;
    }

    public void CreateEdges(){
        GameObject edge = new GameObject("Edge");
        edge.transform.parent = transform;
        lineRenderer = edge.AddComponent<LineRenderer>();
        lineRenderer.material = edgeMaterial;
        lineRenderer.startWidth = edgeThickness;
        lineRenderer.endWidth = edgeThickness;
        lineRenderer.positionCount = (vertices.Count()/3) *5;
        int index = 0;
        foreach(SerializableTriangle t in incomingTriangles){
            for(int i = 0; i < 3; i++){
                lineRenderer.SetPosition(index, t.face[i].GetComponent<SkinnedMeshRenderer>().bounds.center);
                index++;
            }
            lineRenderer.SetPosition(index, t.face[0].GetComponent<SkinnedMeshRenderer>().bounds.center);
            index++;
            lineRenderer.SetPosition(index, t.face[2].GetComponent<SkinnedMeshRenderer>().bounds.center);
            index++;
        }
    }

    public void Update(){
        UpdateFaces();
        UpdateEdges();
    }

    public void UpdateFaces(){
        FillArrays();
        meshFilter.mesh.vertices = vertices;
    }

    public void UpdateEdges(){
        int index = 0;
        foreach(SerializableTriangle t in incomingTriangles){
            for(int i = 0; i < 3; i++){
                lineRenderer.SetPosition(index, t.face[i].GetComponent<SkinnedMeshRenderer>().bounds.center);
                index++;
            }
            lineRenderer.SetPosition(index, t.face[0].GetComponent<SkinnedMeshRenderer>().bounds.center);
            index++;
            lineRenderer.SetPosition(index, t.face[2].GetComponent<SkinnedMeshRenderer>().bounds.center);
            index++;
        }
    }
}
