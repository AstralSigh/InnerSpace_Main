using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

public class SpriteManager : MonoBehaviour
{
    public Sprite[] sprite;
    public Color idleColor;
    public Color onHoverColor;
    public Color onSelectColor;

    public void UpdateSprite(int inputState)
    {
        transform.GetComponent<SpriteRenderer>().sprite = sprite[inputState];
        Debug.Log("Update sprite with index " + inputState);
    }

    public void OnHover()
    {
        transform.GetComponent<SpriteRenderer>().color = onHoverColor;
    }

    public void OnSelect()
    {
        transform.GetComponent<SpriteRenderer>().color = onSelectColor;
    }

    public void OnExit()
    {
        transform.GetComponent<SpriteRenderer>().color = idleColor;
    }
}
