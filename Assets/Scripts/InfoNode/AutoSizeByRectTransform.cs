using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: we are currently using 3d meshs behind in world canvas as a temp solution for Shader issue. Remove this once Shader issue fixed. 
public class AutoSizeByRectTransform : MonoBehaviour
{
    [Header("We are currently using 3d meshs behind in world canvas as a temp\n solution for Shader issue. Remove this once Shader issue fixed")]
    [Space(32)]
    public RectTransform rectTransform;
    public Canvas canvs;
    public Vector2 offset = Vector2.zero; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!rectTransform) { return; }
        Vector2 size = rectTransform.sizeDelta;
        size += offset;
        if (canvs != null) { size /= canvs.scaleFactor; }
        this.transform.localScale.Set(size.x , size.y, 0.001f) ;
    }
}
