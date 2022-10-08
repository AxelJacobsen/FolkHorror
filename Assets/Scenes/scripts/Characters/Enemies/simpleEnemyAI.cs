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
    }

    void FixedUpdate()
    {
        base.FixedUpdate();

        // Find a target
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetObjectsWithTag);
        GameObject target = GetClosestTarget(targets);

        // If it exists...
        if (target != null) {
            // Fetch its rigidbody and the direction towards it
            Rigidbody tRB = target.GetComponent<Rigidbody>();
            Vector3 dir = tRB.transform.position - rb.position;

            // If we're outside outside of range, move towards
            if (dir.magnitude > AttackRange) {
                rb.velocity += (dir.normalized * Speed - rb.velocity) * 2f * Time.deltaTime;

            // If we're within range, attack
            } else if (Weapon != null && Weapon.CanAttack()) {
                Attack(targets);
            }
        }
    }
}
