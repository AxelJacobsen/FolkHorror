using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : CharacterStats
{
	// Public vars
    [Header("Items")]
	public Weapon				Weapon;
	public List<Item>			Items;

	// Private vars
	protected CharacterStats	baseStats;
	protected int				Health;
	protected Rigidbody 		rb;
	protected Animator 		    anim;
	protected SpriteRenderer    sr;
    protected EffectDataLoader 	effectLoader;
    protected bool 			    facingRight;
	protected float 		    flashing;
	protected Dictionary<string, (float, float)> effects = new Dictionary<string, (float, float)>(); // Buffs & Debuffs. Stored in the format "effectName" : (duration, intensity)

	protected void Start()
	{
		// Fetch components
        rb = GetComponent<Rigidbody>();
		if (rb == null) 	Debug.LogError("Character could not find its rigidbody!");

		anim = GetComponent<Animator>();
		if (anim == null) 	Debug.LogError("Character could not find its animator!");

		sr = GetComponentInChildren<SpriteRenderer>();
		if (sr == null) 	Debug.LogError("Character could not find its sprite renderer!");

        GameObject effectLoaderObject = GameObject.FindGameObjectWithTag("EffectLoader");
        effectLoader = effectLoaderObject.GetComponent<EffectDataLoader>();
		if (effectLoader == null) Debug.LogError("Character could not find an effectloader!");

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
	public int Hurt(GameObject caller, int amount) 
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
	public int Heal(GameObject caller, int amount) {
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

	/// <summary>
    /// Makes the character attack with its weapon.
    /// </summary>
    /// <param name="targets">An array containing all possible targets.</param>
    public void Attack(Vector3 aimPosition, string targetTag) 
	{
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
    /// Applies an ontrigger-effect.
    /// Used by ApplyEffect & HandleEffect.
    /// </summary>
    /// <param name="trigger">The trigger & its effects.</param>
    /// <param name="intensity">The intensity</param>
	private void applyTriggerEffect(OnTrigger trigger, float intensity) {
		if (trigger == null) return;
        Hurt(gameObject, trigger.flatDamage * (int)(intensity * Time.deltaTime));
		Hurt(gameObject, MaxHealth * trigger.percentDamage * (int)(intensity * Time.deltaTime) / 100);
	}

	/// <summary>
    /// Applies an effect to the character.
    /// If the effect is already applied, the largest duration is used (old or new) and the intensity is added.
    /// </summary>
    /// <param name="effectName">The name of the effect.</param>
    /// <param name="duration">The duration of the effect (in seconds).</param>
    /// <param name="intensity">The intensity of the effect.</param>
    /// <returns></returns>
	public (float, float) ApplyEffect(string effectName, float duration, float intensity)
    {
        try
        {
			(float, float) value = effects[effectName];
			effects[effectName] = (Mathf.Max(value.Item1, duration), value.Item2 + intensity);
			return (effects[effectName].Item1, effects[effectName].Item2);
		}
		catch (KeyNotFoundException)
        {
			// Apply "begin"-trigger effects
			EffectData effectData = effectLoader.GetEffectData(effectName);
			if (effectData != null) applyTriggerEffect(effectData.GetTrigger("begin"), intensity);
			
            effects[effectName] = (duration, intensity);
			return (duration, intensity);
        }
    }

	/// <summary>
	/// Handles a specific buff/debuff.
	/// </summary>
	/// <param name="kvp">The key-value-pair of the effect, found in the 'effects' dictionary.</param>
    /// <returns>A tuple consisting of the newly updated (effectName, (duration, intensity)).</returns>
	protected (string, (float, float)) HandleEffect(KeyValuePair<string, (float, float)> kvp)
	{
		string		effectName	 = kvp.Key;
		float		duration	 = kvp.Value.Item1,
					intensity	 = kvp.Value.Item2;
		EffectData 	effectData 	 = effectLoader.GetEffectData(effectName);
		Effect		effectScript = null;

		// Iterate all Effect components, looking for the one with matching name.
		Effect[] effectScripts = GetComponents<Effect>();
		foreach (Effect effectScript_t in effectScripts)
		{
			if (effectScript_t.EffectName == effectName)
			{
				effectScript = effectScript_t;
				break;
			}
		}

        // Count down effect duration and end the effect if it timed out.
        duration -= Time.deltaTime;
		if (duration <= 0)
		{
			// Apply "end"-trigger effects
			if (effectData != null) applyTriggerEffect(effectData.GetTrigger("end"), intensity);

			if (effectScript != null) effectScript._Active = false;
			return (null, (0, 0));
		}

		// Alter effect power based on intensity
		if (effectScript != null)
		{
			if (!effectScript._Active) effectScript._Active = true;
			effectScript.Rate = 5 * intensity;
		}

		// Apply "during"-trigger effects
		if (effectData != null) applyTriggerEffect(effectData.GetTrigger("during"), intensity);

		// Update effect and return
		return (effectName, (duration, intensity));
	}

	protected void FixedUpdate()
	{
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

		// Effects (buffs/debuffs)
		Dictionary<string, (float, float)> updatedEffects = new Dictionary<string, (float, float)>();
		foreach (KeyValuePair<string, (float, float)> kvp in effects)
        {
			(string, (float, float)) updatedEffect = HandleEffect(kvp);
			if (updatedEffect.Item1 != null) updatedEffects.Add(updatedEffect.Item1, updatedEffect.Item2);
		}
		effects = updatedEffects;
	}

	// Item triggers
	public virtual void OnPlayerAttack(Vector3 aimPosition, string targetTag){}
    public virtual void OnPlayerHit(GameObject target){}
    public virtual void OnPlayerGetHit(GameObject hitBy, int amount){}
}