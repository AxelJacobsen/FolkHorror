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

    void Start()
    {
        base.Start();

        myBleedEffect = Instantiate(BleedEffect);
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
        if (targetCharacterScript != null) targetCharacterScript.ApplyEffect(myBleedEffect, user);
    }
}
