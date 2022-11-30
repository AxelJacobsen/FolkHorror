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
    /// Checks if any given GameObjects are within a certain radius of a coordinate
    /// </summary>
    /// <param name="targets">List of targets, each having a rigidbody and Character component.</param>
    /// <param name="center">The object to measure distance from.</param>
    /// <param name="radius">distance required between center and nearest object.</param>
    /// <returns>The closest valid target to the object.</returns>
    static public (Vector2, bool) CheckForIntersect(GameObject[] targets, Vector3 center, int radius) {
        Vector3 dir = new Vector3 (0,0,0 );
        // Iterate them, finding the closest
        float minDistSqr = Mathf.Infinity;
        foreach (GameObject obj in targets) {
            // Get the distance to that rigidbody, skip if it's greater or equal to the current min dist
            Vector3 tDir = (obj.transform.position - center);
            float distSqr = tDir.magnitude;
            if (distSqr >= minDistSqr) continue;

            // Set return obj and min dist to current
            minDistSqr = distSqr;
            dir = tDir;
        }

        // Return
        if (minDistSqr < radius) {
            //Returns a vector to move center away from nearest object
            return (-dir, true);
        }
        return (new Vector2 (dir.x,dir.z), false);
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


    /// <summary>
    /// Gets a random point in a given polygon
    /// </summary>
    /// <param name="poly">The polygon where the point is to be placed</param>
    /// <returns>The point in the polygon as a Vector2</returns>
    public static Vector2 GetRandomPointInPolygon(Vector2[] poly) {
        // Get lower- and upper bound of polygon
        Vector2 lowerbounds = new Vector2(0,0),
                upperbounds = new Vector2(0,0);
        (lowerbounds, upperbounds) = GetPolyBounds(poly);

        // Return if upper- and lower bounds are the same
        if (lowerbounds == upperbounds) return new Vector2(0,0);
        
        // Generate a random point within the upper- and lower bound until it's within the polygon
        Vector2 retp    = new Vector2(0, 0);
        bool    retpSet = false;
        int     timeOut = 200;
        while (!retpSet) {
            if (timeOut-- < 0) { break; }
            retp.x = Random.Range(lowerbounds.x, upperbounds.x);
            retp.y = Random.Range(lowerbounds.y, upperbounds.y);
            if (IsInPolygon(poly, retp)) {
                retpSet = true;
            }
        }
        // Return
        return retp;
    }


    /// <summary>
    /// A function to force two things to spawn in the rooms far corners,
    /// currently only used for portals
    /// </summary>
    /// <param name="poly"></param>
    /// <returns>Returns coordinates for the two objects</returns>
    public static (Vector2, Vector2, Vector2, Vector2) ForceFarSpawn(Vector2[] poly) {
        //Fetch polygon bounds
        Vector2 lowerbounds = new Vector2(0, 0),
                upperbounds = new Vector2(0, 0);
        (lowerbounds, upperbounds) = GetPolyBounds(poly);
        //Bring in outer limit to reduce chance of hitting outerwall
        lowerbounds -= lowerbounds / 10;
        upperbounds -= upperbounds / 10;
        int corner = Random.Range(0, 3);
        Vector2 newLowBound, newUpBound;
        //Gets one of the cornerboxes based on what corner was picked
        (newLowBound, newUpBound) = GetCornerBoxes(lowerbounds, upperbounds, corner);
        Vector2 firstPoint = FindPointInCornerBox(newLowBound, newUpBound, poly);
        //Changes to the corner opposite to the initial corner
        switch (corner) {
            case 0: corner = 2; break;
            case 1: corner = 3; break;
            case 2: corner = 0; break;
            case 3: corner = 1; break;
            default: break;
        }
        //Gets the second cornerbox
        (newLowBound, newUpBound) = GetCornerBoxes(lowerbounds, upperbounds, corner);
        Vector2 secondPoint = FindPointInCornerBox(newLowBound, newUpBound, poly);
        //Return
        return (firstPoint, secondPoint, newLowBound, newUpBound);
    }


    /// <summary>
    /// Finds the coordinates of a box in the corners of the map based on the corner int given
    /// </summary>
    /// <param name="lowerbounds"></param>
    /// <param name="upperbounds"></param>
    /// <param name="corner"></param>
    /// <returns>Returns upper and lowerbounds of a cornerbox</returns>
    private static (Vector2, Vector2) GetCornerBoxes(Vector2 lowerbounds, Vector2 upperbounds, int corner) {
        Vector2 outLowerBounds = lowerbounds,
                outUpperBounds = upperbounds;
        switch (corner) {
            case 0:
                outLowerBounds.y = outUpperBounds.y/2;
                outUpperBounds.x = outLowerBounds.x/2;
                break;

            case 1:
                outLowerBounds.x = outUpperBounds.x / 2;
                outLowerBounds.y = outUpperBounds.y / 2;
                break;
            case 2:
                outLowerBounds.x = outUpperBounds.x / 2;
                outUpperBounds.y = outLowerBounds.y / 2;
                break;

            case 3:
                outUpperBounds.x = outLowerBounds.x/2;
                outUpperBounds.y = outLowerBounds.y/2;
                break;
            default: break;
        }
        return (outLowerBounds, outUpperBounds );
    }

    /// <summary>
    /// Finds a coordinate thats within a given polygon and a box
    /// </summary>
    /// <param name="boxLowerBounds"></param>
    /// <param name="boxUpperBounds"></param>
    /// <param name="poly"></param>
    /// <returns></returns>
    private static Vector2 FindPointInCornerBox(Vector2 boxLowerBounds, Vector2 boxUpperBounds, Vector2[] poly) {
        Vector2 outPoint = new Vector2(0, 0);
        int timeOut = 20;
        do {
            outPoint.x = Random.Range(boxLowerBounds.x, boxUpperBounds.x);
            outPoint.y = Random.Range(boxLowerBounds.y, boxUpperBounds.y);
            timeOut--;
        } while (!IsInPolygon(poly, outPoint) && 0<timeOut);
        return outPoint;
    }

    public static Vector2 FindPointOutsidePlayerSpawn(Vector2 boxLowerBounds, Vector2 boxUpperBounds, Vector2[] poly) {
        Vector2 outPoint = new Vector2(0, 0);
        //Fetch polygon bounds
        Vector2 lowerbounds = new Vector2(0, 0),
                upperbounds = new Vector2(0, 0);
        (lowerbounds, upperbounds) = GetPolyBounds(poly);
        int timeOut = 20;
        do {
            outPoint.x = Random.Range(lowerbounds.x, upperbounds.x);
            outPoint.y = Random.Range(lowerbounds.y, upperbounds.y);
            timeOut--;
            if ((boxLowerBounds.x < outPoint.x && outPoint.x < boxUpperBounds.x) &&
                (boxLowerBounds.y < outPoint.y && outPoint.y < boxUpperBounds.y)) {
                continue;
            }
        } while (!IsInPolygon(poly, outPoint) && 0 < timeOut);
        return outPoint;
    }

    /// <summary>
    /// Gets the upper and lower limits of a given polygon
    /// </summary>
    /// <param name="poly"></param>
    /// <returns>returns upper and lower coordinates of a given polygon</returns>
    private static (Vector2, Vector2) GetPolyBounds(Vector2[] poly) {
        Vector2 lowerbounds = new Vector2(0, 0),
                upperbounds = new Vector2(0, 0);
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
        return (lowerbounds, upperbounds);
    }
}
