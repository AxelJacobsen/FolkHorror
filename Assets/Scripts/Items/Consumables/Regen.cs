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

    void Start()
    {
        base.Start();
    }

    void FixedUpdate() 
    {
        base.FixedUpdate();
    }

    /// <summary>
    /// Grant a regeneration buff and delete self on pickup.
    /// </summary>
    protected override void PickUp()
    {
        _playerCharacter.ApplyEffect(RegenEffect, _Player);
        Destroy(this.gameObject);
    }
}
