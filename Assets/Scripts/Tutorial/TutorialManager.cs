using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>TutorialManager</c> controls the flow in the tutorial.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    public string playerTag = "Player";
    public string filePath;
    public GameObject enemy;
    public GameObject portal;
    public DialogueManager dialogueManager;
    public GameObject textBox;

    private TutorialFragment currentTask;
    private Queue<TutorialFragment> tasks = new();
    private GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        portal.SetActive(false);
        // Set the player's speed to zero

        // create tasks for the player to complete
        tasks.Enqueue(new TutorialFragment(TaskType.sword, "Dialogue/tutorial-movement"));
        tasks.Enqueue(new TutorialFragment(TaskType.enemy, "Dialogue/tutorial-combat"));
        tasks.Enqueue(new TutorialFragment(TaskType.portal, "Dialogue/tutorial-portal"));
        currentTask = tasks.Dequeue(); 

        // first part of the dialogue
        dialogueManager.dialogue = currentTask.dialogue;
        dialogueManager.onEnter = true;
        dialogueManager.gameObject.SetActive(true);

        player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null) Debug.Log("Tutorialmanager could not find the player!");
        //player.GetComponent<Character>().Speed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (tasks == null) return;
        if (dialogueManager == null) return;
        if (currentTask == null) return;

        // start new task if the current one is completed
        if (currentTask.isCompleted == true)
        {
            currentTask = tasks.Dequeue();
        }

        // the player is frozen while they go through the dialogue
        if (!currentTask.isCompleted && !currentTask.isStarted)
        {
            NewDialogue();
            currentTask.isStarted = true;
        }

        // the player can move freely and do the task
        if (!dialogueManager.isActiveAndEnabled)
        {
            player.GetComponent<Character>().Speed = 10f;

            switch (currentTask.taskType)
            {
                case TaskType.sword:
                    if (player.GetComponent<PlayerController>().Weapon != null)
                        currentTask.isCompleted = true;
                    break;
                case TaskType.enemy:
                    if (enemy == null)
                    {
                        currentTask.isCompleted = true;
                        break;
                    }

                    enemy.GetComponent<Character>().Speed = 8f;
                    if (!enemy.GetComponent<Character>().enabled)
                        currentTask.isCompleted = true;
                    break;
                case TaskType.portal:
                    portal.SetActive(true);
                    break;
            }
        }
    }

    /// <summary>
    /// Load new dialogue to the dialogue manager and freeze the player.
    /// </summary>
    void NewDialogue()
    {
        player.GetComponent<Character>().Speed = 0f;
        dialogueManager.dialogue = currentTask.dialogue;
        dialogueManager.gameObject.SetActive(true);
    }
}
