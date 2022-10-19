using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    public GameObject treePrototype;

    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    /// <summary>
    /// Spawns a tree at the given position.
    /// </summary>
    /// <param name="position"></param>
    void SpawnTree(Vector3 position) {
        GameObject newTree = Instantiate(treePrototype, position, Quaternion.Euler(45, 0, 0) );
        newTree.SetActive(true);
    }
}
