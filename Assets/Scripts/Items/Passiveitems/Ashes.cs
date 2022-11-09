using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A passive item applying burn on hit.
/// </summary>
public class Ashes : Passiveitem
{
    public EffectData BurnEffect;
    public EffectData myBurnEffect;

    void Start()
    {
        base.Start();

        myBurnEffect = Instantiate(BurnEffect);
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
        if (targetCharacterScript != null) targetCharacterScript.ApplyEffect(myBurnEffect, user);
    }
}
