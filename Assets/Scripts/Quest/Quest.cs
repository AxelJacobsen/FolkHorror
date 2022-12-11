using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum QuestType
{
    Location,
    Kill
}

/// <summary>
/// Class <c>Quest</c> stores information about a quest.
/// </summary>
[CreateAssetMenu]
public class Quest : ScriptableObject
{
    public string title;
    public string description;
    public bool isRunning;
    public bool isCompleted;
    public QuestType type;
}
