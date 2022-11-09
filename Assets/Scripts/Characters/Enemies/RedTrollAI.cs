using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Red Troll AI.
/// </summary>
public class RedTrollAI : BaseEnemyAI
{
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

        // Attack
        Vector3 dir = (enemy.transform.position - transform.position).normalized;
        dir.y = 0;
        Move((dir.normalized * Speed - rb.velocity) * WalkAcceleration * Time.deltaTime);
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }
}
