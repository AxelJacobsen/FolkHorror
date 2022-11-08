using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// A class for spawning particles.
/// </summary>
[CreateAssetMenu(menuName = "Effects/EffectEmitter")]
public class EffectEmitter : ScriptableObject
{
    // Public vars
    [Header("Set by scripts")]
    public BoxCollider  _Hitbox;
    public bool         _Active = false;

    [Header("Settings")]
    public GameObject[] ParticlePrefabs;
    public float Rate = 10; // Particles per second.
    public float Wiggle = 0.1f;
    public float WiggleSpeed = 0.5f;
    public float SpinRadius = 0f;
    public float SpinSpeed = 0f;
    public bool  SpawnParticlesOnGroundOnly = false; // Spawn particles on the ground ONLY.

    [Header("Particle settings")]
    public float    MinSizeMultiplier = 1f;
    public float    MaxSizeMultiplier = 2f;
    public float    LifeTime = 2f;
    public Vector3  MinVelocity = new Vector3(-2, 1, 0) / 2f;
    public Vector3  MaxVelocity = new Vector3(2, 2, 0) / 2f;
    public Vector3  AngleOverride;
    public Func<float, float> SizeFunc = (f => 1f - f);
    public Func<float, float> AlphaFunc = (f => 1f - f);

    // Private vars
    private float timeSinceLastSpawn = 0;

    public void Emit(float deltaTime)
    {
        // Don't do anything if set to inactive
        if (!_Active) return;

        // Increment timer
        timeSinceLastSpawn += deltaTime;

        // If timer exceeds waiting-time...
        float amountPerSec = 1f / Rate;
        if (timeSinceLastSpawn > amountPerSec) {

            // ...for each particle that needs to be spawned...
            while (timeSinceLastSpawn > amountPerSec) {
                (Vector3, Quaternion) t = GetSpawnTransform();
                float wiggleOffset = UnityEngine.Random.Range(0, 360);

                // (And each particle in the particlePrefabs-array)
                foreach(GameObject particlePrefab in ParticlePrefabs) {
                    // Instantiate it and set vars
                    GameObject particle = Instantiate(particlePrefab, t.Item1, t.Item2);

                    Particle pScript = particle.GetComponent<Particle>();
                    pScript._SizeMultiplier = UnityEngine.Random.Range(MinSizeMultiplier, MaxSizeMultiplier);
                    pScript._LifeTime = LifeTime;
                    pScript._Vel = new Vector3( UnityEngine.Random.Range(MinVelocity.x, MaxVelocity.x),
                                                UnityEngine.Random.Range(MinVelocity.y, MaxVelocity.y),
                                                UnityEngine.Random.Range(MinVelocity.z, MaxVelocity.z) );
                    pScript._Wiggle = Wiggle;
                    pScript._WiggleSpeed = WiggleSpeed;
                    pScript._WiggleOffset = wiggleOffset;
                    pScript._SpinRadius = SpinRadius;
                    pScript._SpinSpeed = SpinSpeed;
                    pScript._SpinOffset = UnityEngine.Random.Range(0, 360);
                    pScript._SizeFunc = SizeFunc;
                    pScript._AlphaFunc = AlphaFunc;
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
        Vector3 min = _Hitbox.bounds.min,
                max = _Hitbox.bounds.max,
                pos = new Vector3(
                    UnityEngine.Random.Range(min.x, max.x), 
                    UnityEngine.Random.Range(min.y, max.y), 
                    UnityEngine.Random.Range(min.z, max.z) 
                );

        // Get an angle
        Quaternion ang = Quaternion.Euler(0, 0, 0);

        // Set z=0 if particles are only meant to spawn on the ground
        if (SpawnParticlesOnGroundOnly) {
            pos.y = min.y;
            if (AngleOverride != null) ang = Quaternion.Euler(AngleOverride);
        }

        // Return
        return (pos, ang);
    }
}