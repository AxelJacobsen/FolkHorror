using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    // Public vars
    public GameObject archPrototype;

    public override void Attack(Vector3 aimPosition, string targetTag)
    {
        // If the weapon is on cooldown, do nothing
        if (AttackCooldown > 0f) return;

        // Create a damage arch angled towards the aimposition
        Vector3 dir = (aimPosition - transform.position).normalized;
        dir.y = 0;

        float   ang = Mathf.Atan2(dir.x, dir.z) * 180f / Mathf.PI;

        // Instantiate arch object
        GameObject newArch = Instantiate(archPrototype, transform.position, Quaternion.Euler(90, ang + 90f, 0) );

        // Set script vars
        SimpleDamagefield script = newArch.GetComponent<SimpleDamagefield>();
        script._KnockbackDir        = dir;
        script._TargetTag           = targetTag;
        script._CreatedBy           = user;
        script._DamageFromWeapon    = AttackDamage * userCharScript.ProjectileDamageMult;
        script._KnockbackFromWeapon = Knockback;

        // Set physical properties and activate
        newArch.transform.localScale = new Vector3(10, 10, 10);
        newArch.transform.parent = this.transform;
        newArch.SetActive(true);

        // Set cooldown
        base.Attack(aimPosition, targetTag);
    }
}
