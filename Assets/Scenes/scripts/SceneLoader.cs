using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class for loading/unloading scenes.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public  string StartSceneName;
    private string nameOfSceneToLoad;
    private string nameOfCurrentScene;

    void Start() {
        ChangeScene(StartSceneName);
    }

    /// <summary>
    /// Change the scene.
    /// </summary>
    /// <param name="sceneName">The name of the scene to change to.</param>
    void ChangeScene(string sceneName) {
        nameOfCurrentScene = sceneName;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    // Set callback for loading new map scene, start unloading
    public void LoadNewMapScene(string nameOfSceneToLoad)
    {
        SceneManager.sceneUnloaded += MapSceneUnloadFinished;
        this.nameOfSceneToLoad = nameOfSceneToLoad;
        SceneManager.UnloadSceneAsync(nameOfCurrentScene);
    }

    // Remove callback, call load for new map scene
    private void MapSceneUnloadFinished(Scene unloadedScene)
    {
        SceneManager.sceneUnloaded -= MapSceneUnloadFinished;
        if (!SceneManager.GetSceneByName(nameOfSceneToLoad).isLoaded)
        {
            ChangeScene(nameOfSceneToLoad);
        }
    }
}