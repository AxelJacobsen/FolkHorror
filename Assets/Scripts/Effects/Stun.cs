using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// "Burning" effect
/// </summary>
[CreateAssetMenu(menuName = "Effects/Stun")]
public class Stun : EffectData
{
    public override void OnBegin() 
    {
        base.OnBegin();
    }

    public override void During(float deltaTime) 
    {
        base.During(deltaTime);

        Character targetCharacterScript = _Target.GetComponent<Character>();
        if (targetCharacterScript != null)
        {
            targetCharacterScript.Stun(deltaTime);
        }
    }

    public override void OnEnd() 
    {
        base.OnEnd();
    }
}
