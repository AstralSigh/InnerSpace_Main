using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoNodeButtonsV2 : MonoBehaviour
{
    private InfoNodeHeadV2 infoNodeHead;

    public void AddToChain(Transform spawnLocation)
    {
        infoNodeHead.AddToChain(spawnLocation);
        PointerManager.Instance.DisablePointer();
    }

    public void CollapseChain()
    {
        infoNodeHead.CollapseChain();
    }

    public void SetInfoNodeHead(InfoNodeHeadV2 infoNodeHead)
    {
        this.infoNodeHead = infoNodeHead;
    }
}
