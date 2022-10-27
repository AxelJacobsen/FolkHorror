using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        print("B!");
    }

    void OnTriggerEnter (Collider hit) {
        print("A!");
        GameObject sceneLoaderObject = GameObject.FindGameObjectWithTag("SceneLoader");
        SceneLoader sceneLoader = sceneLoaderObject.GetComponent<SceneLoader>();
        sceneLoader.ChangeScene("BaseScene");
    }
}
