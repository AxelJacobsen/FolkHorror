using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple passive item granting health at the cost of speed.
/// </summary>
public class Beer : Passiveitem
{
    protected override void OnStart() 
    {
        base.OnStart();

        alterStats = charstats =>
        {
            charstats.Speed /= 1.5f;
            charstats.MaxHealth *= 2f;
            return charstats;
        };
    }
}
