using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	// Public vars
	[Header("Stats")]
	public float 	Speed = 10f,
                    AttackSpeed = 0.5f,
                    AttackRange = 3f;
    public int 		MaxHealth = 100;

    [Header("Items")]
	public Weapon				Weapon;
	public List<Item>			Items;			// Potential pointer error? Check

	// Private vars
	protected Rigidbody 		rb;
	protected Animator 		    anim;
	protected SpriteRenderer    sr;
	protected int			    health;
	protected bool 			    facingRight;
	protected float 		    flashing;

	protected void Start()
	{
		// Fetch components
        //rb = GetComponentInChildren(typeof(Rigidbody)) as Rigidbody;
        rb = GetComponent<Rigidbody>();
		if (rb == null) 	Debug.LogError("Character could not find its rigidbody!");

		anim = GetComponent<Animator>();
		if (anim == null) 	Debug.LogError("Character could not find its animator!");

		sr = GetComponentInChildren<SpriteRenderer>();
		if (sr == null) 	Debug.LogError("Character could not find its sprite renderer!");

		health = MaxHealth;
		facingRight = false;
		flashing = 0;
	}

    /// <summary>
    /// Gets the closest valid target.
    /// </summary>
    /// <param name="targets">An array containing the list of possible targets.</param>
    /// <returns>The closest valid target in the list.</returns>
    protected GameObject GetClosestTarget(GameObject[] targets) 
	{
        // Iterate them, finding the closest
        float       minDistSqr = Mathf.Infinity;
        GameObject  retObj = null;
        foreach (GameObject obj in targets) {
            // Check that the object is enabled, skip if it isn't
            if (!obj.GetComponent<Character>().enabled) continue;

            // Get the rigidbody of the current object, skip if it doesn't exist
            Rigidbody tRB = obj.GetComponent<Rigidbody>();
            if (tRB == null) continue;
  
            // Get the distance to that rigidbody, skip if it's greater or equal to the current min dist
            Vector3 tp0 = tRB.position;
            Vector3 tp1 = rb.position;
            Vector3 dir = (tRB.position - rb.position);
            float distSqr = Mathf.Pow(dir.x, 2) + Mathf.Pow(dir.y, 2);
            if (distSqr >= minDistSqr) continue;
            
            // Set return obj and min dist to current
            minDistSqr = distSqr;
            retObj = obj;
        }

        // Return
        return retObj;
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
    /// If its health is reduced below zero, kills it.
    /// </summary>
    /// <param name="amount">T</param>
	public void Hurt(int amount) 
	{
		health -= amount;
		if (health <= 0) {
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
    protected void Attack(GameObject[] targets) 
	{
		// Attacking with a weapon
		if (Weapon != null) {
            Weapon.Attack(targets);

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