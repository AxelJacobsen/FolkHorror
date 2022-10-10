using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon
{
    // Public vars
    public int          ArrowsPerShot = 1;

    // Private vars
    public GameObject arrowPrototype;

    protected void Start()
    {
        base.Start();
        // Fetch components
        /*         arrow = GetComponent<Arrow>();
                if (arrow == null) Debug.LogError("Bow could not find its arrow prototype!"); */
    }

    protected void FixedUpdate()
    {
        base.FixedUpdate();

    }

    public override void Attack(Vector3 aimPosition, string targetTag)
    {
        // If the weapon is on cooldown, do nothing
        if (AttackCooldown > 0f) return;

        // Shoot an arrow towards that point
        Vector3 dir = (aimPosition - transform.position).normalized;
        dir.y = 0;
        float   ang = Mathf.Atan2(dir.x, dir.z) * 180f / Mathf.PI;

        // Instantiate arrow
        GameObject newArrow = Instantiate(arrowPrototype, transform.position, Quaternion.Euler(90, ang + 90f, 0) );

        // Set script vars
        SimpleProjectile script = newArrow.GetComponent<SimpleProjectile>();
        script._TargetTag = targetTag;

        // Set physical properties and activate
        newArrow.transform.localScale = new Vector3(10, 10, 10);
        newArrow.SetActive(true);
        newArrow.GetComponent<Rigidbody>().velocity = dir * 25f;

        // Set cooldown
        base.Attack(aimPosition, targetTag);
    }
}
