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
<<<<<<< HEAD:Assets/Scenes/scripts/Effects/PortalScript.cs

    }

    void OnTriggerEnter (Collider hit) {
        if (isEntrance) return;
=======
    }

    void OnTriggerEnter (Collider hit) {
>>>>>>> 6e1d4903f81280dcafd1dfb13213ea8e0513bf48:Assets/Scripts/Effects/PortalScript.cs
        GameObject sceneLoaderObject = GameObject.FindGameObjectWithTag("SceneLoader");
        SceneLoader sceneLoader = sceneLoaderObject.GetComponent<SceneLoader>();
        sceneLoader.ChangeScene(SceneName);
    }
}
