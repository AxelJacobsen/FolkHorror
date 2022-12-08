using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManagerNew : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI text;
    public static bool isRunning;

    private void Start()
    {
        isRunning = false;
    }

    public void StartDialogue(string name, string sentence)
    {
        title.text = name;
        ChangeSentence(sentence);
    }

    public void FinishSentence(string sentence)
    {
        print("tihi");
        isRunning = false;
        StopAllCoroutines();
        text.text = sentence;
    }

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
        print(isRunning);
        foreach (char c in sentence)
        {
            text.text += c;

            yield return new WaitForSeconds(0.05f);
        }
        isRunning = false;
        print(isRunning);
    }
}
