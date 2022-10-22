using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A heart which heals the player.
/// </summary>
public class Heart : Item
{    
    void Start()
    {
        base.Start();
    }

    void FixedUpdate() {
        base.FixedUpdate();
    }

    protected override void PickUp(){
        // Heal the player for a percent of their HP
        int hp = _playerCharacter.Heal(_Player, _playerCharacter.MaxHealth / 4);
        print(hp);
        Destroy(this.gameObject);
    }
}
