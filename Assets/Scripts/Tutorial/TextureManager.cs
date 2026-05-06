using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureManager: MonoBehaviour
{
    public Texture[] textures;

    public void UpdateTexture(int inputState)
    {
        transform.GetComponent<Renderer>().material.SetTexture("_BaseMap", textures[inputState]);
    }
}
