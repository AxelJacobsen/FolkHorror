using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon
{
    // Public vars
    public int          ArrowsPerShot = 1;
    public LayerMask    AimLayer;

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

    public override void Attack(GameObject[] targets)
    {
        // If the weapon is on cooldown, do nothing
        if (AttackCooldown > 0f) return;

        // Cast ray to find where the player wants to hit
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        Vector3 hitPoint = Vector3.zero;
        if (Physics.Raycast(ray, out hitData, 1000, AimLayer)) hitPoint = hitData.point;

        // Shoot an arrow towards that point
        Vector3 dir = (hitPoint - transform.position).normalized;
        dir.y = 0;

        float   ang = Mathf.Atan2(dir.x, dir.z) * 180f / Mathf.PI;

        print(ang);

        GameObject newArrow = Instantiate(arrowPrototype, transform.position, Quaternion.Euler(90, ang + 90f, 0) );
        newArrow.transform.localScale = new Vector3(10, 10, 10);
        newArrow.SetActive(true);

        newArrow.GetComponent<Rigidbody>().velocity = dir * 25f;

        // Set cooldown
        base.Attack(targets);
    }
}
