
/*
#if UNITY_EDITOR 
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
//using UnityEditor.Animations;

using UnityEditor.Animations;

public class RecordTransformHierarchy : MonoBehaviour
{
    
    public AnimationClip clip;
    //public InsulinSavedEventTimes _savedTime;
    private GameObjectRecorder m_Recorder;

    public bool SaveRecording = false;
    private float saveStartTime;

    public void StartRecording()
    {
        clip = new AnimationClip();
        AssetDatabase.CreateAsset(clip, "Assets/Animation/InsulinCrystalTool/CrystalAnim.anim");
        // Create recorder and record the script GameObject.
        m_Recorder = new GameObjectRecorder(gameObject);

        // Bind all the Transforms on the GameObject and all its children.
        m_Recorder.BindComponentsOfType<Transform>(gameObject, true);

        Debug.Log("Animation recording started");
    }

    void LateUpdate()
    {
        if (clip == null)
            return;

        // Take a snapshot and record all the bindings values for this frame.
        m_Recorder.TakeSnapshot(Time.deltaTime);

        if (SaveRecording)
        {
            Debug.Log("Animation save started");
            saveStartTime = Time.time;
            saveRecording();
            SaveRecording = false;
        }
    }

    private void saveRecording()
    {
        if (clip == null)
            return;

        if (m_Recorder.isRecording)
        {
            // Save the recorded session to the clip.
            //_savedTime._animationDuration = Time.time;
            m_Recorder.SaveToClip(clip);
            Debug.Log("Animation saved");
            Debug.Log("Save took " + (Time.time - saveStartTime) + " seconds");
        }
    }
}

#endif
    
*/