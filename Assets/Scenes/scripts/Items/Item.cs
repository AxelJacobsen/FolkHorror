using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for items.
/// </summary>
public class Item : MonoBehaviour
{
    // Public vars
    [Header("Vars")]
    public string       PickedUpByTag;
    public float        MagnetRange = 10f,
                        PickupRange = 3f;

    [Header("Set by scripts")]
    public GameObject  _Player;
    
    // Private vars
    protected Rigidbody _pRB; // Player rigidbody
    protected Transform transform;
    protected Rigidbody rb;
    protected Animator  anim;
    protected bool      equipped = false;
    private float       pickupCooldown = 0f;
    protected Character _playerCharacter;

    protected void Start()
    {
        // Fetch components
        _Player = GameObject.FindGameObjectWithTag(PickedUpByTag);
        if (_Player == null) Debug.LogError("Pickup could not find the player!");

        _pRB = _Player.GetComponent<Rigidbody>();
        if (_pRB == null) Debug.LogError("Pickup could not find the player's rigidbody!");

        _playerCharacter = _Player.GetComponent<Character>();
        if (_playerCharacter == null) Debug.LogError("Pickup could not find the player's character script!");

        transform = GetComponent<Transform>();
		if (transform == null) Debug.LogError("Pickup could not find its transform!");

        rb = GetComponent<Rigidbody>();
		if (rb == null) Debug.LogError("Pickup could not find its rigidbody!");

        anim = GetComponent<Animator>();
		if (anim == null) Debug.LogError("Pickup could not find its animator!");
    }

    protected void FixedUpdate()
    {
        // Count down pickupCooldown
        if (pickupCooldown > 0f) pickupCooldown -= Time.deltaTime;

        // If picked up...
        if (equipped) {
            // ...

        // If not picked up yet...
        } else if (CanBePickedUp()) {
            // Attract the pickup if the player is close enough
            Vector3 dir   = _pRB.position - rb.position;
            float   dir_m = dir.magnitude;
            if (dir_m < MagnetRange) rb.velocity += dir / (Mathf.Pow(dir_m/5f, 2) + 1f) * 50f * Time.deltaTime;

            // Pick up the pickup if the player is close enough
            if (dir_m < PickupRange) PickUp();
        }
    }

    /// <summary>
    /// Returns if the item can be picked up.
    /// </summary>
    /// <returns>True if the item is ready to be picked up right now.</returns>    
    protected virtual bool CanBePickedUp() {
        return pickupCooldown <= 0f; // Add exceptions later
    }

    /// <summary>
    /// Causes the item to be picked up by the player.
    /// </summary>
    protected virtual void PickUp() {
        // Mark the pickup as picked up
        equipped = true;
        _playerCharacter.Items.Add(this);
        _playerCharacter.UpdateStats();

        // Freeze and parent it
        rb.isKinematic = true;
        transform.SetParent(_Player.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale    = Vector3.one;
    }

    /// <summary>
    /// Causes the item to be dropped.
    /// </summary>
    public virtual void Drop() {
        // Mark the pickup as dropped
        equipped = false;
        Character _playerCharacter = _Player.GetComponent<Character>();
        _playerCharacter.Items.Remove(this);
        _playerCharacter.UpdateStats();

        pickupCooldown = 1f;

        // Unfreeze, unparent and throw it
        rb.isKinematic = false;
        transform.SetParent(null);
        rb.velocity = _pRB.velocity.normalized * (_pRB.velocity.magnitude + 5f);
    }

    // Events
    public virtual void OnPlayerAttack(Vector3 aimPosition, string targetTag){}
    public virtual void OnPlayerHit(GameObject target, float amount){}
    public virtual void OnPlayerGetHit(GameObject hitBy, float amount){}
}
