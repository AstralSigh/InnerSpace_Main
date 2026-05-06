using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisiblyMasked : MonoBehaviour {
    public int renderQueue = 3002;

    Component[] ourRenderers;
	// Use this for initialization
	void Start () {
        ourRenderers = GetComponentsInChildren<Renderer>();

        foreach(Renderer rend in ourRenderers)
        {
            rend.material.renderQueue = renderQueue; 
        }
	}
	
	
}
