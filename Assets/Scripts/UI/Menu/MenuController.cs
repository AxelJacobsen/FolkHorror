using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>MenuController</c> control the game menu.
/// </summary>
public class MenuController : MonoBehaviour
{
    public GameObject menu;
    private bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        menu.gameObject.SetActive(false);
        isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isOpen = !isOpen;
            menu.SetActive(!menu.activeSelf);

            // only if the menu is open
            if (isOpen)
            {
                PauseController.PauseGame();
            }
            else
            {
                PauseController.ResumeGame();
            }
        }
    }

    /// <summary>
    /// Quit the game.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
