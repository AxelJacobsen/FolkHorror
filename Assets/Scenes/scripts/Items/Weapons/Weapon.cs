using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for weapons (ranger or melee).
/// </summary>
public class Weapon : Item
{
    // Public vars
    [Header("Stats")]
    public int      AttackDamage = 25;
    public float    AttackSpeed = 0.5f;
    
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
    /// Checks if the weapon is in range to attack a given target.
    /// </summary>
    /// <param name="target">The target</param>
    /// <returns>True if the weapon is in range to attack the given target.</returns>
    public bool InRangeOf(GameObject target) {
        return true;
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
    /// Attacks towards the given position, targetting only objects with the given tag.
    /// </summary>
    /// <param name="aimPosition">Position to attack towards.</param>
    /// <param name="targetTag">Objects to target.</param> 
    public virtual void Attack(Vector3 aimPosition, string targetTag) {
        // If the weapon is on cooldown, do nothing
        if (AttackCooldown > 0f) return;

        // Otherwise, attack
        anim.SetTrigger("attack");
        AttackCooldown = 1f / AttackSpeed;
    }
}
