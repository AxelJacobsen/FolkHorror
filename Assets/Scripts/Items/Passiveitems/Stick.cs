using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A passive item granting bouncing projectiles.
/// </summary>
public class Stick : Passiveitem
{
    protected override void OnStart() 
    {
        base.OnStart();

        alterStats = charstats =>
        {
            charstats.Chains += 1;
            return charstats;
        };
    }
}
