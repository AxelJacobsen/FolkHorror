using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

/// <summary>
/// Class <c>DialogueTrigger</c> manages triggering of a dialogue interaction.
/// </summary>
public class DialogueTrigger : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public DialogueManager manager;

    public string filePath;
    bool isRunning;

    // Start is called before the first frame update
    void Start()
    {
        ToggleInfoText(false);
        manager.gameObject.SetActive(false);
        manager.dialogue = DialogueReader.ReadXML<Dialogue>(filePath);
        isRunning = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        ToggleInfoText(true);
        manager.gameObject.SetActive(true);
    }

    private void OnTriggerStay(Collider other)
    {
        // remove info text when dialogue is run for the first time
        if (manager.textBox.IsActive() && !isRunning)
        {
            isRunning = true;
            ToggleInfoText(false);
        }

        // show info text again when the dialogue is complete
        if (!manager.textBox.IsActive() && isRunning)
        {
            isRunning = false;
            ToggleInfoText(true);
            manager.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ToggleInfoText(false);
        manager.gameObject.SetActive(false);
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
}
