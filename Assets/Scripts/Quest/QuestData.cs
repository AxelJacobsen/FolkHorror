using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>QuestData</c> stores information about which task is active in a game object.
/// </summary>
public static class QuestData
{
    public static Dictionary<string, Quest> activeQuests = new();

    /// <summary>
    /// Add data to the dictionary.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="quest">The value</param>
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

    /// <summary>
    /// Remove entry in dictionary.
    /// </summary>
    /// <param name="key">The key to remove the value of</param>
    public static void RemoveQuestData(string key)
    {
        if (activeQuests.ContainsKey(key))
        {
            activeQuests.Remove(key);
        }
    } 
}
