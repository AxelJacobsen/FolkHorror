using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Damage over time (flat or percent) effect
/// </summary>
[CreateAssetMenu(menuName = "Effects/DamageOverTime")]
public class DamageOverTime : EffectData
{
    [Header("Stats")]
    public float    DamagePerSecond = 0f,
                    PercentPerSecond = 0f;

    public override void OnBegin() 
    {
        base.OnBegin();
    }

    public override void During(float deltaTime) 
    {
        base.During(deltaTime);
        if (_Target == null) return;

        Character targetCharacterScript = _Target.GetComponent<Character>();
        if (targetCharacterScript != null) 
        {
            if (DamagePerSecond > 0f)       targetCharacterScript.Hurt(_Creator, DamagePerSecond * deltaTime * _Intensity);
            else if (DamagePerSecond < 0f)  targetCharacterScript.Heal(_Creator, -DamagePerSecond * deltaTime * _Intensity);
            if (PercentPerSecond > 0f )     targetCharacterScript.Hurt(_Creator, targetCharacterScript.MaxHealth / 100f * PercentPerSecond * deltaTime * _Intensity);
            else if (PercentPerSecond < 0f )targetCharacterScript.Heal(_Creator, -targetCharacterScript.MaxHealth / 100f * PercentPerSecond * deltaTime * _Intensity);
        }
    }

    public override void OnEnd() 
    {
        base.OnEnd();
    }
}
