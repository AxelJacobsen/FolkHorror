using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple class for spawning objects.
/// </summary>
public class ObjectSpawner : MonoBehaviour
{
    /// <summary>
    /// Spawns the prototype at a given position.
    /// </summary>
    /// <param name="position">The given position.</param>
    public GameObject SpawnObject(Vector3 position, GameObject inObject) {
        SpriteRenderer sprite = inObject.GetComponent<SpriteRenderer>();
        float xAng = 45f;
        if (sprite == null) 
        {
            sprite = inObject.transform.GetComponentInChildren<SpriteRenderer>();
            xAng = 0f;
        }

        position.y += ((sprite.bounds.size.y) / 2 )* Mathf.Sin(45)*(0.83f);
        position.z += ((sprite.bounds.size.y) / 2 )* Mathf.Sin(45)*(0.83f);
        position.x += (sprite.bounds.size.x / 4 );
        GameObject spawnObject = Instantiate(inObject, position, Quaternion.Euler(xAng, 0, 0) );
        spawnObject.SetActive(true);
        return spawnObject;
    }
}
