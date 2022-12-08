using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class <c>DialogueTriggerNew</c> enables dialogue when the user is inside a certain area.
/// </summary>
public class DialogueTriggerNew : MonoBehaviour
{
    public string filePath;
    public DialogueController controller;

    //private DialogueController controller;
    private Dialogue dialogue;

    private void Start()
    {
        //controller = FindObjectOfType<DialogueController>();
        //if (controller == null) print("DialogueTriggerNew couldn't find the controller");
        controller.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent == null || !other.transform.parent.gameObject.CompareTag("Player")) return;

        controller.gameObject.SetActive(true);

        dialogue = XmlLoader.LoadXML<Dialogue>(filePath);
        controller.dialogue = dialogue;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent == null || !other.transform.parent.gameObject.CompareTag("Player")) return;

        controller.gameObject.SetActive(false);
    }
}
