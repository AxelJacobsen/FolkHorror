using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public DialogueManager manager;

    bool isRunning;

    // Start is called before the first frame update
    void Start()
    {
        infoText.gameObject.SetActive(false);
        manager.ToggleTextBox(false);
        manager.gameObject.SetActive(false);
        isRunning = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        infoText.gameObject.SetActive(true);
        manager.gameObject.SetActive(true);
    }

    private void OnTriggerStay(Collider other)
    {
        // remove info text when dialogue is run for the first time
        if (manager.textBox.IsActive() && !isRunning)
        {
            isRunning = true;
            infoText.gameObject.SetActive(false);
        }

        // show info text again when the dialogue is complete
        if (!manager.textBox.IsActive() && isRunning)
        {
            isRunning = false;
            infoText.gameObject.SetActive(true);
            manager.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        infoText.gameObject.SetActive(false);
        manager.gameObject.SetActive(false);
    }
}
