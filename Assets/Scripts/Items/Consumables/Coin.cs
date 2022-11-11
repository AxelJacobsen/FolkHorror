using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A coin that can be picked up.
/// </summary>
public class Coin : Item
{
    // Public vars
    [Header("Sounds")]
    [SerializeField] protected AudioClip CoinHitFloorSound;

    // Private vars
    private float   floorY = 0;
    private bool    floorYSet = false;
    private float   tryThrowingAgainIn = 0f;
    private Vector3 tryThrowingAgainWith = new Vector3 (0,0,0);

    protected override void OnStart() 
    {

    }
    
    /// <summary>
    /// Throws the coin with a given velocity.
    /// Also ensures the coin "lands" on the ground it originally was throw from.
    /// </summary>
    /// <param name="velocity">The velocity to throw the coin with.</param>
    public void Throw(Vector3 velocity)
    {
        // Set the floor height if it isn't set already
        if (!floorYSet)
        {
            floorY = transform.position.y;
            floorYSet = true;
        }

        // Add velocity upwards
        if (rb == null)
        {
            tryThrowingAgainIn = 0.1f;
            tryThrowingAgainWith = velocity;
            return;
        }
        rb.velocity += velocity;
    }

    /// <summary>
    /// Grant a regeneration buff and delete self on pickup.
    /// </summary>
    protected override void PickUp()
    {
        OnPickup();
        Destroy(this.gameObject);
    }

    /// <summary>
    /// What happens when the coin hits the floor.
    /// </summary>
    protected virtual void OnHitFloor()
    {
        SoundManager.Instance.PlaySound(CoinHitFloorSound);
    }

    /// <summary>
    /// Check if the coin has passed through the "floor" yet.
    /// </summary>
    protected override void OnFixedUpdate()
    {
        // Check if we're trying to throw the coin again
        if (tryThrowingAgainIn > 0f)
        {
            tryThrowingAgainIn -= Time.deltaTime;
            if (tryThrowingAgainIn <= 0f) Throw(tryThrowingAgainWith);
        }

        if (floorYSet)
        {
            // Check if the coin is hitting the floor
            if (transform.position.y < floorY)
            {
                Vector3 v = rb.velocity;
                v.y = 0;
                rb.velocity = v;

                Vector3 p = transform.position;
                p.y = floorY;
                transform.position = p;

                floorYSet = false;
                OnHitFloor();
            }
            // Add artificial gravity if we're falling
            else
            {
                rb.velocity += new Vector3(0, 2f * -9.81f, 0) * Time.deltaTime;
            }
        }
    }
}
