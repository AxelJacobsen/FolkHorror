using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple passive item granting speed.
/// </summary>
public class Shoe : Passiveitem
{
    void Start()
    {
        base.Start();

        alterStats = charstats =>
        {
            charstats.Speed += 2.5f;
            return charstats;
        };
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
