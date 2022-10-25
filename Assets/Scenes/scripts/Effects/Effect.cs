using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for spawning particles.
/// </summary>
public class Effect : MonoBehaviour
{
    // Public vars
    [Header("Settings")]
    public string EffectName;
    public GameObject[] ParticlePrefabs;
    public bool SpawnParticlesOnGroundOnly = false; // Spawn particles on the ground ONLY.
    public float Rate = 10,     // Particles per second.
                 Wiggle = 0.1f,
                 WiggleSpeed = 0.5f;

    [Header("Particle settings")]
    public float    MinSizeMultiplier = 1f,
                    MaxSizeMultiplier = 2f,
                    LifeTime = 2f;
    public Vector3  MinVelocity = new Vector3(-2, 1, 0) / 2f,
                    MaxVelocity = new Vector3(2, 2, 0) / 2f;

    [Header("Set by scripts")]
    public bool _Active = false;

    // Private vars
    private BoxCollider hitbox;
    private float timeSinceLastSpawn = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Fetch components
        foreach (Transform child in transform) {
            if (child.gameObject.tag == "Hitbox") {
                hitbox = child.gameObject.GetComponent<BoxCollider>();
                if (hitbox != null) break;
            }
        }
        if (hitbox == null) Debug.LogError("Effect could not find its child hitbox!");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Don't do anything if set to inactive
        if (!_Active) return;

        // Increment timer
        timeSinceLastSpawn += Time.deltaTime;

        // If timer exceeds waiting-time...
        float amountPerSec = 1f / Rate;
        if (timeSinceLastSpawn > amountPerSec) {

            // ...for each particle that needs to be spawned...
            while (timeSinceLastSpawn > amountPerSec) {
                (Vector3, Quaternion) t = GetSpawnTransform();
                float wiggleOffset = Random.Range(0, 360);

                // (And each particle in the particlePrefabs-array)
                foreach(GameObject particlePrefab in ParticlePrefabs) {
                    // Instantiate it and set vars
                    GameObject particle = Instantiate(particlePrefab, t.Item1, t.Item2);

                    Particle pScript = particle.GetComponent<Particle>();
                    pScript._SizeMultiplier = Random.Range(MinSizeMultiplier, MaxSizeMultiplier);
                    pScript._LifeTime = LifeTime;
                    pScript._Vel = new Vector3( Random.Range(MinVelocity.x, MaxVelocity.x),
                                                Random.Range(MinVelocity.y, MaxVelocity.y),
                                                Random.Range(MinVelocity.z, MaxVelocity.z) );
                    pScript._Wiggle = Wiggle;
                    pScript._WiggleSpeed = WiggleSpeed;
                    pScript._WiggleOffset = wiggleOffset;
                }

                timeSinceLastSpawn -= amountPerSec;
            }

            // Reset timer
            timeSinceLastSpawn = 0;
        }
    }

    /// <summary>
    /// Gets a random, but valid, spawn-position for a particle.
    /// </summary>
    /// <returns>A random, valid, spawnpoint for a particle.</returns>
    private (Vector3, Quaternion) GetSpawnTransform() {
        // Get a random position in the hitbox
        Vector3 min = hitbox.bounds.min,
                max = hitbox.bounds.max,
                pos = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));

        // Get an angle
        Quaternion ang = Quaternion.Euler(0, 0, 0);

        // Set z=0 if particles are only meant to spawn on the ground
        if (SpawnParticlesOnGroundOnly) {
            pos.y = min.y;
            Vector3 angAsVec = ang.eulerAngles;
            angAsVec.x = 90;
            ang = Quaternion.Euler(angAsVec);
        }

        // Return
        return (pos, ang);
    }
}