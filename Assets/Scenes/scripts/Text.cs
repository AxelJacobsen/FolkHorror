using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Text : MonoBehaviour
{
    public string[] sentences;
    int index;
    TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
        index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TypeText();
        }
    }

    void TypeText()
    {
        if (index >= sentences.Length)
        {
            index = 0;
        }
        text.text = sentences[index];
        index++;
    }
}
