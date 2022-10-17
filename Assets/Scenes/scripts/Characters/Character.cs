using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : CharacterStats
{
	// Public vars
    [Header("Items")]
	public Weapon				Weapon;
	public List<Item>			Items;			// Potential pointer error? Check

	// Private vars
	protected CharacterStats	baseStats;
	protected Rigidbody 		rb;
	protected Animator 		    anim;
	protected SpriteRenderer    sr;
	protected bool 			    facingRight;
	protected float 		    flashing;

	protected void Start()
	{
		// Fetch components
        rb = GetComponent<Rigidbody>();
		if (rb == null) 	Debug.LogError("Character could not find its rigidbody!");

		anim = GetComponent<Animator>();
		if (anim == null) 	Debug.LogError("Character could not find its animator!");

		sr = GetComponentInChildren<SpriteRenderer>();
		if (sr == null) 	Debug.LogError("Character could not find its sprite renderer!");

		// Set base stats to be starting stats
		baseStats = base.Copy();

		facingRight = false;
		flashing = 0;
	}


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
		CharacterStats dmgtaken = this - (newStats - baseStats);
		
		// TODO: Add some kind of way to keep damage taken when equipping an item.

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
    /// <param name="amount">T</param>
	public void Hurt(int amount) 
	{
		Health -= amount;
		if (Health <= 0) {
			Die();
			return;
		}
		//flashing = 0.25f;
	}

	public void Knockback(Vector3 amount)
    {
		rb.velocity += amount;
		// Apply stun?
    }

	/// <summary>
    /// Makes the character attack with its weapon.
    /// </summary>
    /// <param name="targets">An array containing all possible targets.</param>
    protected void Attack(Vector3 aimPosition, string targetTag) 
	{
		// Attacking with a weapon
		if (Weapon != null) {
            Weapon.Attack(aimPosition, targetTag);

        // Attacking with no weapon
        } else {
			// ...
		}
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

			//anim.SetInteger("walkDir", walkDir);
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
	}
}