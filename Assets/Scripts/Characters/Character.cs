using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for handling any type of <c>Character</c>.
/// </summary>
public abstract class Character : CharacterStats
{
	// Public vars
    [Header("Items")]
	public Weapon				Weapon;
	public List<Item>			Items;
   
    [Header("Walking/jogging")]
	public float 				WalkAcceleration = 10f;


    [Header("Rolling")]
	public float 				RollDuration = 0.2f;
	public float				RollCooldown = 1.5f;
	public float				RollSpeed 	 = 50f;
	public float 				RollAcceleration = 3f;
	public EffectEmitter		DustEffectEmitter;

    [Header("Sounds")]
    [SerializeField] protected AudioClip WalkSound;
    [SerializeField] protected AudioClip RollSound;
    [SerializeField] protected AudioClip HurtSound;
    [SerializeField] protected AudioClip StunSound;
    [SerializeField] protected AudioClip DeathSound;
    [SerializeField] protected AudioClip AttackSound;

    // Private vars
    protected CharacterStats	baseStats;

    [Header("Drops")]
    public GameObject[]         DropsOnDeath;
	protected float				Health;
	protected Rigidbody 		rb;
	protected Animator 		    anim;
	protected SpriteRenderer    sr;
    protected bool 			    facingRight;
	protected float 		    flashing;
	protected List<EffectData> 	effects = new List<EffectData>(); // Buffs & Debuffs

	protected bool	stunnedThisUpdate = false;
    protected float stunDuration = 0f,
                    rollTimer = 0f,
                    rollTimerCD = 0f;

    protected Vector3 walkDir = new Vector3(0,0,0);
	protected Vector3 rollDir = new Vector3(0,0,0);

    protected BoxCollider hitbox;
    protected EffectEmitter myDustEffectEmitter;

    public bool dead = false;
    private float   onRollTimer = 0f,
                    onHurtTimer = 0f,
                    onAttackTimer = 0f,
                    onDieTimer = 0f,
                    onStunTimer = 0f;

    protected abstract void OnStart();
    void Start()
	{
        // Fetch components
        rb = GetComponent<Rigidbody>();
		if (rb == null) 	Debug.LogError(gameObject.name + "(Character) could not find its rigidbody!");

		anim = GetComponent<Animator>();
		if (anim == null) 	Debug.LogError(gameObject.name + "(Character) could not find its animator!");

		sr = GetComponentInChildren<SpriteRenderer>();
		if (sr == null) 	Debug.LogError(gameObject.name + "(Character) could not find its sprite renderer!");

		// Fetch hitbox and apply it to emitter
        foreach (Transform child in transform) {
            if (child.gameObject.tag != "Hitbox") continue;
            BoxCollider childBoxCollider = child.gameObject.GetComponent<BoxCollider>();
            if (childBoxCollider == null) continue;
            hitbox = childBoxCollider;
            break;
        }

        if (hitbox == null) hitbox = GetComponent<BoxCollider>();
        if (hitbox == null) Debug.LogError(gameObject.name + "(Character) could not find its hitbox!");

        // Dynamically create a copy of the given scriptableObject (dustemitter) which belongs to ONLY US
        myDustEffectEmitter = Instantiate(DustEffectEmitter);

        myDustEffectEmitter._Hitbox = hitbox;
        myDustEffectEmitter._Active = true;
        myDustEffectEmitter.SizeFunc = f => f*0.7f + 0.3f;
        myDustEffectEmitter.AlphaFunc = f => 0.5f - f*0.5f;

        // Set base stats to be starting stats
        baseStats = base.Copy();

		Health = MaxHealth;
		facingRight = false;
		flashing = 0;

        // Call children's start
        OnStart();
    }

    /// <summary>
    /// Gets Character's current health
    /// </summary>
    /// <returns>Current health as float</returns>
    public float GetCurrentHealth() { return Health; }

	/// <summary>
    /// Updates the player's stats with their current items.
    /// </summary>
	public void UpdateStats()
    {
		// Gather a list of all stat-changes
		List<AlterStats> alterStatsList = new List<AlterStats>();
		foreach (Item item in Items)
        {
			if (item is Passiveitem)
            {
				Passiveitem passiveItem = (Passiveitem)item;
				if (passiveItem.alterStats != null)
					alterStatsList.Add(passiveItem.alterStats);
            }
        }

		CharacterStats newStats = baseStats.CalculateStats(alterStatsList);
		SetStats(newStats);
    }

