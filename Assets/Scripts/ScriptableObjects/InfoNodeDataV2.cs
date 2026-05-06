using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static InfoNodeDataV2;

[CreateAssetMenu(menuName = "ScriptableObjects/InfoNodeDataV2")]
public class InfoNodeDataV2 : ScriptableObject
{
    public string nodeName;
    public Sprite nodePreview;
    public int nodeNumber;
    public Nexus_Data.eNexusType nodeNexus;
    public InfoNodeType nodeType = InfoNodeType.Structure;
    public List<ConstituentData.ConstituentType> nodeConstituents;
    public List<Vector3> nodePositions;

    [TextArea(5, 10)]
    public string nodeAbstract;

    public List<NodeTextData> nodeTextLayers;

    [TextArea(5, 10)]
    public List<Sprite> nodeSpriteLayer;


    [Serializable]
    public struct NodeTextData
    {
        [TextArea(3, 3)]
        public string title;
        [TextArea(5, 10)]
        public string basic;
        [TextArea(5, 10)]
        public string intermediate;
        [TextArea(5, 10)]
        public string advanced;
    }
}

/// <summary>
/// To upgrade the data we already have from original InfoNodeData to InfoNodeDataV2
/// </summary>
#if UNITY_EDITOR
#pragma warning disable 0618 // ignore "`InfoNodeData` is obsolete" warnings
public class DataMigrationUtility
{
    [MenuItem("Data/Migrate Selected InfoNodeData to InfoNodeDataV2")]
    public static void MigrateSelectedData()
    {
        if (Selection.count <= 0)
        {
            return;
        }
        foreach (var obj in Selection.objects)
        {
            if (obj is InfoNodeData)
            {
                InfoNodeData oldData = (InfoNodeData)obj;

                // Create the new asset
                InfoNodeDataV2 newData = ScriptableObject.CreateInstance<InfoNodeDataV2>();
                newData.nodeName = oldData.nodeName;
                newData.nodeNumber = oldData.nodeNumber;
                newData.nodeNexus = oldData.nodeNexus;
                newData.nodeConstituents = oldData.nodeConstituents;
                newData.nodePositions = oldData.nodePositions;
                newData.nodeAbstract = oldData.nodeAbstract;
                newData.nodeTextLayers = new List<NodeTextData>();
                foreach (var item in oldData.nodeTextLayers)
                {
                    NodeTextData newNTD = new NodeTextData();
                    newNTD.basic = item;
                    newData.nodeTextLayers.Add(newNTD);
                }
                newData.nodeSpriteLayer = oldData.nodeSpriteLayer;

                // ... copy any other fields ...

                string fullPath = AssetDatabase.GetAssetPath(oldData);
                string pathWithoutExtension = Path.GetDirectoryName(fullPath) + "/" + Path.GetFileNameWithoutExtension(fullPath) + "_V2.asset";
                AssetDatabase.CreateAsset(newData, pathWithoutExtension);

                AssetDatabase.Refresh();
            }
        }
    }
}
#pragma warning restore 0618
#endif
public enum InfoNodeType
{
    Structure,
    Interaction,
    Function
}