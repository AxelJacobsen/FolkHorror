using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Class for loading/unloading scenes.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public string StartSceneName;

    void Start() {
        ChangeScene(StartSceneName);
    }

    /// <summary>
    /// Change the scene.
    /// </summary>
    /// <param name="sceneName">The name of the scene to change to.</param>
    public void ChangeScene(string sceneName) {
        // Unload all other scenes than "NeverUnload"
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene it = SceneManager.GetSceneAt(i);
            if (it.name != "NeverUnload")
                SceneManager.UnloadSceneAsync(it);
        }
        // Load the scene
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }
}