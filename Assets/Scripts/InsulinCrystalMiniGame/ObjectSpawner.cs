using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns hexamers for the crystal tiling segment of the game.
/// </summary>

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objToSpawn;
    [Tooltip("Radius of the sphere that objects will be generated around")]
    public float radius = 4f;
    private Vector3 randomPosition;
    private List<GameObject> hexamerList;

    public void GeneratehexamerPrefab()
    {
        bool positionIsValid = false;

        // Keep finding a new random position until it's valid (not overlapping)
        while (!positionIsValid)
        {
            randomPosition = Random.onUnitSphere * radius;
            randomPosition += transform.position;

            // Check for overlap with existing colliders
            Collider[] colliders = Physics.OverlapSphere(randomPosition, 0.25f);
            positionIsValid = colliders.Length == 0;
        }

        // Instantiate the objToSpawn at the random position and with no rotation
        GameObject newHexamer = Instantiate(objToSpawn, randomPosition, Quaternion.Euler(90, 0, 0));
        
        if(hexamerList == null)
        {
            hexamerList = new List<GameObject>();
        }
        hexamerList.Add(newHexamer);
    }

    public void HideNonBindedHexamers()
    {
        foreach(GameObject h in hexamerList)
        {
            if(h.transform.Find("SphereCollider").GetComponent<Hexamer_MiniGame>().currentHexamerState != Hexamer_MiniGame.HexamerState.Placed)
            {
                h.SetActive(false);
            }
        }
    }
}

