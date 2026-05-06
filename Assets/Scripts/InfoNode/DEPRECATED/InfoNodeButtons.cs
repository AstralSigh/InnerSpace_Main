using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("InfoNodeButtons is deprecated.")]
public class InfoNodeButtons : MonoBehaviour
{
    private InfoNodeHead infoNodeHead;

    public void AddToChain(Transform spawnLocation)
    {
        infoNodeHead.AddToChain(spawnLocation);
        PointerManager.Instance.DisablePointer();
    }

    public void CollapseChain()
    {
        infoNodeHead.CollapseChain();
    }

    public void SetInfoNodeHead(InfoNodeHead infoNodeHead)
    {
        this.infoNodeHead = infoNodeHead;
    }
}
