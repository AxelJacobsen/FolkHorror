using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The playercontroller.
/// </summary>
public class PlayerController : Character
{
	// Public vars
	public LayerMask AimLayer;

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
		if (spaceHeld == 1 && Weapon != null && Weapon.CanAttack()) {
			// Cast ray to find where the player wants to hit
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitData;
			Vector3 hitPoint = Vector3.zero;
			if (Physics.Raycast(ray, out hitData, 1000, AimLayer)) hitPoint = hitData.point;
			Attack(hitPoint, "Enemy");
		}

		// Drop weapon (TEMP)
		if (Input.GetKeyDown("space")) {
            if (Weapon != null) Weapon.Drop();
			foreach (Item item in Items)
            {
				item.Drop();
            }
        }

		// Move
		Vector2 joystick = playerControls.General.Move.ReadValue<Vector2>();
		Vector3 dir = new Vector3 (joystick.x, 0, joystick.y).normalized;
        Move((dir * Speed - rb.velocity) * 10f * Time.deltaTime);
    }
}