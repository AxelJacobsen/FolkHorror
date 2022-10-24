using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffect : MonoBehaviour
{
    // Public vars
    public GameObject BloodParticleOuterPrefab;
    public GameObject BloodParticleInnerPrefab;
    public float Rate = 10; // Particles per second

    [Header("Blood particles' settings")]
    public float    MaxSize     = 2f,
                    MinSize     = 1f,
                    LifeTime    = 2f;

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
        
        if (hitbox == null) Debug.LogError("Fireeffect could not find its child hitbox!");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Increment timer
        timeSinceLastSpawn += Time.deltaTime;

        // Spawn particles if it's above
        float amount = 1f / Rate;
        if (timeSinceLastSpawn > amount) {
            while (timeSinceLastSpawn > amount) {
                // Decide position
                Vector3 pos = GetRandomPosInHitbox();
                pos.y = hitbox.bounds.min.y;

                // Spawn particles
                GameObject bloodParticleOuter = Instantiate(BloodParticleOuterPrefab, pos, Quaternion.Euler(90, 0, 0));
                Color col = bloodParticleOuter.GetComponent<SpriteRenderer>().color;
                col.a = 0;
                bloodParticleOuter.GetComponent<SpriteRenderer>().color = col;
                float size = Random.Range(MinSize, MaxSize);
                bloodParticleOuter.GetComponent<BloodParticle>()._Size = size;
                bloodParticleOuter.GetComponent<BloodParticle>()._LifeTime = LifeTime;

                GameObject bloodParticleInner = Instantiate(BloodParticleInnerPrefab, pos, Quaternion.Euler(90, 0, 0));
                col = bloodParticleInner.GetComponent<SpriteRenderer>().color;
                col.a = 0;
                bloodParticleInner.GetComponent<SpriteRenderer>().color = col;
                bloodParticleInner.GetComponent<BloodParticle>()._Size = size * 0.85f;
                bloodParticleInner.GetComponent<BloodParticle>()._LifeTime = LifeTime;

                timeSinceLastSpawn -= amount;
            }

            // Reset timer
            timeSinceLastSpawn = 0;
        }
    }

    /// <summary>
    /// Gets a random position inside the hitbox.
    /// </summary>
    /// <returns>A random Vector3 inside the hitbox.</returns>
    private Vector3 GetRandomPosInHitbox() {
        Vector3 min = hitbox.bounds.min,
                max = hitbox.bounds.max;
        return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
    }
}
