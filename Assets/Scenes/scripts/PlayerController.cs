using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Character
{
	// Private vars
	private int spaceHeld;
	private PlayerControls playerControls;

	private void Awake() {
		playerControls = new PlayerControls();
	}

	private void OnEnable() {
		playerControls.Enable();
	}

	private void OnDisable() {
		playerControls.Disable();
	}
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
		if ( playerControls.General.Attack.ReadValue<float>() == 1f ) {
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
		Vector2 joystick = playerControls.General.Move.ReadValue<Vector2>();
		Vector3 dir = new Vector3 (joystick.x, 0, joystick.y).normalized;
		rb.velocity += (dir*Speed - rb.velocity) * 10f  * Time.deltaTime;
	}
}