using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    void Start() 
    {

    }

    void FixedUpdate() {
        
    }

    void OnTriggerEnter(Collider hit) 
    {
        // If the hit collider belongs to a hitbox, use its parent instead.
        GameObject hitObj = hit.gameObject;
        if (hitObj.tag == "Hitbox") { hitObj = hitObj.transform.parent.gameObject; }

        // Check if it hit a character. If not, return.
        Character characterHit = hitObj.GetComponent<Character>();
        if (characterHit == null) { return; }

        //
        // Do damage, and stuff...
        //
    }
}