	/// <summary>
    /// Kills the character.
    /// </summary>
	public virtual void Die() 
	{
        if (dead) return;
        else dead = true;

        foreach (GameObject obj in DropsOnDeath)
        {
            GameObject drop = Instantiate(obj, transform.position, Quaternion.identity);
        }

        OnDie();
        Destroy(gameObject);
    }

	/// <summary>
    /// Hurts the character.
    /// If its Health is reduced below zero, kills it.
    /// </summary>
    /// <param name="caller">The object which caused the player to be hurt.</param>
    /// <param name="amount">The amount of damage the player takes.</param>
    /// <return>The character's health after taking damage.</return>
	public virtual float Hurt(GameObject caller, float amount) 
	{
        anim.SetTrigger("Hurt");

        // Invoke item triggers
        foreach (Item item in Items) { item.OnPlayerGetHit(caller, amount); }

		Health -= amount;
		if (Health <= 0 && !dead) {
			Die();
		}

        OnHurt();
        return Health;
        //flashing = 0.25f;
    }

	/// <summary>
    /// Heals the character.
    /// </summary>
    /// <param name="caller">The object which caused the player to be healed.</param>
    /// <param name="amount">The amount of healing.</param>
    /// <return>The character's health after healing.</return>
	public float Heal(GameObject caller, float amount) {
		if (Health < MaxHealth) Health = Mathf.Min(Health+amount, MaxHealth);
        return Health;
	}

	/// <summary>
    /// Knocks back the character.
    /// </summary>
    /// <param name="amount"></param>
	public void Knockback(Vector3 amount)
    {
		rb.velocity += amount;
		// Apply stun?
    }

	/// <summary>
    /// Stuns the character.
    /// Also cancels rolls.
    /// </summary>
    /// <param name="duration">How long to stun the character for.</param>
	public void Stun(float duration) 
	{
        stunDuration += duration;
		rollTimer = 0f;

        OnStun();
    }

	/// <summary>
    /// Makes the character attack with its weapon.
    /// </summary>
    /// <param name="targets">An array containing all possible targets.</param>
    public void Attack(Vector3 aimPosition, string targetTag) 
	{
		// If stunned, return
		if (stunDuration > 0f) return;

        anim.SetTrigger("Attack");

        // Invoke item triggers
        foreach (Item item in Items) { item.OnPlayerAttack(aimPosition, targetTag); }

        // Attacking with a weapon
        if (Weapon != null) {
            Weapon.Attack(aimPosition, targetTag);

        // Attacking with no weapon
        } else {
			// ...
		}

        OnAttack();
    }

	/// <summary>
    /// Applies an effect to the character.
    /// If the effect is already applied, the largest duration is used (old or new) and the intensity is added. <summary>
    /// </summary>
    /// <param name="effect">The effect to apply.</param>
    /// <param name="creator">Its creator - whichever gameObject placed the effect onto this.</param>	
	public void ApplyEffect(EffectData effect, GameObject creator)
    {
        // Check if the effect is already applied
        foreach (EffectData effectData in effects) {

			// If so, set duration to max(previous, new) and add intensity.
			if (effectData.name == effect.name) {
                effectData._Duration = Mathf.Max(effectData._Duration, effect.BaseDuration);
                effectData._Intensity += effect.BaseIntensity;
                effectData.OnReapply();
                return;
            }
		}

		// Otherwise, add the new effect.
        effect._Creator = creator;
        effect._Target 	= gameObject;
        effect._Intensity = effect.BaseIntensity;
		effect._Duration = effect.BaseDuration;
        effect.OnBegin();
        effects.Add(effect);
    }

	/// <summary>
    /// Moves the character, applying a given velocity to their current one.
    /// </summary>
    /// <param name="velocity">The velocity to add to the player.</param>
	protected void Move(Vector3 velocity) 
	{
        anim.SetBool("Walking", true);
        anim.SetBool("Running", false);

		// If we're stunned, don't change velocity
		if (stunDuration > 0f) return;

		// Otherwise, update movedir
        walkDir = velocity;
    }

	/// <summary>
    /// Checks if the character can roll right now.
    /// </summary>
    /// <returns>True if the player can roll right now.</returns>
	public bool CanRoll() 
	{
		// If we're stunned, don't roll
		if (stunDuration > 0f) return false;
		return rollTimerCD <= 0f;
    }

