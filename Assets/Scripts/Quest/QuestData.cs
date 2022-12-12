using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuestData
{
    public static Dictionary<string, Quest> activeQuests = new();

    public static void AddQuestData(string key, Quest quest)
    {
        if (activeQuests.ContainsKey(key))
        {
            activeQuests[key] = quest;
        } else
        {
            activeQuests.Add(key, quest);
        }
    }
}
