using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple class for spawning objects.
/// </summary>
public class ObjectSpawner : MonoBehaviour
{
    public GameObject Tree;
    public GameObject Bush;
    public GameObject Chest;
    public GameObject Exit;
    public GameObject Grass;
    public GameObject Rocks;

    /// <summary>
    /// Spawns the prototype at a given position.
    /// </summary>
    /// <param name="position">The given position.</param>
    public void SpawnObject(Vector3 position, int renderIndex) {
        if (renderIndex == -1) { return; }
        List<GameObject> renderList = new List<GameObject>() { Tree, Bush, Chest, Exit, Grass, Rocks };
        GameObject renderObject = renderList[renderIndex];
        SpriteRenderer sprite = renderObject.GetComponent<SpriteRenderer>();
        position.y += ((sprite.bounds.size.y) / 2 )* Mathf.Sin(45)*(0.83f);
        position.z += ((sprite.bounds.size.y) / 2 )* Mathf.Sin(45)*(0.83f);
        position.x += (sprite.bounds.size.x / 4 );
        GameObject newTree = Instantiate(renderObject, position, Quaternion.Euler(45, 0, 0) );
        newTree.SetActive(true);
        
    }
}
