using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>PauseController</c> controls pausing of the game.
/// </summary>
public class PauseController : MonoBehaviour
{
    public static bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;   
    }

    /// <summary>
    /// Pause game.
    /// </summary>
    public static void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Resume game.
    /// </summary>
    public static void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
    }
}
