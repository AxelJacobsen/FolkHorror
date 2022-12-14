using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A passive item granting bouncing projectiles.
/// </summary>
public class Bouncyball : Passiveitem
{
    protected override void OnStart() 
    {
        base.OnStart();

        alterStats = charstats =>
        {
            charstats.Bounces += 1;
            return charstats;
        };
    }
}
