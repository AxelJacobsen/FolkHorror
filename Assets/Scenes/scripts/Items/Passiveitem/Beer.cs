using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beer : Passiveitem
{
    void Start()
    {
        base.Start();

        alterStats = charstats =>
        {
            charstats.Speed -= 5f;
            charstats.MaxHealth += 5;
            charstats.Health += 5;
            return charstats;
        };
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
