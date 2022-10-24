using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class <c>DialogueManager</c> manages a dialogue interaction.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public Image textBox;
    public TextMeshProUGUI text;

    public string[] sentences;
    string currentText;
    public int currentIndex;
    bool isRunning;
    bool reset;

    // Start is called before the first frame update
    void Start()
    {
        text.text = "";
        currentText = "";
        currentIndex = -1;
        isRunning = false;
        reset = true;
    }

    // Update is called once per frame
    void Update()
    {
        // first time the update loop is run
        if (Input.GetKeyDown(KeyCode.E) && currentIndex == -1)
        {
            ToggleTextBox(true);
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

    // TODO: make a general function for resetting variables (used in Start as well)
    void OnDisable()
    {
        StopAllCoroutines();
        text.text = "";
        currentText = "";
        currentIndex = -1;
        isRunning = false;
        reset = false;
        ToggleTextBox(false);
    }

    /// <summary>
    /// Toggles the text box on and off.
    /// </summary>
    public void ToggleTextBox(bool state)
    {
        textBox.gameObject.SetActive(state);
    }

    /// <summary>
    /// Change which sentence is displayed as the text.
    /// </summary>
    void ChangeSentence()
    {
        // check if dialogue is complete
        if (currentIndex >= sentences.Length - 1)
        {
            this.gameObject.SetActive(false);
            return;
        }

        // make text ready for typing
        text.text = "";

        // set new sentence
        currentIndex++;
        currentText = sentences[currentIndex];

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
