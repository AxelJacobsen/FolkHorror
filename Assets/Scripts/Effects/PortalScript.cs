using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    public string SceneName;
    public bool isEntrance = false;
    public bool resetSpeed = false;

    // Start is called before the first frame update
    void Start()
    {
        //If this is an entrance marker then find player and teleport them to the entrance
        if (isEntrance) {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            Vector3 newPos = new Vector3(transform.position.x, playerObject.transform.position.y, transform.position.z);
            playerObject.transform.position = newPos;
        }

        if (resetSpeed)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            playerObject.GetComponent<PlayerController>().Speed = 10f;
        }
    }

    void OnTriggerEnter (Collider hit) {
        //If it its an exit then teleport the player to the next stage on touch
        if (isEntrance || hit.transform.parent == null || !hit.transform.parent.gameObject.CompareTag("Player")) return;
        ChangeScene();
    }

    /// <summary>
    /// Change the current scene.
    /// </summary>
    public void ChangeScene()
    {
        GameObject sceneLoaderObject = GameObject.FindGameObjectWithTag("SceneLoader");
        SceneLoader sceneLoader = sceneLoaderObject.GetComponent<SceneLoader>();
        sceneLoader.ChangeScene(SceneName);
    }
}
