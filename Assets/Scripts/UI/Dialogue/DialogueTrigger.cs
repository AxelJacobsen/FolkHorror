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
    public Dialogue dialogueObj;

    private DialogueInteraction dialogueInteraction;

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

        TriggerDialogue();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent == null || !other.transform.parent.gameObject.CompareTag("Player")) return;

        ExitDialogue();
    }

    public void TriggerDialogue()
    {
        dialogueInteraction.dialogue = dialogueObj;
        dialogueInteraction.gameObject.SetActive(true);
    }

    public void ExitDialogue()
    {
        dialogueInteraction.gameObject.SetActive(false);
    }
}
