using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple passive item granting health.
/// </summary>
public class Oatmeal : Passiveitem
{
    protected override void OnStart() 
    {
        base.OnStart();

        alterStats = charstats =>
        {
            charstats.MaxHealth += 25f;
            return charstats;
        };
    }
}
