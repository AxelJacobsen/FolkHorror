using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Funcs : MonoBehaviour
{

    static public GameObject GetClosestTargetTo(GameObject[] targets, GameObject to) 
	{
        Vector3 toPos = to.transform.position;
        
        // Iterate them, finding the closest
        float       minDistSqr = Mathf.Infinity;
        GameObject  retObj = null;
        foreach (GameObject obj in targets) {
            // Check that the object is enabled, skip if it isn't
            Character charScript = obj.GetComponent<Character>();
            if (charScript == null || !charScript.enabled) continue;

            // Get the rigidbody of the current object, skip if it doesn't exist
            Rigidbody tRB = obj.GetComponent<Rigidbody>();
            if (tRB == null) continue;
  
            // Get the distance to that rigidbody, skip if it's greater or equal to the current min dist
            Vector3 dir = (tRB.position - toPos);
            float distSqr = Mathf.Pow(dir.x, 2) + Mathf.Pow(dir.y, 2);
            if (distSqr >= minDistSqr) continue;
            
            // Set return obj and min dist to current
            minDistSqr = distSqr;
            retObj = obj;
        }

        // Return
        return retObj;
    }
}
