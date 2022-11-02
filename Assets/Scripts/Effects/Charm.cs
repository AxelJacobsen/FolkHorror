using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// "Charm" effect
/// </summary>
[CreateAssetMenu(menuName = "Effects/Charm")]
public class Charm : EffectData
{
    private void applyCharm() 
    {
        // Fetch AI script and return if it isn't found
        simpleEnemyAI targetAIscript = _Target.GetComponent<simpleEnemyAI>();
        if (targetAIscript == null) return;

        // Charm
        targetAIscript.Charm( _Creator.tag == "Player" ? "Enemy" : "Player", _Duration);
    }

    public override void OnBegin() 
    {
        base.OnBegin();

        applyCharm();
    }

    public override void OnReapply()
    {
        base.OnReapply();

        applyCharm();
    }

    public override void During(float deltaTime) 
    {
        base.During(deltaTime);
    }

    public override void OnEnd() 
    {
        base.OnEnd();
    }
}
