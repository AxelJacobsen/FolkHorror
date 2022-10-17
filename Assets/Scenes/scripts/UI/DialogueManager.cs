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
    int totalPages;
    int currentPage;

    // Start is called before the first frame update
    void Start()
    {
        index = -1;
        isRunning = false;
        totalPages = 0;
        currentPage = 0;
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
            totalPages = text.textInfo.pageCount;
            // change page if the current text contains multiple pages
            if (totalPages > 1 && (currentPage < totalPages))
            {
                ChangePage();
            } else
            // change sentence and reset page variables
            {
                currentPage = 1;
                text.pageToDisplay = 1;
                ChangeSentence();
            }
           
            //ChangePage();
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
        text.pageToDisplay++;
        currentPage++;
    }

    /// <summary>
    /// Write every letter in a sentence with a delay.
    /// </summary>
    /*IEnumerator IterateSentence()
    {
        int totalPages = text.textInfo.pageCount;
        int currentPage = 1;

        foreach (char c in sentences[index])
        {
            // check if the page has changed
            if (text.isTextOverflowing && (currentPage < totalPages))
            {
                text.pageToDisplay++;
                currentPage++;
            }
            text.text += c;
            //print(c);
           
            yield return new WaitForSeconds(0.05f);
        }
        isRunning = false;
    }*/
}
