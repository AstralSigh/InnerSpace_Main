using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using HighlightPlus;

[System.Serializable]
public class TourStop_Prototyping
{
    public float animationEndTime;
    public float splineEndTime;
    public string text;
    public List<ConstituentData.ConstituentType> selectableConstituentTypes = new();
    public List<GameObject> annotation = new List<GameObject>();
    public List<HighlightEffect> highlight = new List<HighlightEffect>();
    public FMODUnity.EventReference voiceOverEvent;
    public float animationPlaySpeed = 1f;
    public Transform visualFocus;
}
