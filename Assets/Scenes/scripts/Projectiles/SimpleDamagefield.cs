using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDamagefield : MonoBehaviour
{
   // Public vars
    [Header("Stats")]
    public int      Damage;
    public float    Knockback,
                    Lifetime;

    [Header("Visual")]
    public bool     fades;

    [Header("Set by scripts")]
    public Vector3  _KnockbackDir;
    public string   _TargetTag;
    public GameObject   _CreatedBy;

    // Private vars
    private float livedFor;

    void Start()
    {

    }

    /// <summary>
    /// Destroys this damage field.
    /// </summary>
    void Destroy()
    {
        // Add some more effects here
        Destroy(this.gameObject);
    }

    void FixedUpdate()
    {
        // Count down lifetime
        Lifetime -= Time.deltaTime;
        if (Lifetime <= 0f) Destroy();
    }


    void OnTriggerEnter(Collider hit) 
    {
        // If the hit collider belongs to a hitbox, use its parent instead.
        GameObject hitObj = hit.gameObject;
        if (hitObj.tag == "Hitbox") { hitObj = hitObj.transform.parent.gameObject; }
        else { return; } // Otherwise, return.

        // Check if it hit a character. If not, return.
        Character characterHit = hitObj.GetComponent<Character>();
        if (characterHit == null) { return; }

        // Check if that character has the correct tag. If not, return.
        if (characterHit.tag != _TargetTag) { return; }

        // Apply effects on target
        characterHit.Knockback(_KnockbackDir * Knockback);
        characterHit.Hurt(_CreatedBy, Damage);

        // Invoke items
        Character createdByCharacterScript = _CreatedBy.GetComponent<Character>();
        foreach (Item item in createdByCharacterScript.Items) { item.OnPlayerHit(hitObj); }
    }
}
