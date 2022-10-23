using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEffect : MonoBehaviour
{
    // Public vars
    public GameObject FireParticleRedPrefab;
    public GameObject FireParticleYellowPrefab;
    public float Rate = 10; // Particles per second

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
                Vector3 pos = GetRandomPosInHitbox();
                Instantiate(FireParticleRedPrefab, pos, Quaternion.identity);
                Instantiate(FireParticleYellowPrefab, pos, Quaternion.identity);
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
