using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for simple enemy AIs.
/// </summary>
public class BaseEnemyAI : Character
{
    /// <summary>
    /// An enum describing the behavioral states a normal enemy can have.
    /// </summary>
    public enum Behavior
    {
        Idle,
        Wander,
        Aggressive,
        Investigating
    }

    // Public vars
    [Header("AI Settings")]
    public string       TargetObjectsWithTag = "Player";
    public float        VisionRange = 10f;
    public GameObject   DesiredWeapon;

    // Private vars
    private string      prevTargetTag = "";
    private float       charmDuration = 0f;
    
    public Behavior     behavior;

    private Vector3     waypoint;
    private float       waypointTimer = 0f;
    private float       waypointCooldown = 3f;

    protected override void OnStart()
    {
        
    }

    /// <summary>
    /// Sets the AI's target tag to be something for a given amount of seconds.
    /// </summary>
    /// <param name="newTargetTag">The new target tag.</param>
    /// <param name="duration">How long to wait before setting target tag back to original.</param>
    public virtual void Charm(string newTargetTag, float duration) 
    {
        if (charmDuration <= 0f) 
            prevTargetTag = TargetObjectsWithTag;

        TargetObjectsWithTag = newTargetTag;
        charmDuration = duration;
    }

    /// <summary>
    /// Starts investigating a position, if not occupied with something else.
    /// </summary>
    /// <param name="position">The position to investigate.</param>
    /// <returns>False if AI was occupied and could not start investigating.</returns>
    public bool StartInvestigating(Vector3 position)
    {
        if (behavior == Behavior.Aggressive || behavior == Behavior.Investigating)
            return false;

        waypoint = position;
        behavior = Behavior.Investigating;
        return true;
    }

    /// <summary>
    /// Sets waypoint.
    /// </summary>
    /// <param name="waypoint">The waypoint.</param>
    /// <param name="cooldown">How much cooldown to add.</param>
    public void SetWaypoint(Vector3 waypoint, float cooldown)
    {
        this.waypoint = waypoint;
        waypointTimer += cooldown;
    }

    /// <summary>
    /// Gets a random waypoint.
    /// (Place the AI wants to wander to)
    /// </summary>
    /// <returns>The waypoint.</returns>
    protected Vector3 GetWaypoint()
    {
        float   walkableDistance = Speed / 2f * waypointCooldown,
                walkDistance = Random.Range(0.5f, 1f) * walkableDistance;
        Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * walkDistance;
        return transform.position + offset;
    }

    /// <summary>
    /// Attempts to look for a target.
    /// </summary>
    /// <returns>The target. If none was found/within range, null.</returns>
    protected GameObject LookForTarget() 
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(TargetObjectsWithTag);
        GameObject target = Funcs.GetClosestTargetTo(targets, gameObject);
        if (target != null && Vector3.Distance(target.transform.position, transform.position) < VisionRange)
            return target;
        return null;
    }

    /// <summary>
    /// Defines how the AI reacts when it spots an enemy.
    /// </summary>
    /// <param name="enemy">The enemy it spotted.</param>
    protected virtual void OnEnemySpotted(GameObject enemy) 
    {
        behavior = Behavior.Aggressive;
    }

    /// <summary>
    /// Defines how the AI acts when idle.
    /// </summary>
    protected virtual void Idle()
    {
        behavior = Behavior.Wander;
    }

    /// <summary>
    /// Defines how the AI acts when wandering.
    /// </summary>
    protected virtual void Wander()
    {
        // Set a random waypoint or count down waypointtimer
        if (waypointTimer <= 0f)
        {
            waypoint = GetWaypoint();
            waypointTimer = waypointCooldown;
        }
        else waypointTimer -= Time.deltaTime;

        // If we're already at the waypoint, don't do anything
        if (Vector3.Distance(waypoint, transform.position) < 1f)
            return;

        // Otherwise, walk towards the waypoint
        Vector3 vec = (waypoint - transform.position);
        vec.y = 0;
        Move((vec.normalized * Speed / 2f - rb.velocity) * WalkAcceleration * Time.deltaTime);
    }

    /// <summary>
    /// Defines how the AI acts when aggressive.
    /// </summary>
    /// <param name="enemy">The enemy it's aggressive towards.</param>
    protected virtual void Aggressive(GameObject enemy)
    {
        // If enemy is no longer to be found, change behavior and return.
        if (enemy == null)
        {
            behavior = Behavior.Idle;
            return;
        }
    }

    /// <summary>
    /// Defines how the AI acts when investigating.
    /// </summary>
    protected virtual void Investigate()
    {
        // Move towards the point of investigation
        Vector3 vec = waypoint - transform.position;
        vec.y = 0;
        Move((vec.normalized * Speed - rb.velocity) * WalkAcceleration * Time.deltaTime);

        // If we're at the point to investigate but nothing else happens, change back to wander
        if (vec.magnitude <= 5f)
        {
            behavior = Behavior.Wander;
        }
    }

    protected override void OnFixedUpdate()
    {
        // Count down charm duration and revert charm if it times out
        charmDuration -= Time.deltaTime;
        if (charmDuration <= 0f && prevTargetTag != "") 
        {
            TargetObjectsWithTag = prevTargetTag;
            prevTargetTag = "";
        } 

        // Look for a target
        GameObject target = LookForTarget();
        if (target != null)
            OnEnemySpotted(target);

        // Based on behavior, do different things
        switch (behavior)
        {
            case Behavior.Idle:
                Idle();
                break;

            case Behavior.Wander:
                Wander();
                break;

            case Behavior.Aggressive:
                Aggressive(target);
                break;

            case Behavior.Investigating:
                Investigate();
                break;

            default:
                // Switch to idle if unknown state
                behavior = Behavior.Idle;
                break;
        }
    }
}
