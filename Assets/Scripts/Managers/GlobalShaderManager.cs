using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalShaderManager : MonoBehaviour
{
    public GameObject playerPosition;

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalVector("_PlayerPos", playerPosition.transform.position);

        Shader.SetGlobalVector("_AffectorPos", playerPosition.transform.position);

        Shader.SetGlobalFloat("_AffectorRadius", playerPosition.transform.localScale.x);
        


    }
}
