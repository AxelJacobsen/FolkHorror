using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    void Start()
    {
        base.Start();
    }

    void Update()
    {
        base.Update();
    }

    protected override void PickUp() {
        base.PickUp();

        _Player.GetComponent<Character>().Weapon = this;
    }

    public override void Drop() {
        base.Drop();

        _Player.GetComponent<Character>().Weapon = null;
    }
}
