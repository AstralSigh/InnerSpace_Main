using System.Collections;
using System.Collections.Generic;
using UnityEngine.Splines;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SplineDataVisualizer_Prototyping : MonoBehaviour
{
#if UNITY_EDITOR
[CustomEditor(typeof(SplineDataVisualizer_Prototyping))]
public class ExampleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SplineDataVisualizer_Prototyping myScript = (SplineDataVisualizer_Prototyping)target;
        
        if(GUILayout.Button("Visualize tour stops"))
        {
            myScript.SetupTourStops();
        }
        if(GUILayout.Button("Generate Prefabs"))
        {
            myScript.SetupVisualFocusObjects();
        }
    }
}

    [SerializeField] 
    private SplineContainer spline;
    [SerializeField]
    private SplineData splineData;
    private List<TourStop_Prototyping> tourStops;
    [SerializeField] private GameObject prefabMesh;
    private GameObject[] handles;
    [SerializeField] private bool useStuff = false;


void OnDrawGizmos(){
        if(useStuff == true){

            Handles.color = Color.red;
            int index = 0;
            foreach(TourStop_Prototyping t in tourStops){
            Vector3 tourStopPosition = spline.EvaluatePosition(t.splineEndTime);
            Gizmos.DrawSphere(tourStopPosition, 0.1f);
            Handles.Label(tourStopPosition + Vector3.up * 0.1f + Vector3.right * 0.3f, t.text);
            
            float dotProduct = Vector3.Dot(spline.EvaluateTangent(t.splineEndTime), handles[index].transform.position - tourStopPosition);
            if(dotProduct > 0){
                t.splineEndTime += 0.001f;
            }
            else if(dotProduct < 0){
                t.splineEndTime -= 0.001f;
            }
            if(dotProduct < -.01){
                handles[index].transform.position = tourStopPosition;
            }
            
            index++;
            }   
        }
        
    }

    void OnSceneGUI(){
                    

    }

    public void OnStart(){
        SetupTourStops();
        foreach(GameObject h in handles){
            h.SetActive(false);
        }
    }

    public void SetupTourStops(){
        tourStops = splineData.GetTourStops();
        handles = new GameObject[splineData.GetTourStops().Count];
        int index = 0;
        foreach(TourStop_Prototyping t in tourStops){
            GameObject handle = Instantiate(prefabMesh);
            handle.name = t.text;
            handle.transform.position = spline.EvaluatePosition(t.splineEndTime);
            handles[index] = handle;
            index++;
        }
    }

    public void OnDisable(){
        foreach(GameObject h in handles){
            Destroy(h);
        }
        System.Array.Clear(handles, 0, handles.Length);
    }

    public void SetupVisualFocusObjects(){
        //GENERATE NEW GAME OBJECTS
        int index = 0;
        foreach(TourStop_Prototyping t in tourStops){
            Vector3 tourStopPosition = spline.EvaluatePosition(t.splineEndTime);
            //GameObject currentObject = new GameObject(index + ". " +  t.text + " Visual Focus");
            GameObject currentObject = Instantiate(prefabMesh);
            currentObject.transform.position = tourStopPosition; // + Vector3.up * 0.2f + Vector3.forward * 0.2f
            currentObject.transform.parent = this.transform;
            currentObject.name = t.text;
            t.visualFocus = currentObject.transform;
            index++;
        }
    }
    #endif
}
