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
        position.y += ((sprite.bounds.size.y) / 2 )* Mathf.Sin(45)*(0.83f);
        position.z += ((sprite.bounds.size.y) / 2 )* Mathf.Sin(45)*(0.83f);
        position.x += (sprite.bounds.size.x / 4 );
        GameObject spawnObject = Instantiate(inObject, position, Quaternion.Euler(45, 0, 0) );
        spawnObject.SetActive(true);
        return spawnObject;
    }
}
