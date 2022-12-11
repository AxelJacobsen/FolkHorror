using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>DialogueReset</c> is used to set the starting states of dialogue related objects.
/// </summary>
public class DialogueReset : MonoBehaviour
{
    public DialogueController controller;
    public DialogueInteraction interaction;

    // Start is called before the first frame update
    void Start()
    {
        controller.gameObject.SetActive(true);
        interaction.gameObject.SetActive(false);
    }
}
