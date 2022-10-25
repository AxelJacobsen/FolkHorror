using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple passive item granting health at the cost of speed.
/// </summary>
public class VampireTooth : Passiveitem
{
    void Start()
    {
        base.Start();

        alterStats = charstats =>
        {
            charstats.Speed *= 1.5f;
            charstats.MaxHealth /= 2;
            return charstats;
        };
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();
    }

    /// <summary>
    /// Heal when hitting an enemy.
    /// </summary>
    public override void OnPlayerHit(GameObject target, int amount){
        _playerCharacter.Heal(_Player, amount / 2);
        Character targetCharacterScript = target.GetComponent<Character>();
        if (targetCharacterScript != null) targetCharacterScript.ApplyEffect("Bleeding", 3, 1);
    }
}
