using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    // Public vars
    [Header("Stats")]
    public int      AttackDamage = 25;
    public float    AttackRange = 3f,
                    AttackSpeed = 0.5f;
    
    // Private vars
    protected float   AttackCooldown;

    protected void Start()
    {
        base.Start();
    }

    protected void FixedUpdate()
    {
        base.FixedUpdate();

        // Count down attack timer
        if (AttackCooldown > 0) AttackCooldown -= Time.deltaTime;
    }

    /// <summary>
    /// Returns if the weapon can be picked up right now.
    /// </summary>
    /// <returns>True if the weapon can be equipped right now.</returns>
    protected override bool CanBePickedUp()
    {
        return _Player.GetComponent<Character>().Weapon == null && base.CanBePickedUp();
    }

    /// <summary>
    /// Checks if the weapon is ready and able to make an attack.
    /// </summary>
    /// <returns>True if the weapon is ready to attack.</returns>
    public bool CanAttack() {
        return AttackCooldown <= 0f;
    }

    /// <summary>
    /// Causes the weapon to be picked up by the player.
    /// </summary>
    protected override void PickUp() {
        base.PickUp();

        _Player.GetComponent<Character>().Weapon = this;
    }

    /// <summary>
    /// Causes the weapon to be dropped.
    /// </summary>
    public override void Drop() {
        base.Drop();

        _Player.GetComponent<Character>().Weapon = null;
    }

    /// <summary>
    /// Attacks the targets which are in range.
    /// </summary>
    /// <param name="targets">An array containing every possible target.</param>    
    public virtual void Attack(GameObject[] targets) {
        // If the weapon is on cooldown, do nothing
        if (AttackCooldown > 0f) return;

        // Otherwise, attack
        anim.SetTrigger("attack");
        AttackCooldown = 1f / AttackSpeed;

        // Iterate all possible targets...
        foreach(GameObject target in targets) {
            // Return if it's invalid or out of range
            if ( target == null ) return;
            Rigidbody tRB = target.GetComponent<Rigidbody>();
            if ( (tRB.position - rb.position).magnitude > AttackRange ) return;

            // Damage target and knock them back
            target.GetComponent<Character>().Hurt(AttackDamage);
            tRB.velocity += (tRB.position - rb.position).normalized * 50f;
        }
    }
}
