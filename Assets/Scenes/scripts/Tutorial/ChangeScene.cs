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
        SceneManager.LoadScene("TownScene");
    }
}
