using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public DialogueManager manager;

    // Start is called before the first frame update
    void Start()
    {
        infoText.gameObject.SetActive(false);
        manager.ToggleTextBox(false);
        manager.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Heiiiii");
        infoText.gameObject.SetActive(true);
        manager.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        infoText.gameObject.SetActive(false);
        manager.gameObject.SetActive(false);
    }
}
