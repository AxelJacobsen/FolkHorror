using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A passive item applying burn on hit.
/// </summary>
public class Ashes : Passiveitem
{
    public EffectData BurnEffect;

    void Start()
    {
        base.Start();
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
        if (targetCharacterScript != null) targetCharacterScript.ApplyEffect(BurnEffect, _Player);
    }
}
