using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    // Public vars
    [Header("Stats")]
    public int      Damage,
                    Piercing,
                    Bounces;
    public float    Knockback,
                    Lifetime;

    [Header("Visual")]
    public bool     fades;

    // Private vars
    private Rigidbody rb;
    private float livedFor;

    void Start() 
    {
        // Fetch vars
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("Projectile could not find its rigidbody!");
    }

    void FixedUpdate() {
        // Count down lifetime
        Lifetime -= Time.deltaTime;
        if (Lifetime <= 0f) Destroy();
    }

    /// <summary>
    /// Destroys this projectile.
    /// </summary>
    void Destroy()
    {
        this.enabled = false;
        // Add some more effects here
    }

    void OnTriggerEnter(Collider hit) 
    {
        // If the hit collider belongs to a hitbox, use its parent instead.
        GameObject hitObj = hit.gameObject;
        if (hitObj.tag == "Hitbox") { hitObj = hitObj.transform.parent.gameObject; }
        else { return; } // Otherwise, return.

        // Check if it hit a character. If not, return.
        Character characterHit = hitObj.GetComponent<Character>();
        if (characterHit == null) { return; }

        // Apply effects on target
        characterHit.Knockback(rb.velocity.normalized * Knockback);
        characterHit.Hurt(Damage);

        // Pierce
        if (Piercing > 0) Piercing--;
        if (Piercing == 0) { Destroy(); return; }

        // Bounce
        if (Bounces > 0)
        {
            Bounces--;
            Vector3 dir = (hitObj.transform.position - rb.position).normalized;
            rb.velocity = rb.velocity - 2f * (Vector3.Dot(rb.velocity, dir)) * dir;
        }
    }
}
