using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    public Quest quest;
    public bool isStart;    // start quest or complete quest

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent == null || !other.transform.parent.gameObject.CompareTag("Player")) return;

        if (isStart && !quest.isCompleted && !quest.isRunning)
        {
            quest.isRunning = true;
            QuestManager.changedQuests.Add(quest);
        } else if (!isStart && quest.isRunning)
        {
            quest.isRunning = false;
            quest.isCompleted = true;
            QuestManager.changedQuests.Add(quest);
        }
    }
}
