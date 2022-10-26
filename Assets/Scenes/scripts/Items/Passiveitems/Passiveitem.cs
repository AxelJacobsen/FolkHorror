using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passiveitem : Item
{
    // Public vars
    public AlterStats alterStats;
    
    void Start()
    {
        base.Start();
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void OnPlayerAttack(Vector3 aimPosition, string targetTag){

    }

    public override void OnPlayerGetHit(GameObject hitBy, float amount) {

    }

    public override void OnPlayerHit(GameObject target, float amount){

    }

}
