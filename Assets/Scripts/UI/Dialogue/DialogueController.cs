using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public Image textBox;
    public string title;
    public List<string> sentences;

    private Dialogue dialogue;
    private int currentIndex;
    private bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        ToggleTextBox(true);
        currentIndex = 0;
        isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentIndex >= sentences.Count && !DialogueManagerNew.isRunning) return;
        // look for user input
        // 
        if (Input.GetKeyDown(KeyCode.E))
        {
            // start dialogue
            if (!DialogueManagerNew.isRunning)
            { 
                FindObjectOfType<DialogueManagerNew>().StartDialogue(title, sentences[currentIndex]);
                currentIndex++;
            } else if (DialogueManagerNew.isRunning)
            {
                print("enhi");
                FindObjectOfType<DialogueManagerNew>().FinishSentence(sentences[currentIndex-1]);
            }
        }
    }

    /// <summary>
    /// Toggles the text box on and off.
    /// </summary>
    public void ToggleTextBox(bool state)
    {
        if (textBox == null) return;
        textBox.gameObject.SetActive(state);
    }
}
