using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleDirector : MonoBehaviour
{
    Transform particleTarget;
    ParticleSystem ps;
    ParticleSystem.Particle[] parts = new ParticleSystem.Particle[1000];

    private void Start()
    {
        particleTarget = GameObject.FindGameObjectWithTag("Sing").transform;
        if (particleTarget != null) print("Got Sing: " + particleTarget.name);
        ps = GetComponent<ParticleSystem>();
    }

    private void LateUpdate()
    {
        int partSize = ps.GetParticles(parts);

        for (int i = 0; i < partSize; i++)
            parts[i].position = Vector3.MoveTowards(parts[i].position, particleTarget.position, Time.deltaTime * 2f);

        // Apply the modified particles
        ps.SetParticles(parts, partSize);


    }
}
