using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon
{
    // Public vars
    public int ArrowsPerShot = 1;

    // Private vars
    public GameObject  arrowPrototype;

    protected void Start() {
        base.Start();
        // Fetch components
        /*         arrow = GetComponent<Arrow>();
                if (arrow == null) Debug.LogError("Bow could not find its arrow prototype!"); */
    }

    protected void FixedUpdate() {
        base.FixedUpdate();

    }

    public override void Attack(GameObject[] targets) {
        // If the weapon is on cooldown, do nothing
        if (AttackCooldown > 0f) return;

        base.Attack(targets);

        GameObject newArrow = Instantiate(arrowPrototype, transform.position, transform.rotation);
        newArrow.transform.localScale = new Vector3(10, 10, 10);
        newArrow.SetActive(true);

        newArrow.GetComponent<Rigidbody>().velocity = new Vector3(10, 0, 0);
    }
}
