using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for items.
/// </summary>
public abstract class Item : MonoBehaviour
{
    // Public vars
    [Header("Settings")]
    public string       PickedUpByTag;
    public float        MagnetRange = 10f,
                        PickupRange = 3f;

    [Header("Sounds")]
    [SerializeField] protected AudioClip PickupSound;
    
    // Private vars
    protected GameObject     user;
    protected Character      userCharScript;
    protected Rigidbody      userRigidbody;
    protected Rigidbody      rb;
    protected Animator       anim;
    protected SpriteRenderer sr;
    protected bool           equipped = false;
    private float            pickupCooldown = 0f;

    protected abstract void OnStart();
    protected void Start()
    {
        user = lookForUser();
        if (user == null) Debug.LogError("Item could not find a user!");
        reconfigure();

        OnStart();
    }

    /// <summary>
    /// Reconfigures the item's variables.
    /// Assumes a valid user has been set.
    /// </summary>
    protected void reconfigure()
    {
        userCharScript = user.GetComponent<Character>();
        if (userCharScript == null) Debug.LogError("Item could not find its user's character script!");

        userRigidbody = user.GetComponent<Rigidbody>();
        if (userRigidbody == null) Debug.LogError("Item could not find its user's rigidbody!");


        rb = GetComponent<Rigidbody>();
		if (rb == null) Debug.LogError("Item could not find its rigidbody!");

        anim = GetComponent<Animator>();
		if (anim == null) Debug.LogWarning("Item could not find its animator!");

        sr = GetComponent<SpriteRenderer>();
        if (sr == null) Debug.LogError("Item could not find its spriterenderer!");
    }

    /// <summary>
    /// Returns the closest, valid user.
    /// </summary>
    /// <returns>The closest, valid user. If there is none, null.</returns>
    protected GameObject lookForUser()
    {
        // Fetch closest, valid user
        GameObject[] gObjs = GameObject.FindGameObjectsWithTag(PickedUpByTag);
        float       minDistSqr = Mathf.Infinity;
        GameObject  closestObj = null;
        foreach (GameObject obj in gObjs)
        {
            // Fetch character script, skip if its null.
            Character objCharScript = obj.GetComponent<Character>();
            if (objCharScript == null) continue;

            // If this obj can't pick up this item, skip.
            if (this.GetType() == typeof(Weapon) && objCharScript.Weapon != null)
                continue;

            // Check if this obj is closer than the prev
            float distSqr = Vector3.Distance(obj.transform.position, transform.position);
            if (distSqr < minDistSqr)
            {
                closestObj = obj;
                minDistSqr = distSqr;
            }
        }

        // Return
        return closestObj;
    }

    protected abstract void OnFixedUpdate();
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
            Vector3 dir   = user.transform.position - transform.position;
            float   dir_m = dir.magnitude;
            if (dir_m < MagnetRange) rb.velocity += dir / (Mathf.Pow(dir_m/5f, 2) + 1f) * 50f * Time.deltaTime;

            // Pick up the pickup if the player is close enough
            if (dir_m < PickupRange) PickUp();
        }
        OnFixedUpdate();
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
        userCharScript.Items.Add(this);
        userCharScript.UpdateStats();

        // Freeze, parent and make it invisible
        rb.isKinematic = true;
        Color src_t = sr.color; src_t.a = 0; sr.color = src_t;
        transform.SetParent(user.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale    = Vector3.one;

        OnPickup();
    }

    /// <summary>
    /// Causes the item to be dropped.
    /// </summary>
    public virtual void Drop() {
        // Mark the pickup as dropped
        equipped = false;
        userCharScript.Items.Remove(this);
        userCharScript.UpdateStats();

        pickupCooldown = 1f;

        // Unfreeze, unparent, throw it and make it visible
        rb.isKinematic = false;
        Color src_t = sr.color; src_t.a = 255; sr.color = src_t;
        transform.SetParent(null);
        
        rb.velocity = userRigidbody.velocity.normalized * (userRigidbody.velocity.magnitude + 5f);
    }

    // Events
    public virtual void OnPlayerAttack(Vector3 aimPosition, string targetTag){}
    public virtual void OnPlayerHit(GameObject target, float amount){}
    public virtual void OnPlayerGetHit(GameObject hitBy, float amount){}
    public virtual void OnPickup(){SoundManager.Instance.PlaySound(PickupSound, user.transform);}
}
