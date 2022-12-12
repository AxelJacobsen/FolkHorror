using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskType {
    sword,
    enemy,
    portal
}

/// <summary>
/// Class <c>TutorialFragment</c> stores information about a tutorial task.
/// </summary>
public class TutorialFragment
{
    public TaskType taskType;
    public Dialogue dialogue;
    public bool isCompleted;
    public bool isStarted;

    public TutorialFragment(TaskType taskType, Dialogue dialogue)
    {
        this.taskType = taskType;
        this.dialogue = dialogue;
        isCompleted = false;
        isStarted = false;
    }
}
