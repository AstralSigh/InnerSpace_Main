using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowAnimationAssignment : MonoBehaviour
{
    GameObject _itemToFollow = null;
    void Update()
    {
        if(_itemToFollow != null)
        {
            transform.position = _itemToFollow.transform.position;
        }
    }

    public void followItem(GameObject item)
    {
        _itemToFollow = item;
    }
}
