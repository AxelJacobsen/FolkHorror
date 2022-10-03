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
	public int		MaxHealth = 100,
                    AttackDamage = 50;
	
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
    protected float             attackCooldown;

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

    /**
     *  Gets the closest GameObject with the given tag.
     *
     *  @param tag - The tag.
     *  @return The closest GameObject with the tag.
     */
    protected GameObject GetClosestTarget(string tag) {
        // Fetch all objects with the given tag
        GameObject[] tObjects = GameObject.FindGameObjectsWithTag(tag);

        // Iterate them, finding the closest
        float       minDistSqr = Mathf.Infinity;
        GameObject  retObj = null;
        foreach (GameObject obj in tObjects) {
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

	/**
	 *	Kills the Character.
	 */
	void Die() {
        sr.color = new Color(0,0,0,0);
        this.enabled = false;
	}

	/**
	 *	Hurts the Character.
     *  If its health is reduced below zero, dies.
     *  @see Die()
	 *
	 *	@param amount - The amount of health the Character will lose.
	 */
	public void Hurt(int amount) {
		health -= amount;
		if (health <= 0) {
			Die();
			return;
		}
		//flashing = 0.25f;
	}

    /**
     *  Attacks a target.
     *
     *  @param target - The target to attack.
     */
    protected void Attack(GameObject target) {
		if (Weapon != null) Weapon.Drop();

        // Play animation
        anim.SetTrigger("attack");
        attackCooldown = 1f / AttackSpeed;

		// If the target is invalid or out of range, return
		if ( target == null ) return;
        Rigidbody tRB = target.GetComponent<Rigidbody>();
		if ( (tRB.position - rb.position).magnitude > AttackRange ) return;

		// Damage target and knock them back
        target.GetComponent<Character>().Hurt(AttackDamage);
        tRB.velocity += (tRB.position - rb.position).normalized * 50f;
    }

	protected void FixedUpdate()
	{
        // Count down attack timer
        if (attackCooldown > 0) attackCooldown -= Time.deltaTime;

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