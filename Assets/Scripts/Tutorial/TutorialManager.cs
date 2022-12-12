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
    public List<Dialogue> dialogues = new();

    private DialogueInteraction dialogueInteraction;
    private TutorialFragment currentTask;
    private Queue<TutorialFragment> tasks = new();
    private GameObject player;
    private bool menuIsOpen;

    // Start is called before the first frame update
    void Start()
    {
        // create tasks for the player to complete
        tasks.Enqueue(new TutorialFragment(TaskType.sword, dialogues[0]));
        tasks.Enqueue(new TutorialFragment(TaskType.enemy, dialogues[1]));
        tasks.Enqueue(new TutorialFragment(TaskType.portal, dialogues[2]));
        currentTask = tasks.Dequeue(); 

        // get DialogueInteraction
        GameObject dialogue = GameObject.Find("/Dialogue");
        if (dialogue == null) Debug.Log(gameObject.name + " could not find Dialogue");

        dialogueInteraction = dialogue.transform.Find("DialogueInteraction").GetComponent<DialogueInteraction>();
        if (dialogueInteraction == null) Debug.Log(gameObject.name + " could not find DialogueInteraction");

        dialogueInteraction.controller = GameObject.Find("/Dialogue/DialogueController").GetComponent<DialogueController>();
        if (dialogueInteraction.controller == null) Debug.Log(gameObject.name + " could not find DialogueController");

        dialogueInteraction.gameObject.SetActive(true);
        dialogueInteraction.autoStart = true;

        // first part of the dialogue
        dialogueInteraction.dialogue = currentTask.dialogue;

        // get Player
        player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null) Debug.Log(gameObject.name + " could not find Player");

        menuIsOpen = false;
        portal.SetActive(false);

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
