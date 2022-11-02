using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class <c>ChangeScene</c> loads a new scene.
/// </summary>
public class ChangeScene : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        NewScene();
    }

    public void NewScene()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().Speed = 10f;
        GameObject sceneLoaderObject = GameObject.FindGameObjectWithTag("SceneLoader");
        SceneLoader sceneLoader = sceneLoaderObject.GetComponent<SceneLoader>();
        sceneLoader.ChangeScene("TownScene");
        //SceneManager.LoadScene("TownScene");
    }
}
