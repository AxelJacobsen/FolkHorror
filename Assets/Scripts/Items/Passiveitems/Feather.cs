using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple passive item granting speed at the cost of health.
/// </summary>
public class Feather : Passiveitem
{
    void Start()
    {
        base.Start();

        alterStats = charstats =>
        {
            charstats.Speed *= 2f;
            charstats.MaxHealth /= 1.5f;
            return charstats;
        };
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
