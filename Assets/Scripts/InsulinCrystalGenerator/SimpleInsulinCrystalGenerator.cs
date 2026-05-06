using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInsulinCrystalGenerator : MonoBehaviour
{
    public PossibleHexamerLocations hexamerSavedLocation;
    public Mesh lowpolyMesh;
    public Material hexamerMaterial;
    private List<List<Matrix4x4>> renderBatches = new List<List<Matrix4x4>>();
    private int batchCount;
    [Tooltip("Use to omit rendering first 90 hexamers for animation loop")]
    [SerializeField] private float skipHexamers = 90;
    private void Update()
    {
        UpdateRenderBatches();
        RenderAllBatches();
    }

    void UpdateRenderBatches()
    {
        batchCount = 0;
        renderBatches.Clear();
        renderBatches.Add(new List<Matrix4x4>());
        float index = 0;
        foreach (Vector3 h in hexamerSavedLocation.possibleHexamerLocations)
        {
            if (index < skipHexamers)
            {
                index++;
            }
            else
            {
                if (renderBatches[batchCount].Count >= 999)
                {
                    batchCount++;
                    renderBatches.Add(new List<Matrix4x4>());
                }
                Vector3 targetPosition = this.transform.position + transform.forward * h.z + transform.up * h.y + transform.right * h.x;
                renderBatches[batchCount].Add(Matrix4x4.TRS(targetPosition, this.transform.rotation, Vector3.one));
            }
        }
    }

    void RenderAllBatches()
    {
        foreach (List<Matrix4x4> batch in renderBatches)
        {
            for (int i = 0; i < lowpolyMesh.subMeshCount; i++)
            {
                Graphics.DrawMeshInstanced(lowpolyMesh, i, hexamerMaterial, batch);
            }
        }
    }

    
}
