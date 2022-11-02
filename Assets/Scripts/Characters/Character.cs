using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class Character : CharacterStats
{
	// Public vars
    [Header("Items")]
	public Weapon				Weapon;
	public List<Item>			Items;

	// Private vars
	protected CharacterStats	baseStats;
	protected float				Health;
	protected Rigidbody 		rb;
	protected Animator 		    anim;
	protected SpriteRenderer    sr;
    //protected EffectDataLoader 	effectLoader;
    protected bool 			    facingRight;
	protected float 		    flashing;
	protected List<EffectData> 	effects = new List<EffectData>(); // Buffs & Debuffs

    protected float stunDuration = 0f;

    protected void Start()
	{
		// Fetch components
        rb = GetComponent<Rigidbody>();
		if (rb == null) 	Debug.LogError("Character could not find its rigidbody!");

		anim = GetComponent<Animator>();
		if (anim == null) 	Debug.LogError("Character could not find its animator!");

		sr = GetComponentInChildren<SpriteRenderer>();
		if (sr == null) 	Debug.LogError("Character could not find its sprite renderer!");

        //GameObject effectLoaderObject = GameObject.FindGameObjectWithTag("EffectLoader");
        //effectLoader = effectLoaderObject.GetComponent<EffectDataLoader>();
		//if (effectLoader == null) Debug.LogError("Character could not find an effectloader!");

        // Set base stats to be starting stats
        baseStats = base.Copy();

		Health = MaxHealth;
		facingRight = false;
		flashing = 0;
    }

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
				alterStatsList.Add(passiveItem.alterStats);
            }
        }

		CharacterStats newStats = baseStats.CalculateStats(alterStatsList);
		SetStats(newStats);
    }

	/// <summary>
    /// Kills the character.
    /// </summary>
	void Die() 
	{
        sr.color = new Color(0,0,0,0);
        this.enabled = false;
	}

	/// <summary>
    /// Hurts the character.
    /// If its Health is reduced below zero, kills it.
    /// </summary>
    /// <param name="caller">The object which caused the player to be hurt.</param>
    /// <param name="amount">The amount of damage the player takes.</param>
    /// <return>The character's health after taking damage.</return>
	public float Hurt(GameObject caller, float amount) 
	{
		// Invoke item triggers
		foreach (Item item in Items) { item.OnPlayerGetHit(caller, amount); }

		Health -= amount;
		if (Health <= 0) {
			Die();
		}
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
		Health += amount;
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

	public void Stun(float duration) {
        stunDuration += duration;
    }

	/// <summary>
    /// Makes the character attack with its weapon.
    /// </summary>
    /// <param name="targets">An array containing all possible targets.</param>
    public void Attack(Vector3 aimPosition, string targetTag) 
	{
		// If stunned, return
		if (stunDuration > 0f) return;

        // Invoke item triggers
        foreach (Item item in Items) { item.OnPlayerAttack(aimPosition, targetTag); }

        // Attacking with a weapon
        if (Weapon != null) {
            Weapon.Attack(aimPosition, targetTag);

        // Attacking with no weapon
        } else {
			// ...
		}
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
        System.Type effectType = effect.GetType();
        foreach (EffectData effectData in effects) {

			// If so, set duration to max(previous, new) and add intensity.
			if (effectData.GetType() == effectType) {
                effectData._Duration = Mathf.Max(effectData._Duration, effect.BaseDuration);
                effectData._Intensity += effect.BaseIntensity;
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

	protected void Move(Vector3 velocity) 
	{
		// If we're stunned, don't move
		if (stunDuration > 0f) return;

		// Otherwise, update velocity
        rb.velocity += velocity;
    }

	protected void FixedUpdate()
	{
		// Stun
		if (stunDuration > 0f)
			stunDuration -= Time.deltaTime;

        // Flip sprite if we're changing direction
        float 	max = Mathf.Max(rb.velocity.x, rb.velocity.z),
				min = Mathf.Min(rb.velocity.x, rb.velocity.z);
		int		walkDir = max > -min ? (rb.velocity.x > rb.velocity.z ? 3 : 0) : (rb.velocity.x < rb.velocity.z ? 2 : 1);
		
		if (walkDir == 2 && facingRight || walkDir == 3 && !facingRight) {
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
	}

	// Item triggers
	public virtual void OnPlayerAttack(Vector3 aimPosition, string targetTag){}
    public virtual void OnPlayerHit(GameObject target){}
    public virtual void OnPlayerGetHit(GameObject hitBy, int amount){}
}