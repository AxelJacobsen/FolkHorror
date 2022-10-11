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
        GameObject target = Funcs.GetClosestTargetTo(targets, this.gameObject);

        // If it exists...
        if (target != null) {
            // If we're outside outside of range, move towards
            if (Weapon == null || !Weapon.InRangeOf(target) || !Weapon.CanAttack()) {

                // Fetch its rigidbody and the direction towards it
                Rigidbody tRB = target.GetComponent<Rigidbody>();
                Vector3 dir = tRB.transform.position - rb.position;

                rb.velocity += (dir.normalized * Speed - rb.velocity) * 2f * Time.deltaTime;

            // If we're within range, attack
            } else if (Weapon != null) {
                Attack(target.transform.position, targetObjectsWithTag);
            }
        }
    }
}
