using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Green Troll AI.
/// </summary>
public class GreenTrollAI : BaseEnemyAI
{
    // Public vars
    [Header("Green troll AI settings")]
    public float    AttackRange         = 25f;
    public float    RunAwaySpeedMult    = 2f;
    public float    RunAwayRange        = 10f;
    public float    RunAwayMaxSeconds   = 3f;
    public float    RunAwayCooldown     = 5f;

    // Private vars
    private float   RunAwayTimer = 0f,
                    RunAwayCooldownTimer = 0f,
                    RunAwayTimer2 = 0f;

    protected override void OnStart()
    {
        base.OnStart();
    }

    /// <summary>
    /// Run towards enemy until within attack range, then 
    /// </summary>
    /// <param name="enemy">The enemy.</param>
    protected override void Aggressive(GameObject enemy)
    {
        // Check if we still are aggressive
        base.Aggressive(enemy);
        if (behavior != BaseEnemyAI.Behavior.Aggressive) return;

        Character enemyCharacterScript = enemy.GetComponent<Character>();

        float distanceToEnemy = Vector3.Distance(enemy.transform.position, transform.position);
        Vector3 directionToEnemy = (enemy.transform.position - transform.position);
        directionToEnemy.y = 0;
        directionToEnemy = directionToEnemy.normalized;

        // If we're in run-mode...
        if (RunAwayTimer2 > 0f)
        {
            Move((-directionToEnemy * Speed * RunAwaySpeedMult - rb.velocity) * WalkAcceleration * Time.deltaTime);
            if (RunAwayCooldownTimer <= 0f && RunAwayTimer <= 0f)
                RunAwayTimer = RunAwayMaxSeconds;
            return;
        }

        // If we're too close, start running
        if (distanceToEnemy <= RunAwayRange && RunAwayTimer <= 0f && RunAwayCooldownTimer <= 0f)
        {
            RunAwayTimer2 = 1f;
            return;
        }

        // If we're within range, attack
        if (distanceToEnemy <= AttackRange)
        {
            if (Weapon != null && Weapon.InRangeOf(enemy) && Weapon.CanAttack()) 
                Attack(enemy.transform.position, TargetObjectsWithTag);
            return;
        }

        // If we're outside of attack range, move closer
        Move((directionToEnemy * Speed - rb.velocity) * WalkAcceleration * Time.deltaTime);
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        // Count down run-timers
        if (RunAwayTimer2 > 0f) RunAwayTimer2 -= Time.deltaTime;
        if (RunAwayTimer > 0f)
        {
            RunAwayTimer -= Time.deltaTime;
            if (RunAwayTimer <= 0f)
                RunAwayCooldownTimer = RunAwayMaxSeconds;
        }
        if (RunAwayCooldownTimer > 0f) RunAwayCooldownTimer -= Time.deltaTime;
    }
}
