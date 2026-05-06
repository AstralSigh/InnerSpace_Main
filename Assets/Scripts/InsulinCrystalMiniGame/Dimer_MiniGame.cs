using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dimer_MiniGame : MonoBehaviour
{ 
    public float radius = 1f;
    public float maxDistance = 1f;
    public LayerMask layerMask;
    RaycastHit[] hits;
    [SerializeField] private GameObject hexamerPrefab;
    public bool available = true;

    // Update is called once per frame
    void Update()
    {
        if (available)
        {
            hits = Physics.SphereCastAll(this.transform.position, radius, this.transform.forward, maxDistance, layerMask);
            List<GameObject> currentDimers = new List<GameObject>();
            // Loop through the array and do something with each hit object
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.CompareTag("Dimer") && hit.collider.gameObject.GetComponent<Dimer_MiniGame>().available)
                {
                    Debug.Log("Hit " + hit.collider.gameObject.name);
                    currentDimers.Add(hit.collider.gameObject);
                }

                if (currentDimers.Count == 3 && hit.collider.gameObject.GetComponent<Dimer_MiniGame>().available)
                {
                    Debug.Log("hexamerPrefab Made");
                    available = false;
                    currentDimers[0].GetComponent<Dimer_MiniGame>().SetFilled(false);
                    currentDimers[1].GetComponent<Dimer_MiniGame>().SetFilled(false);
                    currentDimers[2].GetComponent<Dimer_MiniGame>().SetFilled(false);
                    DoStuff(currentDimers[0], currentDimers[1], currentDimers[2]);
                    break;
                }
            }
        }
    }

    public void DoStuff(GameObject DimerA, GameObject DimerB, GameObject DimerC)
    {
        Instantiate(
            hexamerPrefab, transform.position, Quaternion.identity);
        Destroy(DimerA);
        Destroy(DimerB);
        Destroy(DimerC);
        Destroy(this.gameObject);
    }
    public void SetFilled(bool available)
    {
        this.available = available;
    }
}
