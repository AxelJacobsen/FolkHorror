using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class <c>DialogueTriggerNew</c> enables dialogue when the user is inside a certain area.
/// </summary>
public class DialogueTrigger : MonoBehaviour
{
    public string filePath;

    private DialogueInteraction dialogueInteraction;
    private Dialogue dialogue;

    void Start()
    {
        // get DialogueInteraction
        GameObject dialogue = GameObject.Find("/Dialogue");
        if (dialogue == null) Debug.Log(gameObject.name + " could not find Dialogue");

        dialogueInteraction = dialogue.transform.Find("DialogueInteraction").GetComponent<DialogueInteraction>();
        if (dialogueInteraction == null) Debug.Log(gameObject.name + " could not find DialogueInteraction");

        dialogueInteraction.controller = GameObject.Find("/Dialogue/DialogueController").GetComponent<DialogueController>();
        if (dialogueInteraction.controller == null) Debug.Log(gameObject.name + " could not find DialogueController");

        dialogueInteraction.autoStart = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent == null || !other.transform.parent.gameObject.CompareTag("Player")) return;

        dialogueInteraction.gameObject.SetActive(true);

        dialogue = XmlLoader.LoadXML<Dialogue>(filePath);
        dialogueInteraction.dialogue = dialogue;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent == null || !other.transform.parent.gameObject.CompareTag("Player")) return;

        dialogueInteraction.gameObject.SetActive(false);
    }
}
