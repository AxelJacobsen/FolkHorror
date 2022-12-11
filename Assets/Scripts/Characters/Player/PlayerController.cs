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
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitData;
			Vector3 hitPoint = Vector3.zero;
			if (Physics.Raycast(ray, out hitData, 1000, AimLayer)) hitPoint = hitData.point;
			Attack(hitPoint, "Enemy");
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
		dropAllItems();

		// display death screen
		InfoScreen info = GameObject.Find("/InfoScreen").transform.GetComponent<InfoScreen>();
		if (info == null) Debug.Log(gameObject.name + " could not find InfoScreen");
		info.ToggleInfoScreen(true);

		GameObject sceneLoaderObject = GameObject.FindGameObjectWithTag("SceneLoader");
		SceneLoader sceneLoader = sceneLoaderObject.GetComponent<SceneLoader>();
		sceneLoader.ChangeScene(respawnLocation);
		StartCoroutine(resetStats());
		OnDie();
	}

	/// <summary>
	/// Drops and destroys all items a player has
	/// </summary>
	void dropAllItems() {
		int j = 0;
		for (int i = 0; i < Items.Count; i++) {
			Items[i - j++].Drop();
			//Destroy(item.GameObject);
		}
	}

	/// <summary>
	/// Resets player status, waits untill end of frame to avoid overkill
	/// </summary>
	IEnumerator resetStats() {
		yield return new WaitForEndOfFrame();
		UpdateStats();
		//baseStats = base.Copy();
		Health = MaxHealth;
		dead = false;
	}
}