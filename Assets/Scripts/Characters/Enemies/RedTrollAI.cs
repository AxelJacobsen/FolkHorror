using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Red Troll AI.
/// </summary>
public class RedTrollAI : BaseEnemyAI
{
    // Public vars
    [Header("Red troll AI settings")]
    public float    StampedeWithinRange = 25f;
    public float    StampedeMaxDamage   = 30f;
   
    protected override void OnStart()
    {
        base.OnStart();
    }

    /// <summary>
    /// Run towards enemy and charge when close enough.
    /// </summary>
    /// <param name="enemy">The enemy.</param>
    protected override void Aggressive(GameObject enemy)
    {
        // Check if we still are aggressive
        base.Aggressive(enemy);
        if (behavior != BaseEnemyAI.Behavior.Aggressive) return;

        Character enemyCharacterScript = enemy.GetComponent<Character>();

        // Check if we're within range to "stampede" (roll)
        float distanceToEnemy = Vector3.Distance(enemy.transform.position, transform.position);
        Vector3 directionToEnemy = (enemy.transform.position - transform.position);
        directionToEnemy.y = 0;
        directionToEnemy = directionToEnemy.normalized;

        // Stampede (roll) if we can
        bool rolling = false;
        if (distanceToEnemy <= StampedeWithinRange)
            rolling = SteerableRoll(directionToEnemy);

        // If we're rolling
        if (rolling)
        {
            // If close enough to the enemy, deal dmg and knockback.
            if (distanceToEnemy <= 3f) 
            {
                enemyCharacterScript.Knockback(directionToEnemy * rb.velocity.magnitude / 2f);
                float mult = Vector3.Dot(rb.velocity.normalized, directionToEnemy);
                enemyCharacterScript.Hurt(gameObject, mult * StampedeMaxDamage);
            }

        }

        // If we're not rolling
        else
        {
            // Attack with weapon if possible
            if (Weapon != null && Weapon.InRangeOf(enemy) && Weapon.CanAttack()) 
            {    
                Attack(enemy.transform.position, TargetObjectsWithTag);

            // Otherwise, just move towards the enemy
            }
            else Move((directionToEnemy * Speed - rb.velocity) * WalkAcceleration * Time.deltaTime);
        }
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }
}
