using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Character
{
	// Private vars
	private int spaceHeld;

	void Start()
	{
		base.Start();

		// Initialize vars
		spaceHeld = 0;
	}

	void FixedUpdate()
	{
		base.FixedUpdate();

		// Toggle spacebar
		if (Input.GetKey("space")) {
			spaceHeld++;
		} else {
			spaceHeld = 0;
		}

		// Attack
		if (spaceHeld == 1 && attackCooldown <= 0f) {
			GameObject 	target = GetClosestTarget("Enemy");
			Attack(target);
		}

		// Move
		Vector3 dir = new Vector3 (Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
		rb.velocity += (dir*Speed - rb.velocity) * 10f  * Time.deltaTime;
		transform.localPosition += rb.velocity * Time.deltaTime;
	}
}