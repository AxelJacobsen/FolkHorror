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
        if (isEntrance) {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            playerObject.transform.position = transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter (Collider hit) {
        if (isEntrance) return;
        GameObject sceneLoaderObject = GameObject.FindGameObjectWithTag("SceneLoader");
        SceneLoader sceneLoader = sceneLoaderObject.GetComponent<SceneLoader>();
        sceneLoader.ChangeScene(SceneName);
    }
}