	/// <summary>
    /// Rolls in a given direction.
    /// </summary>
    /// <param name="currentDirection">The direction we're trying to roll in now.</param>
    /// <returns>True if we're rolling, false otherwise.</returns>
	protected bool SteerableRoll(Vector3 currentDirection) 
	{
        anim.SetBool("Running", true);
        anim.SetBool("Walking", false);

		// If we're not rolling and can roll, start rolling.
		if (rollTimer <= 0f && CanRoll()) {
			rollTimer = RollDuration;
            rollDir = currentDirection;

            OnRoll();
        }

		// If we're not rolling, return false (not rolling)
		if (rollTimer <= 0f) return false;

        // Set direction
        rollDir += (currentDirection - rollDir) * RollAcceleration * Time.deltaTime;

        return true;
    }

    protected abstract void OnFixedUpdate();
    protected void FixedUpdate()
	{
        // Stun timer
        if (stunDuration > 0f)
        {
            stunnedThisUpdate = true;
            stunDuration -= Time.deltaTime;
        }

        // Roll timers
        if (rollTimer > 0f)
        {
            rollTimer -= Time.deltaTime;
			// Start cooldown if we just ticked down to <0
			if (rollTimer <= 0f) rollTimerCD = RollCooldown;
        }
        if (rollTimerCD > 0f) rollTimerCD -= Time.deltaTime;

        // Other timers
        onRollTimer     -= Time.deltaTime;
        onHurtTimer     -= Time.deltaTime;
        onAttackTimer   -= Time.deltaTime;
        onDieTimer      -= Time.deltaTime;
        onStunTimer     -= Time.deltaTime;

		// Movement - stunned
		if (stunnedThisUpdate) 
		{
            rb.velocity -= rb.velocity * 3f * Time.deltaTime;
        }
		// Movement - rolling
		else if (rollTimer > 0f) 
		{
            if (myDustEffectEmitter != null)
                myDustEffectEmitter.Emit(Time.deltaTime);	

            rb.velocity = rollDir.normalized * RollSpeed;
        }
		// Movement - walking
		else
		{
            rb.velocity += (walkDir * Speed - rb.velocity) * WalkAcceleration * Time.deltaTime;
            walkDir = Vector3.zero;
        }

        // Flip sprite if we're changing direction
        float 	max = Mathf.Max(rb.velocity.x, rb.velocity.z),
				min = Mathf.Min(rb.velocity.x, rb.velocity.z);
		int		curDir = max > -min ? (rb.velocity.x > rb.velocity.z ? 3 : 0) : (rb.velocity.x < rb.velocity.z ? 2 : 1);
		
		if (curDir == 2 && !facingRight || curDir == 3 && facingRight) {
			Vector3 tvec = transform.localScale;
			tvec.x *= -1;
			transform.localScale = tvec;
			facingRight = !facingRight;
		}

		// Flash
		if (flashing > 0) {
			flashing -= Time.deltaTime;
			Color tCol = sr.color;

			if (flashing <= 0) {
				tCol.a = 1;
			} else {
				tCol.a = Mathf.Sin(flashing * 25);
			}

			sr.color = tCol;
		}

        // Handle effects
        List<EffectData> effectsEnded = new List<EffectData>();
        foreach (EffectData effect in effects) 
		{
			// Check if the effect has expired
            effect._Duration -= Time.deltaTime;
            if (effect._Duration <= 0f)
            {
                effectsEnded.Add(effect);
                continue;
            }

            // Apply during-effect
            effect.During(Time.deltaTime);
        }

		// Delete effects that have timed out
		foreach (EffectData effectEnded in effectsEnded) 
		{
            effectEnded.OnEnd();
            effects.Remove(effectEnded);
        }

        // Remove this-update-effects
        stunnedThisUpdate = false;

        // Call children's FixedUpdate...
        OnFixedUpdate();
    }

	// Item triggers
	public virtual void OnPlayerAttack(Vector3 aimPosition, string targetTag){}
    public virtual void OnPlayerHit(GameObject target){}
    public virtual void OnPlayerGetHit(GameObject hitBy, int amount){}

    // Sound triggers
    protected virtual void OnRoll()  { if (onRollTimer <= 0f)   { SoundManager.Instance.PlaySound(RollSound); onRollTimer = 1f; } }
    protected virtual void OnHurt()  { if (onHurtTimer <= 0f)   { SoundManager.Instance.PlaySound(HurtSound); onHurtTimer = 1f; }}
    protected virtual void OnAttack(){ if (onAttackTimer <= 0f) { SoundManager.Instance.PlaySound(AttackSound); onAttackTimer = 1f; }}
    protected virtual void OnDie()   { if (onDieTimer <= 0f)    { SoundManager.Instance.PlaySound(DeathSound); onDieTimer = 1f; }}
    protected virtual void OnStun()  { if (onStunTimer <= 0f)   { SoundManager.Instance.PlaySound(StunSound); onStunTimer = 1f; }}
}