using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A passive item applying charm on hit.
/// </summary>
public class Harp : Passiveitem
{
    public EffectData CharmEffect;
    public EffectData myCharmEffect;

    protected override void OnStart() 
    {
        base.OnStart();

        myCharmEffect = Instantiate(CharmEffect);
    }

    /// <summary>
    /// Burn enemies on-hit.
    /// </summary>
    public override void OnPlayerHit(GameObject target, float amount){
        Character targetCharacterScript = target.GetComponent<Character>();
        if (targetCharacterScript != null) targetCharacterScript.ApplyEffect(myCharmEffect, user);
    }
}
