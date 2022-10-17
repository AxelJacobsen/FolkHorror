using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Image textBox;
    public TextMeshProUGUI text;

    public string[] sentences;
    string currentText;
    int index;
    bool isRunning;

    // Start is called before the first frame update
    void Start()
    {
        text.text = "";
        currentText = "";
        index = -1;
        isRunning = false;
        ToggleTextBox();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleTextBox();
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
            else if (text.isTextOverflowing)
            {
                ChangePage();
            } else
            {
                ChangeSentence();
            }
        }
    }

    /// <summary>
    /// Toggles the text box on and off.
    /// </summary>
    void ToggleTextBox()
    {
        textBox.gameObject.SetActive(!textBox.gameObject.activeSelf);
    }

    /// <summary>
    /// Change which sentence is displayed as the text.
    /// </summary>
    void ChangeSentence()
    {
        if (index >= sentences.Length - 1)
        {
            index = -1;
        }

        // make text ready for typing
        text.text = "";

        // set new sentence
        index++;
        currentText = sentences[index];

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
