using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
    [CustomEditor(typeof(IntroParticleSystemManager))]
    public class ParticleEditor : Editor
    {
        static int psindex;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

        IntroParticleSystemManager myScript = (IntroParticleSystemManager)target;

        psindex = EditorGUILayout.IntField("Particle System Index:", psindex);    

            if (GUILayout.Button("Switch On"))
            {
                myScript.SwitchOn(psindex);
            }
            if (GUILayout.Button("Switch Off"))
            {
                myScript.SwitchOff(psindex);
            }
        }
    }
#endif

    public class IntroParticleSystemManager : MonoBehaviour
{
    [SerializeField]
    private List<ParticleSystem> particleSystems;

    private void Start()
    {
        foreach(ParticleSystem p in particleSystems)
        {
            p.Stop();
        }
    }

    public void SwitchOn(int i)
    {
        particleSystems[i].Play();
    }

    public void SwitchOff(int i)
    {
        particleSystems[i].Stop();
    }
}
