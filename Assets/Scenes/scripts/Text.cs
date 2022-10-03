using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Text : MonoBehaviour
{
    public string[] sentences;
    int index;
    TextMeshProUGUI text;
    bool isRunning;

    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
        index = -1;
        isRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // if coroutine is running, the current sentence needs to be completely written out
            if (isRunning)
            {
                isRunning = false;
                StopAllCoroutines();
                text.text = sentences[index];
            } else
            {
                ChangeSentence();
            }
        }
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
