using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestLogBeat{
    public string header;
    public string subtext;
    public int progressIndex;
    public int progressCount;
    public bool complete;
}

[CreateAssetMenu(menuName = "ScriptableObjects/QuestLogData")]
public class QuestLogData : ScriptableObject
{
    public List<QuestLogBeat> questlog = new List<QuestLogBeat>();
}
