using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon
{
    // Public vars
    public int          ArrowsPerShot = 1;
    public float        DegSpread     = 15f;
    public GameObject   arrowPrototype;

    public override void Attack(Vector3 aimPosition, string targetTag)
    {
        // If the weapon is on cooldown, do nothing
        if (AttackCooldown > 0f) return;

        // Shoot an arrow towards that point
        Vector3 dir = (aimPosition - transform.position).normalized;
        dir.y = 0;
        float   ang = Mathf.Atan2(dir.x, dir.z);

        // Instantiate arrow(s)
        for (int i = 0; i < ArrowsPerShot * userCharScript.ProjectileCountMult; i++) 
        {
            float   myAng = ang + Random.Range(-DegSpread/2f, DegSpread/2f) * Mathf.Deg2Rad;
            Vector3 myDir = new Vector3(Mathf.Sin(myAng), 0, Mathf.Cos(myAng));
            GameObject newArrow = Instantiate(arrowPrototype, transform.position, Quaternion.Euler(90, (myAng) * Mathf.Rad2Deg + 90, 0));

            // Set script vars
            SimpleProjectile script = newArrow.GetComponent<SimpleProjectile>();
            script._TargetTag           = targetTag;
            script._CreatedBy           = user;
            script._DamageFromWeapon    = AttackDamage * userCharScript.ProjectileDamageMult;
            script._KnockbackFromWeapon = Knockback;

            script.Bounces  = userCharScript.Bounces;
            script.Pierces  = userCharScript.Pierces;
            script.Chains   = userCharScript.Chains;

            // Set physical properties and activate
            newArrow.transform.localScale = new Vector3(10, 10, 10);
            newArrow.SetActive(true);
            newArrow.GetComponent<Rigidbody>().velocity = myDir * 50f; /// FIX THIS
        }


        // Set cooldown
        base.Attack(aimPosition, targetTag);
    }
}
