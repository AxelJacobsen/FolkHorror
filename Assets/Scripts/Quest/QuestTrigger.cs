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

    private void OnEnable()
    {
        if (QuestData.activeQuests.ContainsKey(gameObject.name))
        {
            Quest quest = QuestData.activeQuests[gameObject.name];
            if (quest.type == QuestType.Location)
                locQuest = (LocationQuest)quest;
            else if (quest.type == QuestType.Kill)
                killQuest = (KillQuest)quest;
        }
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
    /// Starts the next quest in line.
    /// </summary>
    /// <param name="quest">The quest to be started</param>
    private void StartNewQuest(Quest quest)
    {
        // calles når playeren er ferdig med en quest
        locQuest = null;
        killQuest = null;

        // sjekk om den har en next quest
        if (quest.nextQuest != null)
        {
            if (quest.nextQuest.type == QuestType.Location)
            {
                locQuest = (LocationQuest)quest.nextQuest;
                QuestData.AddQuestData(gameObject.name, locQuest);
            }
            else if (quest.nextQuest.type == QuestType.Kill) { 
                killQuest = (KillQuest)quest.nextQuest;
                QuestData.AddQuestData(gameObject.name, killQuest);
            }
        }
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
                QuestData.AddQuestData(gameObject.name, killQuest);
                break;
            case 0:
                if (killQuest.CheckCompleted())
                {
                    killQuest.Status = 1;
                    StartDialogue(killQuest.dialogue[1]);
                    StartNewQuest(killQuest);
                }
                else
                    StartDialogue(killQuest.dialogue[2]);
                break;
            case 1:
                QuestManager.changedQuests.Add(killQuest);
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
                QuestData.AddQuestData(gameObject.name, locQuest);
            }
        } else
        {
            StartDialogue(locQuest.endDialogue[locQuest.Status + 1]);
            if (locQuest.Status == 0)
            {
                locQuest.Status = 1;
                QuestManager.changedQuests.Add(locQuest);
                StartNewQuest(locQuest);
            }
        }
    }
}
