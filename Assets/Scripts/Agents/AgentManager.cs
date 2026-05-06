using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Klak.Motion;

public class AgentManager : MonoBehaviour {


    //Pool of Agent(s) to instantiate from
	public List<GameObject> myAgents;

    //How many agents to spawn
	public int populationCount;

    // How far from the Manager to spawn along each axis
    public Vector3 spawnRange;

    public bool insideSphere;
    public bool onSphere;
    public float sphereRadius;
    public Transform spawnOrigin;

    public Transform spawnAbove;
    //public float instDistX, instDistY, instDistZ;

    public bool randomizedRotation;

    public bool randomizedRotationOnlyYAxis;

    public bool randomizedScaleUniform;
    public float randomScaleMin;

    public bool scaleUniformly;
    public Vector3 uniformScaleVector;

    public int maxSpawnAttemptsPerObstacle = 10;
    public float obstacleCheckRadius = 3f;
    public List<string> colliderTags;

	void OnEnable () 
    {
        SpawnEachAgent(0.01f);
	}
    
    public void SpawnEachAgent(float delayAmount)
    {
        if(spawnOrigin== null)
        {
            spawnOrigin = gameObject.transform;
        }

        StartCoroutine(AgentSequence(delayAmount));
    }

	IEnumerator AgentSequence(float spawnDelay)
    {
        for (int i = 0; i < populationCount; i++)
        {
            GameObject instAgent;


            // Create a position variable
            Vector3 pos = Vector3.zero;

            // whether or not we can spawn in this position
            bool validPosition = false;

            // How many times we've attempted to spawn this obstacle
            int spawnAttempts = 0;

            // While we don't have a valid position 
            //  and we haven't tried spawning this obstacle too many times
            while (!validPosition && spawnAttempts < maxSpawnAttemptsPerObstacle)
            {
                // Increase our spawn attempts
                spawnAttempts++;

                // Pick a random position...
                if(insideSphere)
                {
                    //... within a sphere with specified radius and center origin
                    pos = Random.insideUnitSphere * sphereRadius + spawnOrigin.position;
                }
                else if(onSphere)
                {
                    //... on a sphere
                    pos = Random.onUnitSphere * sphereRadius + spawnOrigin.position;
                }
                else
                {
                    //... within adjustable X, Y, Z ranges from specified origin
                    pos = new Vector3(Random.Range(spawnOrigin.position.x - spawnRange.x, spawnOrigin.position.x + spawnRange.x),
                    Random.Range(spawnOrigin.position.y - spawnRange.y, spawnOrigin.position.y + spawnRange.y),
                    Random.Range(spawnOrigin.position.z - spawnRange.z, spawnOrigin.position.z + spawnRange.z));
                }

                // This position is valid until proven invalid
                validPosition = true;

                // Collect all colliders within our Obstacle Check Radius
                Collider[] colliders = Physics.OverlapSphere(pos, obstacleCheckRadius);

                // Go through each collider collected
                foreach (Collider col in colliders)
                {
                    //Check if any colliders have tags indicating illegal overlap; default tag to avoid for inner space is "Constituent"
                    foreach(string avoidTag in colliderTags)
                    {
                        if(col.tag == avoidTag)
                            validPosition = false;
                    }
                }

                if(spawnAbove && pos.y < spawnAbove.position.y)
                    validPosition= false;

            }

            Quaternion rot;

            if (randomizedRotationOnlyYAxis)
                rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            else if (randomizedRotation)
                rot = Random.rotation;
            else
                rot = Quaternion.identity;


            if (validPosition)
            {
                instAgent = Instantiate(myAgents[Random.Range(0, myAgents.Count)], pos, rot);
                instAgent.transform.parent = gameObject.transform;

                if (onSphere)
                {
                    var direction = (pos - spawnOrigin.position).normalized;
                    instAgent.transform.up = direction;
                }
                else
                {
                    instAgent.AddComponent<BrownianMotion>();
                }

                if (randomizedScaleUniform)
                {
                    float unitScaleFactor = Random.Range(randomScaleMin, 1.0f);
                    Vector3 unitScaleVector = new Vector3(unitScaleFactor, unitScaleFactor, unitScaleFactor);
                    instAgent.transform.localScale = unitScaleVector;

                }

                if (scaleUniformly)
                {
                    instAgent.transform.localScale = uniformScaleVector;
                }
                iTween.ScaleFrom(instAgent, iTween.Hash("scale", Vector3.zero, "time", 1.0f, "easetype", iTween.EaseType.easeOutBack));

            }
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public void HideEachAgent(float delayAmount)
    {
        StartCoroutine(HideSequence(delayAmount));
    }

    IEnumerator HideSequence(float hideOffset)
    {
        for (int i = 0; i < populationCount; i++)
        {

            iTween.ScaleTo(transform.GetChild(i).gameObject, iTween.Hash("scale", Vector3.zero, "time", 1.0f, "easetype", iTween.EaseType.easeOutBack));
            yield return new WaitForSeconds(hideOffset);

        }
    }

    public void ShowEachAgent(float delayAmount)
    {
        StartCoroutine (ShowSequence(delayAmount));
    }

    IEnumerator ShowSequence(float showOffset)
    {
        for (int i = 0; i < populationCount; i++)
        {

            iTween.ScaleTo(transform.GetChild(i).gameObject, iTween.Hash("scale", Vector3.one, "time", 1.0f, "easetype", iTween.EaseType.easeOutBack));
            yield return new WaitForSeconds(showOffset);

        }
    }
}
