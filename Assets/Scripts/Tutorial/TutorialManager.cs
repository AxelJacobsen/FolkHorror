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

    private DialogueController dialogueController;
    private TutorialFragment currentTask;
    private Queue<TutorialFragment> tasks = new();
    private GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        // start 
        dialogueController = FindObjectOfType<DialogueController>();
        dialogueController.autoStart = true;

        portal.SetActive(false);

        // create tasks for the player to complete
        tasks.Enqueue(new TutorialFragment(TaskType.sword, "Dialogue/tutorial-movement"));
        tasks.Enqueue(new TutorialFragment(TaskType.enemy, "Dialogue/tutorial-combat"));
        tasks.Enqueue(new TutorialFragment(TaskType.portal, "Dialogue/tutorial-portal"));
        currentTask = tasks.Dequeue(); 

        // first part of the dialogue
        dialogueController.dialogue = currentTask.dialogue;

        player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null) Debug.Log("Tutorialmanager could not find the player!");
        player.GetComponent<PlayerController>().Speed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (tasks == null) return;
        if (dialogueController == null) return;
        if (currentTask == null) return;

        // start new task if the current one is completed
        if (currentTask.isCompleted == true)
        {
            currentTask = tasks.Dequeue();
            NewDialogue();
            currentTask.isStarted = true;
        }

        // the player can move freely and do the task
        if (!dialogueController.isActiveAndEnabled)
        {
            player.GetComponent<PlayerController>().Speed = 10f;

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
        player.GetComponent<PlayerController>().Speed = 0f;
        dialogueController.dialogue = currentTask.dialogue;
        dialogueController.gameObject.SetActive(true);
    }
}
