using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A passive item granting sharper projectiles.
/// </summary>
public class Sharperarrows : Passiveitem
{
    protected override void OnStart() 
    {
        base.OnStart();

        alterStats = charstats =>
        {
            charstats.ProjectileDamageMult += 1;
            return charstats;
        };
    }
}
