using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // Public vars
    [Header("Vars")]
    public GameObject   _Player;
    public float        MagnetRange = 10f,
                        PickupRange = 3f;

    // Private vars
    protected Rigidbody _pRB; // Player rigidbody
    protected Transform transform;
    protected Rigidbody rb;
    protected Animator  anim;
    protected bool      equipped = false;
    private float       pickupCooldown = 0f;

    protected void Start()
    {
        // Fetch components
        _pRB = _Player.GetComponent<Rigidbody>();
        if (_pRB == null) Debug.LogError("Pickup could not find the player's rigidbody!");

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
        } else if (pickupCooldown <= 0f) {
            // Attract the pickup if the player is close enough
            Vector3 dir   = _pRB.position - rb.position;
            float   dir_m = dir.magnitude;
            if (dir_m < MagnetRange) rb.velocity += dir / (Mathf.Pow(dir_m/5f, 2) + 1f) * 50f * Time.deltaTime;

            // Pick up the pickup if the player is close enough
            if (dir_m < PickupRange) PickUp();
        }
    }

    /// <summary>
    /// Causes the item to be picked up by the player.
    /// </summary>
    protected virtual void PickUp() {
        // Mark the pickup as picked up
        equipped = true;
        _Player.GetComponent<Character>().Items.Add(this);

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
        _Player.GetComponent<Character>().Items.Remove(this);
        pickupCooldown = 1f;

        // Unfreeze, unparent and throw it
        rb.isKinematic = false;
        transform.SetParent(null);
        rb.velocity = _pRB.velocity.normalized * (_pRB.velocity.magnitude + 5f);
    }
}
