using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple passive item granting speed at the cost of health.
/// </summary>
public class Feather : Passiveitem
{
    protected override void OnStart() 
    {
        base.OnStart();

        alterStats = charstats =>
        {
            charstats.Speed *= 1.35f;
            charstats.MaxHealth /= 1.35f;
            return charstats;
        };
    }
}
