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
    public DialogueInteraction dialogueInteraction;

    private Dialogue dialogue;

    void Start()
    {
        dialogueInteraction.gameObject.SetActive(false);
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
