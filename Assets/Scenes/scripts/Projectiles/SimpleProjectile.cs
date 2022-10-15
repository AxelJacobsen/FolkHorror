using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SimpleProjectile : MonoBehaviour
{
    // Public vars
    [Header("Stats")]
    public int      Damage;
    public float    Knockback,
                    Lifetime;

    [Header("Effects")]
    public int      Chains,
                    Bounces,
                    Pierces;


    [Header("Visual")]
    public bool     Fades;

    [Header("Set by scripts")]
    public string   _TargetTag;

    // Private vars
    private Rigidbody rb;
    private float livedFor;
    private List<GameObject> chainTargets;

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

        // Check if that character has the correct tag. If not, return.
        if (characterHit.tag != _TargetTag) { return; }

        // Check if the projectile rigidbody is initialized. If not, return.
        if (rb == null) { return; }

        // Apply stats on target
        characterHit.Knockback(rb.velocity.normalized * Knockback);
        characterHit.Hurt(Damage);

        // Bounce
        if (Bounces != 0 || Chains != 0)
        { 

            // (Chain) 
            if (Chains != 0) 
            {
                // Fetch new chain list if there aren't any
                if (chainTargets == null || chainTargets.Count == 0) {
                    chainTargets = GameObject.FindGameObjectsWithTag(_TargetTag).ToList();
                    chainTargets.Remove(hitObj);
                }

                // Change direction towards the closest enemy in the list and remove it from the list
                if (chainTargets.Count > 0) {
                    GameObject target = Funcs.GetClosestTargetTo(chainTargets.ToArray(), this.gameObject);
                    chainTargets.Remove(target);

                    if (target != null) {
                    Vector3 dir = (target.transform.position - hitObj.transform.position).normalized;
                    rb.velocity = rb.velocity.magnitude * dir;
                    Chains--;
                    }
                }

            } else 
            {
                Vector3 dir = (hitObj.transform.position - rb.position).normalized;
                rb.velocity = rb.velocity - 2f * (Vector3.Dot(rb.velocity, dir)) * dir;
                Bounces--;
            }
            
            // Set angle based on new velocity
            Vector3 curAngEuler = rb.rotation.eulerAngles;
            curAngEuler.y = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * 180f / Mathf.PI + 90f;
            rb.rotation = Quaternion.Euler(curAngEuler);

        } else 
        { // Pierce
        if (Pierces == 0) {Destroy(); return; }
        if (Pierces > 0) Pierces--;
        }
    }
}