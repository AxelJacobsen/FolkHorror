using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Class <c>DialogueManagerNew</c> contains functionality to start, continue and complete a dialogue.
/// </summary>
public class DialogueController : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI text;
    public bool isRunning;

    private void Start()
    {
        isRunning = false;
    }

    public void Reset()
    {
        title.text = "";
        text.text = "";
        isRunning = false;
    }

    /// <summary>
    /// Starts a dialogue interaction.
    /// </summary>
    /// <param name="title">The title of the dialogue</param>
    /// <param name="sentence">The first sentence in the dialogue</param>
    public void StartDialogue(string title, string sentence)
    {
        this.title.text = title;
        ChangeSentence(sentence);
    }

    /// <summary>
    /// Finishes a sentence.
    /// </summary>
    /// <param name="sentence">The sentence to be finished</param>
    public void FinishSentence(string sentence)
    {
        isRunning = false;
        StopAllCoroutines();
        text.text = sentence;
    }

    /// <summary>
    /// Starts typing out a new sentence.
    /// </summary>
    /// <param name="sentence">The sentence to be typed out</param>
    private void ChangeSentence(string sentence)
    {
        text.text = "";
        StartCoroutine(IterateSentence(sentence));
    }

    /// <summary>
    /// Write every letter in a sentence with a delay.
    /// </summary>
    private IEnumerator IterateSentence(string sentence)
    {
        isRunning = true;
        foreach (char c in sentence)
        {
            text.text += c;

            yield return new WaitForSecondsRealtime(0.05f);
        }
        isRunning = false;
    }
}
