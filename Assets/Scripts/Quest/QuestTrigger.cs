using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Class <c>QuestTrigger</c> starts and ends quests when the player enters a specific area.
/// </summary>
public class QuestTrigger : MonoBehaviour
{
    public Quest quest;
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

        if (isStart)
        {
            print(quest.Status);
            switch (quest.Status)
            {
                case -1:
                    // add starting variables
                    if (quest.type == QuestType.Kill)
                        quest.Initialize();
                    quest.Status = 0;
                    QuestManager.changedQuests.Add(quest);
                    break;
            }
        }

        /*if (isStart)
        {
            switch(quest.status)
            {
                case -1:
                    //dialogueTrigger.dialogueObj = quest.startDialogue[0];
                    quest.status = 0;
                    QuestManager.changedQuests.Add(quest);
                    break;
                case 0:
                    //dialogueTrigger.dialogueObj = quest.startDialogue[1];
                    break;
                case 1:
                    //dialogueTrigger.dialogueObj = quest.startDialogue[2];
                    break;
            }
        } else
        {
            switch(quest.status)
            {
                case -1:
                    //dialogueTrigger.dialogueObj = quest.endDialogue[0];
                    break;
                case 0:
                    //dialogueTrigger.dialogueObj = quest.endDialogue[1];
                    quest.status = 1;
                    QuestManager.changedQuests.Add(quest);
                    break;
                case 1:
                    //dialogueTrigger.dialogueObj = quest.endDialogue[2];
                    break;
            }
        }*/
    }
}
