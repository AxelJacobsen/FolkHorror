using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A passive item granting extra projectiles.
/// </summary>
public class Plusarrows : Passiveitem
{
    protected override void OnStart() 
    {
        base.OnStart();

        alterStats = charstats =>
        {
            charstats.ProjectileCountMult += 1;
            return charstats;
        };
    }
}
