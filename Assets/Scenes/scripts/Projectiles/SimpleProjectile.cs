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
        // Add some more effects here
        Destroy(this.gameObject);
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


        // Bounce
        if (Bounces != 0)
        {
            Vector3 dir = (hitObj.transform.position - rb.position).normalized;
            rb.velocity = rb.velocity - 2f * (Vector3.Dot(rb.velocity, dir)) * dir;

            Vector3 curAngEuler = rb.rotation.eulerAngles;
            curAngEuler.y = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * 180f / Mathf.PI + 90f;
            rb.rotation = Quaternion.Euler(curAngEuler);
            Bounces--;
        } else 
        {
        // Pierce
        if (Piercing == 0) {Destroy(); return; }
        if (Piercing > 0) Piercing--;
        }
    }
}
