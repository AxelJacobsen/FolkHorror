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
    public bool hasParent;  // if quest is first quest in the line or not
    
    private int status = -1;
    public int Status { 
        get { return status; }
        set { status = value; } 
    }

    public virtual void Initialize()
    {
    }

    public virtual bool CheckCompleted()
    {
        return false;
    }
}
