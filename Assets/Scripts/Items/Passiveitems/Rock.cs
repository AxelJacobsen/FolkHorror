using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A passive item applying stun on hit.
/// </summary>
public class Rock : Passiveitem
{
    public EffectData StunEffect;
    private EffectData myStunEffect;

    void Start()
    {
        base.Start();

        // Create a copy of StunEffect dynamically
        myStunEffect = Instantiate(StunEffect);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();
    }

    /// <summary>
    /// Burn enemies on-hit.
    /// </summary>
    public override void OnPlayerHit(GameObject target, float amount){
        Character targetCharacterScript = target.GetComponent<Character>();
        if (targetCharacterScript != null) targetCharacterScript.ApplyEffect(myStunEffect, _Player);
    }
}
