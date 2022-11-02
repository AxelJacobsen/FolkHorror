using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Funcs : MonoBehaviour
{
    /// <summary>
    /// Gets the closest target to an object.
    /// </summary>
    /// <param name="targets">List of targets, each having a rigidbody and Character component.</param>
    /// <param name="to">The object to measure distance from.</param>
    /// <returns>The closest valid target to the object.</returns>
    static public GameObject GetClosestTargetTo(GameObject[] targets, GameObject to) 
	{
        Vector3 toPos = to.transform.position;
        
        // Iterate them, finding the closest
        float       minDistSqr = Mathf.Infinity;
        GameObject  retObj = null;
        foreach (GameObject obj in targets) {
            // Check that the object is not the object we're referencing to
            if (obj == to) continue;

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

    /// <summary>
    /// Checks if a point is within a polygon.
    /// Taken from: https://stackoverflow.com/a/7123291
    /// Credit goes to https://stackoverflow.com/users/42845/keith.
    /// </summary>
    /// <param name="poly">The polygon.</param>
    /// <param name="p">The point.</param>
    /// <returns>True if the point is within the polygon.</returns>
    public static bool IsInPolygon(Vector2[] poly, Vector2 p)
    {
        Vector2 p1, p2;
        bool inside = false;

        if (poly.Length < 3)
        {
            return inside;
        }

        var oldPoint = new Vector2(
            poly[poly.Length - 1].x, poly[poly.Length - 1].y);

        for (int i = 0; i < poly.Length; i++)
        {
            var newPoint = new Vector2(poly[i].x, poly[i].y);

            if (newPoint.x > oldPoint.x)
            {
                p1 = oldPoint;
                p2 = newPoint;
            }
            else
            {
                p1 = newPoint;
                p2 = oldPoint;
            }

            if ((newPoint.x < p.x) == (p.x <= oldPoint.x)
                && (p.y - (long) p1.y)*(p2.x - p1.x)
                < (p2.y - (long) p1.y)*(p.x - p1.x))
            {
                inside = !inside;
            }

            oldPoint = newPoint;
        }

        return inside;
    }

    public static Vector2 GetRandomPointInPolygon(Vector2[] poly) {
        // Get lower- and upper bound of polygon
        Vector2 lowerbounds = new Vector2(0,0),
                upperbounds = new Vector2(0,0);
        bool    lowerboundsSet = false,
                upperboundsSet = false;

        foreach (Vector2 point in poly) {
            lowerbounds.x = lowerboundsSet ? (point.x < lowerbounds.x ? point.x : lowerbounds.x) : point.x;
            lowerbounds.y = lowerboundsSet ? (point.y < lowerbounds.y ? point.y : lowerbounds.y) : point.y;
            upperbounds.x = upperboundsSet ? (point.x > upperbounds.x ? point.x : upperbounds.x) : point.x;
            upperbounds.y = upperboundsSet ? (point.y > upperbounds.y ? point.y : upperbounds.y) : point.y;
            if (!lowerboundsSet) lowerboundsSet = true;
            if (!upperboundsSet) upperboundsSet = true;
        }

        // Return if upper- and lower bounds are the same
        if (lowerbounds == upperbounds) return new Vector2(0,0);

        // Generate a random point within the upper- and lower bound until it's within the polygon
        Vector2 retp    = new Vector2(0, 0);
        bool    retpSet = false;
        int     timeOut = 100;
        while (!retpSet) {
            if (timeOut-- < 0) { break; }
            retp.x = Random.Range(lowerbounds.x, upperbounds.x);
            retp.y = Random.Range(lowerbounds.y, upperbounds.y);
            if (IsInPolygon(poly, retp)) retpSet = true;
        }

        // Return
        return retp;
    }
}
