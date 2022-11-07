using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    public string SceneName;
    public bool isEntrance = false;
    // Start is called before the first frame update
    void Start()
    {
        //If this is an entrance marker then find player and teleport them to the entrance
        if (isEntrance) {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            playerObject.transform.position = transform.position;
        }
    }

    void OnTriggerEnter (Collider hit) {
        //If it its an exit then teleport the player to the next stage on touch
        if (isEntrance) return;
        GameObject sceneLoaderObject = GameObject.FindGameObjectWithTag("SceneLoader");
        SceneLoader sceneLoader = sceneLoaderObject.GetComponent<SceneLoader>();
        sceneLoader.ChangeScene(SceneName);
    }
}
