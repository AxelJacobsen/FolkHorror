using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Class <c>QuestTrigger</c> starts and ends quests when the player enters a specific area.
/// </summary>
public class QuestTrigger : MonoBehaviour
{
    public LocationQuest locQuest;
    public KillQuest killQuest;
    public DialogueTrigger dialogueTrigger;
    public bool isStart;    // start quest or complete quest

    private void Start()
    {
        dialogueTrigger = gameObject.GetComponent<DialogueTrigger>();
        if (dialogueTrigger == null) Debug.Log(gameObject.name + " could not find DialogueTrigger");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent == null || !other.transform.parent.gameObject.CompareTag("Player")) return;

        if (killQuest != null)
            KillQuest();
        else if (locQuest != null)
            LocationQuest();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent == null || !other.transform.parent.gameObject.CompareTag("Player")) return;

        dialogueTrigger.ExitDialogue();
    }

    /// <summary>
    /// Starts a dialogue.
    /// </summary>
    /// <param name="dialogue">The dialogue to be started</param>
    private void StartDialogue(Dialogue dialogue)
    {
        dialogueTrigger.dialogueObj = dialogue;
        dialogueTrigger.TriggerDialogue();
    }

    /// <summary>
    /// Manages kill quests.
    /// </summary>
    private void KillQuest()
    {
        switch (killQuest.Status)
        {
            case -1:
                // add starting variables
                killQuest.Initialize();
                killQuest.Status = 0;
                QuestManager.changedQuests.Add(killQuest);
                StartDialogue(killQuest.dialogue[0]);
                break;
            case 0:
                if (killQuest.CheckCompleted())
                {
                    killQuest.Status = 1;
                    StartDialogue(killQuest.dialogue[1]);
                }
                else
                    StartDialogue(killQuest.dialogue[2]);
                break;
            case 1:
                StartDialogue(killQuest.dialogue[3]);
                break;
        }
    }

    /// <summary>
    /// Manages location quests.
    /// </summary>
    private void LocationQuest()
    {
        if (isStart)
        {
            StartDialogue(locQuest.startDialogue[locQuest.Status + 1]);
            if (locQuest.Status == -1)
            {
                locQuest.Status = 0;
                QuestManager.changedQuests.Add(locQuest);
            }
        } else
        {
            StartDialogue(locQuest.endDialogue[locQuest.Status + 1]);
            if (locQuest.Status == 0)
            {
                locQuest.Status = 1;
                QuestManager.changedQuests.Add(locQuest);
            }
        }
    }
}
