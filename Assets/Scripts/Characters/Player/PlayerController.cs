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
	private int attackHeld = 0;
	private PlayerControls playerControls;
    private bool tryRolling = false;

    private void Awake() {
		playerControls = new PlayerControls();
	}

	private void OnEnable() {
		playerControls.Enable();
	}

	private void OnDisable() {
		playerControls.Disable();
	}
	
	protected override void OnStart()
	{
		
	}

	protected override void OnFixedUpdate()
	{
		// Toggle spacebar
		if ( playerControls.General.Attack.ReadValue<float>() == 1f ) {
			attackHeld++;
		} else {
			attackHeld = 0;
		}

		// Attack
		if (attackHeld == 1 && Weapon != null && Weapon.CanAttack()) {
			// Cast ray to find where the player wants to hit
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitData;
			Vector3 hitPoint = Vector3.zero;
			if (Physics.Raycast(ray, out hitData, 1000, AimLayer)) hitPoint = hitData.point;
			Attack(hitPoint, "Enemy");
		}

		// Move
		Vector2 joystick = playerControls.General.Move.ReadValue<Vector2>();
		Vector3 dir = new Vector3 (joystick.x, 0, joystick.y).normalized;

		if (playerControls.General.Roll.ReadValue<float>() == 1f || tryRolling) 
		{
            tryRolling = SteerableRoll(dir);
        }
		else 
		{
        	Move(dir);
		}
    }
}