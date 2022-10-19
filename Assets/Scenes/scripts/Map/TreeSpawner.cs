using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple class for spawning objects.
/// </summary>
public class ObjectSpawner : MonoBehaviour
{
    public GameObject Prototype;

    /// <summary>
    /// Spawns the prototype at a given position.
    /// </summary>
    /// <param name="position">The given position.</param>
    void SpawnObject(Vector3 position) {
        GameObject newTree = Instantiate(Prototype, position, Quaternion.Euler(45, 0, 0) );
        newTree.SetActive(true);
    }
}
