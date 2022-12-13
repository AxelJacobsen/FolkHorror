using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The playercontroller.
/// </summary>
public class PlayerController : Character {
	// Public vars
	public LayerMask AimLayer;
	public int currentStage = 0;
	public string currentBiome = "";
	public string respawnLocation = "TownScene";

	// Private vars
	private int attackHeld = 0;
	private PlayerControls playerControls;

  private bool tryRolling = false;
	private int coinAmount = 0; 

	private void Awake() {
		playerControls = new PlayerControls();
	}

	private void OnEnable() {
		playerControls.Enable();
	}

	private void OnDisable() {
		playerControls.Disable();
	}

	protected override void OnStart() {

	}

	public void tryIncrementCoinAmount() {
		if (coinAmount < int.MaxValue) coinAmount++; 
	}
	public bool tryRemoveCoinAmount(int removeAmount) {
		if (coinAmount >= removeAmount && removeAmount > 0)
		{
			coinAmount--;
            tryRemoveCoinAmount(removeAmount - 1);
		}
		else if (removeAmount <= 0) return true;
		return false;
	}
	public int getCoinAmount() { return coinAmount; }


	protected override void OnFixedUpdate() {
		// Toggle spacebar
		if (playerControls.General.Attack.ReadValue<float>() == 1f) {
			attackHeld++;
		}
		else {
			attackHeld = 0;
		}

		// Attack
		if (attackHeld == 1 && Weapon != null && Weapon.CanAttack()) {
			// Cast ray to find where the player wants to hit
			float distance;
			Vector3 worldPosition = Vector3.zero;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (new Plane(Vector3.up, 0).Raycast(ray, out distance)) {
				worldPosition = ray.GetPoint(distance);
			}
			Attack(worldPosition, "Enemy");
		}

		// Move
		Vector2 joystick = playerControls.General.Move.ReadValue<Vector2>();
		Vector3 dir = new Vector3(joystick.x, 0, joystick.y).normalized;

		if (playerControls.General.Roll.ReadValue<float>() == 1f || tryRolling) {
			tryRolling = SteerableRoll(dir);
		}
		else {
			Move(dir);
		}
	}

	/// <summary>
	/// Overrides death funciton and sends player to TownScene
	/// </summary>
	public override void Die() {
		if (dead) return;
		else dead = true;
		StartCoroutine(dropAllItems());
		StartCoroutine(respawnDelay());

		// display death screen
		InfoScreen info = GameObject.Find("/InfoScreen").transform.GetComponent<InfoScreen>();
		if (info == null) Debug.Log(gameObject.name + " could not find InfoScreen");
		info.ToggleInfoScreen(true);
		StartCoroutine(resetStats());

		OnDie();
	}

	/// <summary>
	/// Drops and destroys all items a player has
	/// </summary>
	IEnumerator dropAllItems() {
		yield return new WaitForEndOfFrame();
		int j = 0;
		if (Items.Count <= 0) { yield break; }
		for (int i = 0; i < Items.Count; i++) {
			if (!(Items[i - j] is Weapon)) {
				Destroy(Items[i - j].gameObject);
				Items[i - j++].Drop();
			}
		}
	}

	/// <summary>
	/// Resets player status, waits untill end of frame to avoid overkill
	/// </summary>
	IEnumerator resetStats() {
		yield return new WaitForEndOfFrame();
		currentBiome = respawnLocation;
		currentStage = 0;
		UpdateStats();
		Health = MaxHealth;
		dead = false;
	}

	/// <summary>
	/// Resets player status, waits untill end of frame to avoid overkill
	/// </summary>
	IEnumerator respawnDelay() {
		yield return new WaitForEndOfFrame();
		GameObject sceneLoaderObject = GameObject.FindGameObjectWithTag("SceneLoader");
		SceneLoader sceneLoader = sceneLoaderObject.GetComponent<SceneLoader>();
		sceneLoader.ChangeScene(respawnLocation);
	}
}