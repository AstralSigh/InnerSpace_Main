using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Monomer_MiniGame : MonoBehaviour
{
    public GameObject connectionPointA;
    public GameObject connectionPointB;
    [SerializeField] private float distanceTolerance = 0.2f;
    [SerializeField] private GameObject dimerPrefab;
    public bool available = true;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Monomer"))
        {
            float DistanceA = Vector3.Distance(connectionPointA.transform.position, other.GetComponent<Monomer_MiniGame>().connectionPointB.transform.position);
            float DistanceB = Vector3.Distance(connectionPointB.transform.position, other.GetComponent<Monomer_MiniGame>().connectionPointA.transform.position);

            if(DistanceA < distanceTolerance && DistanceB < distanceTolerance && other.GetComponent<Monomer_MiniGame>().available)
            {
                available = false;
                StartCoroutine(MergeAnimation(.5f, other));
            }
        }
    }

    IEnumerator MergeAnimation(float duration, Collider other)
    {
        //Switch off colliders 
        other.transform.GetComponent<BoxCollider>().enabled = false;
        transform.GetComponent<Collider>().enabled = false;

        //Lerp positions
        yield return new WaitForEndOfFrame();

        //Instantiate Dimer
        Transform.Instantiate(dimerPrefab, transform.position, transform.rotation);

        //Destroy monomers 
        Destroy(other.transform.gameObject);
        Destroy(transform.gameObject);
    }
}
