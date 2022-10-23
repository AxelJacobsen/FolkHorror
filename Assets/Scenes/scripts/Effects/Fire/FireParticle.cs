using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for fire particles.
/// </summary>
public class FireParticle : MonoBehaviour
{
    // Public vars
    public Vector3  MinVelocity = new Vector3(-2, 1, 0) / 2f,
                    MaxVelocity = new Vector3(2, 2, 0) / 2f;
    public float    LifeTime    = 2f;

    // Private vars
    private Vector3 vel;
    private float   livedFor = 0f;

    void Start()
    {
        vel = new Vector3(  Random.Range(MinVelocity.x, MaxVelocity.x),
                            Random.Range(MinVelocity.y, MaxVelocity.y),
                            Random.Range(MinVelocity.z, MaxVelocity.z) );
    }

    void FixedUpdate()
    {
        // Count up lifetime and destroy if it exceeds set lifetime.
        livedFor += Time.deltaTime;
        if (livedFor >= LifeTime) {
            Destroy(gameObject);
        }

        // Update position
        transform.position += vel * Time.deltaTime;
    }
}
