using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A passive item granting piercing projectiles.
/// </summary>
public class Spike : Passiveitem
{
    protected override void OnStart() 
    {
        base.OnStart();

        alterStats = charstats =>
        {
            charstats.Pierces += 1;
            return charstats;
        };
    }
}
