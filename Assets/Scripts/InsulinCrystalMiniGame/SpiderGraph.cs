using System;
using UnityEngine;
using UnityEngine.UI;

public class SpiderGraph : MonoBehaviour
{
    public int targetLayer = 3;
    public float targetDiameter = 3.5f;
    public int throwAttempts = 5;
    public int targetHexamerCount = 20;

    public Vector3 subtextPositionOffset;
    public GameObject subtext;
    public GameObject[] headers;
    private float[] spiderData;
    private float[] cd;

    public float radius = 5f;
    Vector3[] connectionPoints = new Vector3[7];
    public LineRenderer lineRenderer;
    public Material material;

    private void OnEnable()
    {
        cd = CrystalManager.Instance.GetCrystalData();
        spiderData = new float[6];
        //SPEED (total hexamers / target hexamers)
        spiderData[0] = Mathf.Clamp(cd[0] / targetHexamerCount, 0, 1); ;
        //SYMMETRY (regular hexamers/ total hexamers)
        spiderData[1] = cd[1] / cd[0]; 
        //LAYERS (current layer / target layer)
        spiderData[2] = Mathf.Clamp(cd[2]/ targetLayer, 0, 1);
        //DIAMETER (diameter / target diameter)
        spiderData[3] = Mathf.Clamp(cd[3]/ targetDiameter, 0 , 1);
        //STABILITY (stable hexamers / total hexamers)
        spiderData[4] = cd[4] / cd[0];
        //ACURACY (boundaryCollisionCount / throw attempts)
        spiderData[5] = 1 - Mathf.Clamp(cd[5] / throwAttempts, 0, 1);
        for(int x = 0; x < 6; x++)
        {
            float angle = ((Mathf.PI * x) / 3f);
            connectionPoints[x] = new Vector3(Mathf.Cos(angle) * radius * spiderData[x], Mathf.Sin(angle) * radius * spiderData[x], 0);
        }
        connectionPoints[6] = connectionPoints[0];
        //lineRenderer.positionCount = connectionPoints.Length;
        //lineRenderer.SetPositions(connectionPoints);
        CreateCircularMesh();
        GenerateText();
    }

    public void GenerateText()
    {
        float time = CrystalGameManager.Instance.GetTime();
        TimeSpan timeB = TimeSpan.FromSeconds((double)time);
            
        //UPDATE SYMMETRY 
        //UPDATE DIAMETER 

        string[] td = new string[]{
            "You placed " + Convert.ToInt32(cd[0]) + " hexamers over " + timeB.ToString("mm':'ss") + "; your average speed was " + Convert.ToInt32(time/ cd[0]) + " seconds per hexamer",
            "Your crystal is " + Convert.ToInt32(spiderData[1] * 100f) + "% symmetrical",
            "The crystal you made was " + Convert.ToInt32(cd[2]) + " hexamers deep",
            "The crystals diameter is " + (cd[3] * 10f).ToString("F2") + "nm wide",
            Convert.ToInt32(cd[4]) + "/ " +Convert.ToInt32(cd[0]) + " hexamers you placed " + Convert.ToInt32(spiderData[4] * 100f) + "% are stable, meaning they have 3 or more points of connection.",
            cd[5] + " hexamers you moved fell out of bounds"
        };

        for(int x = 0; x < 6; x++){
            headers[x].GetComponent<Text>().text = td[x];
        }
    }

    void CreateCircularMesh()
    {
        // Create a new GameObject to hold the mesh
        GameObject circularMeshObject = new GameObject("CircularMesh");
        MeshFilter mf = circularMeshObject.AddComponent<MeshFilter>();
        MeshRenderer mr = circularMeshObject.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        mf.mesh = mesh;
        mr.material = material;

        // Vertices: Include the connection points and the center (Vector3.zero)
        Vector3[] vertices = new Vector3[connectionPoints.Length + 1];
        for (int i = 0; i < connectionPoints.Length; i++)
        {
            vertices[i] = connectionPoints[i];
        }
        vertices[connectionPoints.Length] = Vector3.zero;

        // Triangles: Form triangles using adjacent points and the center
        int[] triangles = new int[connectionPoints.Length * 3];
        for (int i = 0; i < connectionPoints.Length; i++)
        {
            triangles[i * 3] = i;
            triangles[i * 3 + 1] = (i + 1) % connectionPoints.Length;
            triangles[i * 3 + 2] = connectionPoints.Length;
        }

        // Normals: Set all normals to face outward
        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            normals[i] = -Vector3.forward;
        }

        // UVs: You can set UVs if needed
        Vector2[] uv = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            uv[i] = new Vector2(vertices[i].x, vertices[i].y);
        }

        // Assign data to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;  
        circularMeshObject.transform.position = this.transform.position;
        circularMeshObject.transform.rotation = this.transform.GetChild(0).rotation;
        circularMeshObject.transform.SetParent(this.transform);
        transform.GetComponent<LookAtCamera>().enabled = true;
    }

}
