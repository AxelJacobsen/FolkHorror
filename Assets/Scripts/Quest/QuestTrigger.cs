using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    // -1 if waiting for the user to go throug the current dialogue, 0 if the user is currently going through the current dialogue, 1 if the user needs a new dialogue
    public static int state;

    public LocationQuest locationQuest;
    public KillQuest killQuest;
    public bool isStart;                // starting point or end point of a location quest

    private DialogueTrigger dialogueTrigger;

    // Start is called before the first frame update
    void Start()
    {
        dialogueTrigger = gameObject.GetComponent<DialogueTrigger>();
        if (dialogueTrigger == null) Debug.Log(gameObject.name + " could not find DialogueTrigger");

        state = -1;
    }

    private void OnEnable()
    {
        if (QuestData.activeQuests.ContainsKey(gameObject.name))
        {
            killQuest = null;
            locationQuest = null;

            Quest quest = QuestData.activeQuests[gameObject.name];
            if (quest.type == QuestType.Location)
            {
                locationQuest = (LocationQuest)quest;
            }
            else if (quest.type == QuestType.Kill)
            {
                killQuest = (KillQuest)quest;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent == null || !other.transform.parent.gameObject.CompareTag("Player")) return;

        // first time the player enters the trigger in the current loop
        if (locationQuest != null)
        {
            LocationQuest();
        }
        else if (killQuest != null)
        {
            KillQuest();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.parent == null || !other.transform.parent.gameObject.CompareTag("Player")) return;

        // either waiting for user to start dialogue or for the current dialogue to finish
        if (state == -1 && state == 0) return;

        // new dialogue needs to be loaded
        if (state == 1)
        {
            if (locationQuest != null)
                LocationQuest();
            else if (killQuest != null)
                KillQuest();

            state = -1;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        state = -1;
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
        locationQuest = null;
        killQuest = null;

        if(quest.nextQuest.type == QuestType.Location)
        {
            locationQuest = (LocationQuest)quest.nextQuest;
            QuestData.AddQuestData(gameObject.name, locationQuest);
        }
        else if (quest.nextQuest.type == QuestType.Kill)
        {
            killQuest = (KillQuest)quest.nextQuest;
            QuestData.AddQuestData(gameObject.name, killQuest);
        }
    }

    /// <summary>
    /// Manages location quests.
    /// </summary>
    public void LocationQuest()
    {
        if (isStart)
        {
            StartDialogue(locationQuest.startDialogue[locationQuest.status + 1]);
            
            // start quest if it has not been started or completed
            if (locationQuest.status == -1)
            {
                locationQuest.status = 0;
                QuestManager.changedQuests.Add(locationQuest);
                QuestData.AddQuestData(gameObject.name, locationQuest);
            }
        } else
        {
            StartDialogue(locationQuest.endDialogue[locationQuest.status + 1]);

            // end quest if it has been started and is not completed
            if (locationQuest.status == 0)
            {
                locationQuest.status = 1;
                QuestManager.changedQuests.Add(locationQuest);

                // look for next quest in line
                if (locationQuest.nextQuest != null)
                    StartNewQuest(locationQuest);
            }
        }
    }

    /// <summary>
    /// Manages kill quests.
    /// </summary>
    public void KillQuest()
    {
        switch (killQuest.status)
        {
            case -1:
                // add starting variables
                killQuest.Initialize();
                killQuest.status = 0;
                QuestManager.changedQuests.Add(killQuest);
                StartDialogue(killQuest.dialogue[0]);
                break;
            case 0:
                // see if the quest has been completed
                if (killQuest.CheckCompleted())
                {
                    StartDialogue(killQuest.dialogue[1]);
                    killQuest.status = 1;
                    QuestManager.changedQuests.Add(killQuest);

                    // look for next quest in line
                    if (killQuest.nextQuest != null)
                        StartNewQuest(killQuest);
                }
                else
                    // still not completed dialogue
                    StartDialogue(killQuest.dialogue[2]);
                break;
            case 1:
                // basic after quest dialogue
                StartDialogue(killQuest.dialogue[3]);
                break;
        }
    }
}
