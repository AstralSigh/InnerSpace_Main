using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BNG;

public class InfoNodeSlot_DEPRECATED : MonoBehaviour
{
    private InfoNodeHeadV2 _targetInfoNode;
    
    public void initialize(string nodeStringIndex, InfoNodeHeadV2 targetInfoNode)
    {
        // Update letter with nodeIndex
        _targetInfoNode = targetInfoNode;
        transform.Find("TextCanvas").transform.GetChild(0).transform.GetComponent<Text>().text = nodeStringIndex;
    }

    public void TeleportTo()
    {
        Vector3 teleportDest = _targetInfoNode.transform.position - ((_targetInfoNode.GetStartRotation() * Vector3.forward).normalized * 0.5f );
        PlayerManager.Instance.TeleportPlayer(teleportDest, _targetInfoNode.GetStartRotation());
    }
}
