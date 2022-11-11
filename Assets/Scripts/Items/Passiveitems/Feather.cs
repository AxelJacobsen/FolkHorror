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
            charstats.Speed *= 2f;
            charstats.MaxHealth /= 1.5f;
            return charstats;
        };
    }
}
