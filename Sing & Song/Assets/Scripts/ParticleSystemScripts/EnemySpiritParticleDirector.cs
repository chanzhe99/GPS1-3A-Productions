using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpiritParticleDirector : MonoBehaviour
{
    Transform particleTarget;
    ParticleSystem ps;
    ParticleSystem.Particle[] parts = new ParticleSystem.Particle[1000];
    float timeBeforeAbsorb = 0f;

    private void Start()
    {
        particleTarget = GameObject.FindGameObjectWithTag("Sing").transform;
        if (particleTarget != null) print("Got Sing: " + particleTarget.name);
        ps = GetComponent<ParticleSystem>();
        timeBeforeAbsorb = 0f;
    }

    void Update()
    {
        int partSize = ps.GetParticles(parts);
        timeBeforeAbsorb += Time.deltaTime;
        if(timeBeforeAbsorb >= 0.5f)
        {
            for (int i = 0; i < partSize; i++)
                parts[i].position = Vector3.MoveTowards(parts[i].position, particleTarget.position, Time.deltaTime * 25f);
        }
        ps.SetParticles(parts, partSize);
    }
}
