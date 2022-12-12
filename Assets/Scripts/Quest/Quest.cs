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
    public QuestType type;
    public Quest nextQuest;    
    public int status;

    private void OnEnable()
    {
        status = -1;
    }

    public virtual void Initialize()
    {
    }

    public virtual bool CheckCompleted()
    {
        return false;
    }
}
