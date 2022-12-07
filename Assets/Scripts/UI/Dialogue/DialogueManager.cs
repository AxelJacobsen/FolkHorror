using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class <c>DialogueManager</c> manages a dialogue interaction.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public Image textBox;
    public TextMeshProUGUI text;
    public TextMeshProUGUI name;
    public Dialogue dialogue;
    public bool onEnter;

    string currentText;
    int currentIndex;
    bool isRunning;
    bool reset;

    // Start is called before the first frame update
    void Start()
    {
        ResetVariables();
        reset = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseController.isPaused) return;
        if (dialogue == null) return;

        // first time the update loop is run
        if (Input.GetKeyDown(KeyCode.E) && currentIndex == -1)
        {
            ToggleTextBox(true);
        } 
        
        if (onEnter && currentIndex == -1)
        {
            ToggleTextBox(true);
            ChangeSentence();
        }

        if (Input.GetKeyDown(KeyCode.E) && textBox.gameObject.activeSelf)
        {
            // check for overflow, then we know if there are still pages left
            if (isRunning)
            {
                isRunning = false;
                StopAllCoroutines();
                text.text = currentText;
            }
            else if (text.isTextOverflowing && reset)
            {
                ChangePage();
            } else
            {
                ChangeSentence();
                reset = true;
            }
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
        ResetVariables();
        reset = false;
        ToggleTextBox(false);
    }

    /// <summary>
    /// Set variable to their default values.
    /// </summary>
    private void ResetVariables()
    {
        text.text = "";
        name.text = "";
        currentText = "";
        currentIndex = -1;
        isRunning = false;
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
    /// Change which sentence is displayed as the text.
    /// </summary>
    void ChangeSentence()
    {
        // check if dialogue is complete
        if (currentIndex >= dialogue.sentences.Length - 1)
        {
            this.gameObject.SetActive(false);
            return;
        }

        // make text ready for typing
        text.text = "";

        // set new sentence
        currentIndex++;
        currentText = dialogue.sentences[currentIndex];

        // start typing effect
        isRunning = true;
        StartCoroutine(IterateSentence(currentText));
    }

    /// <summary>
    /// Change the page to be displayed from the current text.
    /// </summary>
    void ChangePage()
    {
        // remove used page from text
        text.text = text.text.Substring(text.firstOverflowCharacterIndex);
        currentText = text.text;

        text.text = "";

        // start typing current page
        isRunning = true;
        StartCoroutine(IterateSentence(currentText));
    }

    /// <summary>
    /// Write every letter in a sentence with a delay.
    /// </summary>
    IEnumerator IterateSentence(string page)
    {
        foreach (char c in page)
        {
            text.text += c;
           
            yield return new WaitForSeconds(0.05f);
        }
        isRunning = false;
    }
}
