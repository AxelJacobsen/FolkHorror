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
        if (Input.GetKeyDown(KeyCode.E))
        {
            // if coroutine is running, the current sentence needs to be completely written out
            if (isRunning)
            {
                isRunning = false;
                StopAllCoroutines();
                text.text = sentences[index];
            }
            else
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

    // OnDisable is called when the game object becomes disabled
    void OnDisable()
    {
        text.text = "";
        index = -1;
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
        text.text = "";
        isRunning = true;
        index++;
        StartCoroutine(IterateSentence());
    }

    /// <summary>
    /// Write every letter in a sentence with a delay.
    /// </summary>
    IEnumerator IterateSentence()
    {
        foreach (char c in sentences[index])
        {
            text.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        isRunning = false;
    }
}
