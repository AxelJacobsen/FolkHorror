using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for simple enemy AIs.
/// </summary>
public class simpleEnemyAI : Character
{
    // Public vars
    [Header("Settings")]
    public string   targetObjectsWithTag = "Player";

    // Private vars
    private string  prevTargetTag = "";
    private float   charmDuration = 0f;

    void Start()
    {
        base.Start();
    }

    /// <summary>
    /// Sets the AI's target tag to be something for a given amount of seconds.
    /// </summary>
    /// <param name="newTargetTag">The new target tag.</param>
    /// <param name="duration">How long to wait before setting target tag back to original.</param>
    public void Charm(string newTargetTag, float duration) 
    {
        if (charmDuration <= 0f) 
            prevTargetTag = targetObjectsWithTag;

        targetObjectsWithTag = newTargetTag;
        charmDuration = duration;
    }

    void FixedUpdate()
    {
        base.FixedUpdate();

        // Count down charm duration and revert charm if it times out
        charmDuration -= Time.deltaTime;
        if (charmDuration <= 0f && prevTargetTag != "") 
        {
            targetObjectsWithTag = prevTargetTag;
            prevTargetTag = "";
        } 

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
                Move((dir.normalized * Speed - rb.velocity) * 2f * Time.deltaTime);

            // If we're within range, attack
            } else if (Weapon != null) {
                Attack(target.transform.position, targetObjectsWithTag);
            }
        }
    }
}
