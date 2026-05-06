using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CellShadedAnimation : MonoBehaviour
{
    [SerializeField] List<GameObject> cellPrefabs;
    [SerializeField] List<GameObject> spawnedObjects;
    [SerializeField] int count;
    [SerializeField] float radius;
    [SerializeField] GameObject planeA;
    [SerializeField] GameObject planeB;
    [SerializeField] GameObject sphere;
    [SerializeField] float planeSensitivity;
    public enum ScaleType { BySphere, ByPlane}
    public ScaleType scaleType;

    // Start is called before the first frame update
    void Start()
    {
        //CreateSpheres();
    }

    private void CreateSpheres()
    {
        for (int x = 0; x < count; x++)
        {
            for (int y = 0; y < cellPrefabs.Count; y++)
            {
                spawnedObjects.Add(Instantiate(cellPrefabs[y]));
                spawnedObjects[x].transform.position = FindPosition(cellPrefabs[y].transform.localScale.x);
            }
        }
    }

    private void Update()
    {
        switch (scaleType)
        {
            case ScaleType.BySphere:
                AdjustScaleBySphere();
                break;
            case ScaleType.ByPlane:
                AdjustScaleByPlane();
                break;
        }
        
    }

    private void AdjustScaleBySphere()
    {
        foreach(GameObject g in spawnedObjects)
        {
            float distanceToSphereCenter = Vector3.Distance(g.transform.position, sphere.transform.position);
            float sphereRadius = sphere.transform.localScale.x / 2;
            if (distanceToSphereCenter > sphereRadius)
            {
                float scale = Mathf.Clamp(1 - planeSensitivity * (distanceToSphereCenter - sphereRadius), 0, 1);
                g.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }

    private void AdjustScaleByPlane()
    {
        Plane pA = new Plane(planeA.transform.forward, planeA.transform.position);
        Plane pB = new Plane(planeB.transform.forward, planeB.transform.position);

        foreach (GameObject g in spawnedObjects) { 
            float dotA = Vector3.Dot(planeA.transform.forward, g.transform.position - planeA.transform.position);
            float dotB = Vector3.Dot(planeB.transform.forward, g.transform.position - planeA.transform.position);
        if (dotA > 0)
            {
            float distanceToPlane = pA.GetDistanceToPoint(g.transform.position);
                   
            float scale = Mathf.Clamp(1 - planeSensitivity * distanceToPlane, 0, 1);
            g.transform.localScale = new Vector3(scale, scale, scale);
            }
            else
            {
                g.transform.localScale = Vector3.one;
                if (dotB > 0)
                {
                    float distanceToPlane = pB.GetDistanceToPoint(g.transform.position);

                    float scale = Mathf.Clamp(1 - planeSensitivity * distanceToPlane, 0, 1);
                    g.transform.localScale = new Vector3(scale, scale, scale);
                }

                
            }
        }
       
    }

    private Vector3 FindPosition(float incomingRadius)
    {
        Vector3 position = Vector3.zero;
        bool positionCleared = false;
        while (positionCleared == false)
        {
            position = Random.insideUnitSphere * radius;
            bool overlapping = false;
            foreach (GameObject s in spawnedObjects)
            {
                if (Vector3.Distance(s.transform.position, position) < s.transform.localScale.x + incomingRadius)
                {
                    overlapping = true;
                }
            }
            if(overlapping == false)
            {
                positionCleared = true;
            }
        }
        return position;
    }
}
