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
    int index;
    bool isRunning;

    // Start is called before the first frame update
    void Start()
    {
        ToggleTextBox();
        index = -1;
        text.text = "";
        isRunning = false;
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
            if (text.isTextOverflowing)
            {
                ChangePage();
            } else
            {
                ChangeSentence();
            }

            /*/ if coroutine is running, the current sentence needs to be completely written out
            if (isRunning)
            {
                isRunning = false;
                StopAllCoroutines();
                text.text = sentences[index];
            }
            else
            {
                ChangeSentence();
            }*/
        }
    }

    // OnDisable is called when the game object becomes disabled
    void OnDisable()
    {
        text.text = "";
        index = -1;
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

        // set new sentence
        index++;
        text.text = sentences[index];

        /*text.text = "";
        isRunning = true;
        index++;
        StartCoroutine(IterateSentence());*/
    }

    /// <summary>
    /// Change the page to be displayed from the current text.
    /// </summary>
    void ChangePage()
    {
        // remove current text from text game object
        text.text = text.text.Substring(text.firstOverflowCharacterIndex);
    }

    /// <summary>
    /// Write every letter in a sentence with a delay.
    /*// </summary>
    IEnumerator IterateSentence()
    {
        foreach (char c in sentences[index])
        {
            text.text += c;
            //print(c);
           
            yield return new WaitForSeconds(0.05f);
        }
        isRunning = false;
    }*/
}
