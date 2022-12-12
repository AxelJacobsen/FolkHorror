using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Red Troll AI.
/// </summary>
public class BossTrollAI : BaseEnemyAI
{
    // Public vars
    [Header("Attacks")]
    public GameObject LavaSpirePrefab;
    public GameObject RollingRockPrefab;

    [Header("Attack costs")]
    public float    Stamina       = 10f;
    public float    LavaSpireCost = 1f;
    public float    ThrowRockCost = 1f;
    public float    StompCost     = 0.5f;

    [Header("Attack minimum cooldowns")]
    public float    LavaSpireCooldown = 3f;
    public float    ThrowRockCooldown = 3f;
    public float    StompCooldown     = 7f;
    public float    BreathePeriod     = 2f;

    [Header("Stomp attack")]
    public float    StompRadius     = 30f;
    public float    StompDamage     = 30f;
    public float    StompKnockback  = 40f;

    [Header("Sounds")]
    [SerializeField] private AudioClip StompStartSound;
    [SerializeField] private AudioClip StompHitGroundSound;

    // Private vars
    private bool    isStomping = false;

    private float   throwRockTimer = 0f,
                    lavaSpireTimer = 0f,
                    stompTimer     = 0f,
                    breatheTimer   = 0f;

    private float   lavaSpireWeight = 0f,
                    throwRockWeight = 0f;
   
    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        throwRockTimer -= Time.deltaTime;
        lavaSpireTimer -= Time.deltaTime;
        stompTimer     -= Time.deltaTime;
        breatheTimer   -= Time.deltaTime;

        // Regen (some) stamina passively
        Stamina += 0.2f * Time.deltaTime;

        if (isStomping)
        {
            myDustEffectEmitter.Emit(Time.deltaTime);
        }
    }

    public override void Charm(string newTargetTag, float duration)
    {
        // Do nothing, as charming the boss isn't possible
    }

    /// <summary>
    /// ...
    /// </summary>
    /// <param name="enemy">The enemy.</param>
    protected override void Aggressive(GameObject enemy)
    {
        // If mid-attack, don't change behavior
        if (isStomping) return;

        // Check if we still are aggressive
        base.Aggressive(enemy);
        if (behavior != BaseEnemyAI.Behavior.Aggressive) return;

        // Check if the enemy still exists and is valid
        if (enemy == null) return;
        Character enemyCharacterScript = enemy.GetComponent<Character>();
        if (enemyCharacterScript == null) return;

        // Choose attack
        Vector3 v = (enemy.transform.position - transform.position);
        float   enemyDist = v.magnitude;
        v.y = 0;

        // If the enemy is too close, stomp
        if (stompTimer <= 0f && enemyDist <= StompRadius * 0.7f && breatheTimer <= 0f)
        {
            Stomp();
        }
        // Otherwise, choose semi-randomly between throwing rocks and spawning spires
        // (If thats not possible, just walk.)
        else
        {
            if (Random.Range(1f - lavaSpireWeight, 1f + throwRockWeight) > 0 && throwRockTimer <= 0f && Stamina > ThrowRockCost && breatheTimer <= 0f)
            {
                ThrowRock(v.normalized * 25f);
                throwRockWeight += 1f;
            }
            else if (lavaSpireTimer <= 0f && Stamina > LavaSpireCost && breatheTimer <= 0f)
            {
                Vector3 pos = enemy.transform.position - new Vector3(0,1.1f,0);
                Rigidbody enemyRB = enemy.GetComponent<Rigidbody>();
                if (enemyRB != null) pos += enemyRB.velocity * 0.8f;
                SpawnLavaSpire(pos);
                lavaSpireWeight += 1f;
            }
            else
            {
                Move((v.normalized * Speed - rb.velocity) * WalkAcceleration * Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// Throws a rock, if possible.
    /// </summary>
    /// <param name="velocity">The velocity of the rock.</param>
    /// <returns>True if the rock was successfully thrown.</returns>
    bool ThrowRock(Vector3 velocity)
    {
        // Return false if the ability is on cooldown. Otherwise, set CD and stamina.
        if (throwRockTimer >= 0f || Stamina < ThrowRockCost) return false;
        throwRockTimer = ThrowRockCooldown;
        Stamina -= ThrowRockCost;
        breatheTimer = BreathePeriod;

        anim.SetTrigger("Attack");

        // Instantiate rock
        Vector3 pos = gameObject.transform.position + velocity.normalized * 5f;
        GameObject rollingrock = Instantiate(RollingRockPrefab, pos, Quaternion.identity);
        rollingrock.GetComponent<RollingRock>().Velocity = velocity;
        rollingrock.GetComponent<RollingRock>().IgnoreList.Add(gameObject);

        return true;
    }

    /// <summary>
    /// Spawns a lavaspire, if possible.
    /// </summary>
    /// <param name="position">The position to strike.</param>
    /// <returns>True if the lavaspire was successfully spawned.</returns>
    bool SpawnLavaSpire(Vector3 position)
    {
        // Return false if the ability is on cooldown. Otherwise, set CD and stamina.
        if (lavaSpireTimer >= 0f || Stamina < LavaSpireCost) return false;
        lavaSpireTimer = LavaSpireCooldown;
        Stamina -= LavaSpireCost;
        breatheTimer = BreathePeriod;

        anim.SetTrigger("Attack");

        // Instantiate lavaspire
        GameObject lavaspirespawn = Instantiate(LavaSpirePrefab, position, Quaternion.identity);
        return true;
    }

    /// <summary>
    /// Stomps, if possible.
    /// </summary>
    /// <returns>True if the boss successfully initiated a stomp attack.</returns>
    bool Stomp()
    {
        if (stompTimer >= 0f || Stamina < StompCost) return false;
        stompTimer = StompCooldown;
        Stamina -= StompCost;
        breatheTimer = BreathePeriod;

        anim.SetTrigger("Jump");

        rb.velocity = new Vector3(0, 7, 0);
        isStomping = true;

        SoundManager.Instance.PlaySound(StompStartSound, gameObject.transform);

        return true;
    }

    /// <summary>
    /// When the boss hits the ground (or player) while stomping, go boom.
    /// </summary>
    /// <param name="hit">Collision data.</param>
    void OnCollisionEnter(Collision hit)
    {
        if (isStomping && hit.gameObject.name != "RollingRock")
        {
            isStomping = false;

            // Damage and knockback all enemies in area
            GameObject[] possibleTargets = GameObject.FindGameObjectsWithTag(TargetObjectsWithTag);
            foreach (GameObject target in possibleTargets)
            {
                // Ignore anything else than characters
                Character targetCharacterScript = target.GetComponent<Character>();
                if (targetCharacterScript == null) return;

                if (Vector3.Distance(target.transform.position, transform.position) < StompRadius)
                {
                    targetCharacterScript.Hurt(gameObject, StompDamage);
                    Vector3 v = (target.transform.position - transform.position).normalized * StompKnockback;
                    v.y = 0;
                    targetCharacterScript.Knockback(v);
                }
            }

            // Play sound
            SoundManager.Instance.PlaySound(StompHitGroundSound, gameObject.transform);
        }
    }
}
