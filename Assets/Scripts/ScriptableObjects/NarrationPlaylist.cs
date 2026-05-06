using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;

[CreateAssetMenu(menuName = "ScriptableObjects/NarrationPlaylist")]
public class NarrationPlaylist : ScriptableObject
{
    public string sceneHeader;
    public string sceneSubHeader;

    public List<NarrationBeat> NarrationBeats;
}

[System.Serializable]
public class NarrationBeat
{
    [TextArea(5,5)]
    public string narrationText;

    //public string eventPathFMOD;
    public FMODUnity.EventReference narrationTrack;
}