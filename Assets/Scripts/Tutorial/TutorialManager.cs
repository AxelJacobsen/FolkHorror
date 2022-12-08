using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class <c>TutorialManager</c> controls the flow in the tutorial.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    public string playerTag = "Player";
    public string filePath;
    public GameObject enemy;
    public GameObject portal;
    public Button buttonSkip;

    private DialogueInteraction dialogueInteraction;
    private TutorialFragment currentTask;
    private Queue<TutorialFragment> tasks = new();
    private GameObject player;
    private bool menuIsOpen;

    // Start is called before the first frame update
    void Start()
    {
        menuIsOpen = false;

        dialogueInteraction = FindObjectOfType<DialogueInteraction>();
        dialogueInteraction.autoStart = true;

        portal.SetActive(false);

        // create tasks for the player to complete
        tasks.Enqueue(new TutorialFragment(TaskType.sword, "Dialogue/tutorial-movement"));
        tasks.Enqueue(new TutorialFragment(TaskType.enemy, "Dialogue/tutorial-combat"));
        tasks.Enqueue(new TutorialFragment(TaskType.portal, "Dialogue/tutorial-portal"));
        currentTask = tasks.Dequeue(); 

        // first part of the dialogue
        dialogueInteraction.dialogue = currentTask.dialogue;

        player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null) Debug.Log("Tutorialmanager could not find the player!");
        PauseController.PauseGame();
    }

    private void OnDisable()
    {
        menuIsOpen = false;
        PauseController.ResumeGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseController.isPaused)
        {
            buttonSkip.gameObject.SetActive(false);
            menuIsOpen = true;
            return;
        }

        if (menuIsOpen)
        {
            buttonSkip.gameObject.SetActive(true);
            menuIsOpen = false;
        }

        if (tasks == null) return;
        if (dialogueInteraction == null) return;
        if (currentTask == null) return;

        // start new task if the current one is completed
        if (currentTask.isCompleted == true)
        {
            currentTask = tasks.Dequeue();
            NewDialogue();
            currentTask.isStarted = true;
        }

        // the player can move freely and do the task
        if (!dialogueInteraction.isActiveAndEnabled)
        {
            PauseController.ResumeGame();

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
        PauseController.PauseGame();
        dialogueInteraction.dialogue = currentTask.dialogue;
        dialogueInteraction.gameObject.SetActive(true);
    }
}
