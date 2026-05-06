using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AminoAcidLabel : MonoBehaviour
{
    [SerializeField]
    private Text labelText;
    
    public void SummonAcidLabel(Vector3 newPosition, string newAcidName)
    {
        gameObject.SetActive(true);
        gameObject.transform.position = newPosition;
        labelText.text = newAcidName;
    }

    public void CloseAcidLabel()
    {
        gameObject.SetActive(false);
    }
}
