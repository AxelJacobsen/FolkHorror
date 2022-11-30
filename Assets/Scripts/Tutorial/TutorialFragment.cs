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

    // Start is called before the first frame update
    public TutorialFragment(TaskType taskType, string fileName)
    {
        this.taskType = taskType;
        dialogue = DialogueReader.LoadXML<Dialogue>(fileName);
        isCompleted = false;
        isStarted = false;
    }
}
