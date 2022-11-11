using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A passive item applying bleed on hit.
/// </summary>
public class Knife : Passiveitem
{
    public EffectData BleedEffect;
    public EffectData myBleedEffect;

    protected override void OnStart() 
    {
        base.OnStart();

        myBleedEffect = Instantiate(BleedEffect);
    }

    /// <summary>
    /// Burn enemies on-hit.
    /// </summary>
    public override void OnPlayerHit(GameObject target, float amount){
        Character targetCharacterScript = target.GetComponent<Character>();
        if (targetCharacterScript != null) targetCharacterScript.ApplyEffect(myBleedEffect, user);
    }
}
