using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A buff which grants regeneration.
/// </summary>
public class Regen : Item
{
    [Header("Effect")]
    public EffectData RegenEffect;

    protected override void OnStart() 
    {

    }

    protected override void OnFixedUpdate() 
    {
        
    }

    /// <summary>
    /// Grant a regeneration buff and delete self on pickup.
    /// </summary>
    protected override void PickUp()
    {
        userCharScript.ApplyEffect(RegenEffect, user);
        Destroy(this.gameObject);
    }
}
