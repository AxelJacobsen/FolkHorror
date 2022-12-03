using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Green Troll AI.
/// </summary>
public class NisseAI : BaseEnemyAI
{
    // Public vars
    [Header("Nisse AI settings")]
    public float RunSpeedMult                 = 2f;
    public float AlertEnemiesRangeWithinRange = 75f;
    public float StopScreamingAfterSeconds    = 15f;

    // Private vars
    private Vector3     enemyLastSeen;
    private GameObject  runAndScreamTowards;
    private float       runTowardsDist = Mathf.Infinity,
                        screamingTimer = 0f;
    [SerializeField] private AudioClip onScreamClip;
    protected override void OnStart()
    {
        base.OnStart();
    }

    /// <summary>
    /// Run away, screaming and alerting its allies about where the enemies are.
    /// </summary>
    /// <param name="enemy">The enemy.</param>
    protected override void Aggressive(GameObject enemy)
    {
        // Check if we still are aggressive
        if (enemy == null && screamingTimer <= 0f)
        {
            behavior = Behavior.Idle;
            runAndScreamTowards = null;
            runTowardsDist = Mathf.Infinity;
            return;
        }

        // If we're screaming and running...
        if (screamingTimer > 0f)
        {
            if (enemy != null)
                enemyLastSeen = enemy.transform.position;

            // Look for enemies to alert...
            GameObject[] enemyTaggedObjects = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject obj in enemyTaggedObjects)
            {
                // Skip if the object isn't an Ai or is self
                BaseEnemyAI objAIScript = obj.GetComponent<BaseEnemyAI>();
                if (objAIScript == null || obj == gameObject) continue;

                // Skip if the AI isn't an ally to us
                if (objAIScript.TargetObjectsWithTag != TargetObjectsWithTag) continue;

                // Skip if the object is outside of alert range
                // (And check if it's the closest ally outside of range, in which case set it to be our destination)
                float distToObj = Vector3.Distance(obj.transform.position, transform.position);
                if (distToObj > AlertEnemiesRangeWithinRange)
                {
                    if (distToObj < runTowardsDist && 
                        objAIScript.behavior != BaseEnemyAI.Behavior.Aggressive && 
                        objAIScript.behavior != BaseEnemyAI.Behavior.Investigating )
                    {
                        runAndScreamTowards = obj;
                        runTowardsDist = distToObj;
                    }
                    continue;
                }


                // Alert them
                objAIScript.StartInvestigating(enemyLastSeen);
            }

            // If our current alert-target is alerted, find a new one
            if (runAndScreamTowards != null)
            {
                Behavior alertTargetBehavior = runAndScreamTowards.GetComponent<BaseEnemyAI>().behavior;
                if (alertTargetBehavior == Behavior.Aggressive || alertTargetBehavior == Behavior.Investigating)
                {
                    runAndScreamTowards = null;
                    runTowardsDist = Mathf.Infinity;
                }
            }

            Vector3 dir;
            // If there's no new allies to alert, run away from the enemy
            if (runTowardsDist == Mathf.Infinity) { dir = transform.position - enemyLastSeen; dir.y = 0; StartCoroutine(SoundManager.Instance.InterruptAfter(.2f)); }

            // Otherwise, run towards allies to alert
            else { dir = runAndScreamTowards.transform.position - transform.position; dir.y = 0; }

            // Run and return
            Move((dir.normalized * Speed * RunSpeedMult - rb.velocity) * WalkAcceleration * Time.deltaTime);
            return;
        }

        // Otherwise, if we just found a new enemy
        else
        {
            // Start screaming
            screamingTimer = StopScreamingAfterSeconds;
            runAndScreamTowards = null;
            runTowardsDist = Mathf.Infinity;
            // Play Sound
            AudioSource sound = SoundManager.Instance.PlaySound(onScreamClip, gameObject.transform);
            sound.volume = 0.1f;
        }
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        // Count down scream-timer
        if (screamingTimer > 0f) screamingTimer -= Time.deltaTime;
    }
}
