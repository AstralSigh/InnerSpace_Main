using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetraParticleEmitter : MonoBehaviour
{
    [SerializeField] private GameObject tetraParticlePrefab;
    [SerializeField] private GameObject tetraCellParticlePrefab;
    [SerializeField] private Transform emitter;
    public float emissionRate = 1.0f; // Adjust this value in the Inspector
    public float cellEmissionRate = 0.2f;
    public float velocity = 8f;
    public Vector3 scale;
    public Vector3 rotation;
    private float timer = 0.0f;
    private float cellTimer = 0.0f;



    void Update()
    {
        {
            if(emitter.gameObject.activeSelf == true)
            {
                timer += Time.deltaTime;

                if (timer > 1f / emissionRate)
                {
                    timer = 0.0f;
                    EmitParticle();
                }

                cellTimer += Time.deltaTime;

                if (cellTimer > 1f / cellEmissionRate)
                {
                    cellTimer = 0.0f;
                    EmitCellParticle();
                }
            }
        }
    }
    void EmitParticle()
    {
        Vector3 randomPosition = Vector3.zero;
        while (Vector3.Distance(randomPosition, Vector3.zero) <= 30)
        {
            randomPosition = new Vector3(
                Random.Range(-emitter.localScale.x / 2, emitter.localScale.x / 2),
                Random.Range(-emitter.localScale.y / 2, emitter.localScale.y / 2),
                Random.Range(-emitter.localScale.z / 2, emitter.localScale.z / 2)
            );
        }
       
        randomPosition += transform.position; // Position within the cube

        GameObject particle = Instantiate(tetraParticlePrefab, randomPosition, Quaternion.identity);
        particle.GetComponent<TetraParticles>().SetParameters(velocity, rotation, scale);
        particle.transform.parent = transform; 
    }
    void EmitCellParticle()
    {
        Vector3 randomPosition = Vector3.zero;
        while (Vector3.Distance(randomPosition, Vector3.zero) <= 70)
        {
            randomPosition = new Vector3(
                Random.Range(-emitter.localScale.x / 2, emitter.localScale.x / 2),
                Random.Range(-emitter.localScale.y / 2, emitter.localScale.y / 2),
                Random.Range(-emitter.localScale.z / 2, emitter.localScale.z / 2)
            );
        }

            

        randomPosition += transform.position; // Position within the cube

        GameObject particle = Instantiate(tetraCellParticlePrefab, randomPosition, Quaternion.identity);
        particle.GetComponent<TetraCellParticles>().SetParameters(velocity, rotation, scale);
        particle.transform.parent = transform;
    }
}
