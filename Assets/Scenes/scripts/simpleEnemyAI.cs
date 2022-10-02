using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleEnemyAI : Character
{
    // Public vars
    [Header("Settings")]
    public string   targetObjectsWithTag = "Player";

    void Start()
    {
        base.Start();
        
        // Initialize vars
        attackCooldown = 0f;
    }

    void FixedUpdate()
    {
        base.FixedUpdate();

        // Find a target
        GameObject target = GetClosestTarget(targetObjectsWithTag);
        if (target != null) {
            // Fetch its rigidbody and the direction towards it
            Rigidbody tRB = target.GetComponent<Rigidbody>();
            Vector3 dir = tRB.transform.position - rb.position;

            // If we're outside outside of range, move towards
            if (dir.magnitude > AttackRange) {
                rb.velocity += (dir.normalized * Speed - rb.velocity) * 10f  * Time.deltaTime;

            // If we're within range, attack
            } else {
                if (attackCooldown <= 0) {
                    Attack(target);
                }
            }
        }
    }

    void OnDrawGizmosSelected() {
        // Draw attack range indicator
        if (AttackRange > 0f) {
            if (rb == null) rb = GetComponent<Rigidbody>();
            Gizmos.color = new Color(1f, 0.1f, 0.1f, 0.35f);
            Gizmos.DrawSphere(rb.position, AttackRange);
        } 
    }
}
