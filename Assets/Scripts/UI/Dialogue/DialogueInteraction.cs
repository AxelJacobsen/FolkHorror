using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class <c>DialogueController</c> controls the user interaction of a dialogue interaction.
/// </summary>
public class DialogueInteraction : MonoBehaviour
{
    public static bool isActive;

    public Canvas canvas;
    public Image textBox;
    public TextMeshProUGUI infoText;
    public Dialogue dialogue;
    public DialogueController controller;
    public bool autoStart = false;

    private int currentIndex;
    private bool menuIsOpen;

    // Start is called before the first frame update
    void Start()
    {
        currentIndex = 0;
        isActive = autoStart;
        menuIsOpen = false;
    }

    private void OnEnable()
    {
        isActive = autoStart;
        menuIsOpen = false;
        if (!autoStart) ToggleInfoText(true);
    }

    private void OnDisable()
    {
        PauseController.ResumeGame();
        currentIndex = 0;
        isActive = false;
        menuIsOpen = false;
        ToggleTextBox(false);
        ToggleInfoText(false);
    }

    // Update is called once per frame
    void Update()
    {
        print(dialogue.name);
        // if the game is paused AND a dialogue is not currently happening
        if (PauseController.isPaused && !isActive) return;

        // what happens when the player opens the menu mid dialogue
        if (MenuController.isOpen && !autoStart)
        {
            // finish the dialogue, and reset
            ResetDialogue();
            isActive = false;
            return;
        }
        else if (MenuController.isOpen && autoStart)
        {
            // pause the dialogue and hide the text box
            menuIsOpen = true;
            canvas.gameObject.SetActive(false);
            return;
        }

        // if menu is opened in auto start dialogue, this continues the dialogue when the menu is closed again
        if (menuIsOpen)
        {
            canvas.gameObject.SetActive(true);
            menuIsOpen = false;
        }
      
        // dialogue should start on its own, not by the player pressing E
        if (autoStart && currentIndex == 0) 
        {
            isActive = true;
            PauseController.PauseGame();
            ToggleTextBox(true);
            ToggleInfoText(false);
            controller.StartDialogue(dialogue.name, dialogue.sentences[0]);
            currentIndex++;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            // reset the dialogue if the last sentence is completed
            if (currentIndex >= dialogue.sentences.Count && !controller.isRunning)
            {
                PauseController.ResumeGame();
                isActive = false;

                ResetDialogue();

                // if the dialogue cannot be restarted
                if (autoStart)
                {
                    this.gameObject.SetActive(false);
                }

                return;
            };

            // open text box and turn off info text when the dialogue is started
            if (!autoStart && (currentIndex == 0 && !controller.isRunning))
            {
                ToggleTextBox(true);
                ToggleInfoText(false);
                isActive = true;
                PauseController.PauseGame();
            }

            // managing of the dialogue
            if (!controller.isRunning)
            {
                controller.StartDialogue(dialogue.name, dialogue.sentences[currentIndex]);
                currentIndex++;
            } else if (controller.isRunning)
            {
                controller.FinishSentence(dialogue.sentences[currentIndex-1]);
            }
        }
    }

    /// <summary>
    /// Resets dialogue and making it ready for a new round.
    /// </summary>
    private void ResetDialogue()
    {
        controller.Reset();
        currentIndex = 0;
        ToggleTextBox(false);
        ToggleInfoText(true);
    }

    /// <summary>
    /// Toggles the text box on and off.
    /// </summary>
    public void ToggleTextBox(bool state)
    {
        if (textBox == null) return;
        textBox.gameObject.SetActive(state);
    }

    /// <summary>
    /// Toggles info text on and off.
    /// </summary>
    /// <param name="state">New state of the game object</param>
    void ToggleInfoText(bool state)
    {
        if (infoText == null) return;
        infoText.gameObject.SetActive(state);
    }

    public static implicit operator DialogueInteraction(GameObject v)
    {
        throw new NotImplementedException();
    }
}
