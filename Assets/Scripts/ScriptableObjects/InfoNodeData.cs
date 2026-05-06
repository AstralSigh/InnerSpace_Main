using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("InfoNodeData is deprecated, please use InfoNodeDataV2 instead.")]
[CreateAssetMenu(menuName = "ScriptableObjects/InfoNodeData")]
public class InfoNodeData : ScriptableObject
{
    public string nodeName;
    public int nodeNumber;
    public Nexus_Data.eNexusType nodeNexus;
    public List<ConstituentData.ConstituentType> nodeConstituents;
    public List<Vector3> nodePositions;

    [TextArea(5, 10)]
    public string nodeAbstract;

    [TextArea(5, 10)]
    public List<string> nodeTextLayers;

    [TextArea(5, 10)]
    public List<Sprite> nodeSpriteLayer;

}
