using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>TutorialManager</c> controls the flow in the tutorial.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    public string     playerTag = "Player";
    public GameObject enemy;
    public GameObject textBox;
    public GameObject magicCube;
    public DialogueManager dialogueManager;
    public string filePath;

    private GameObject player;
    bool taskWalk;
    bool taskWeapon;
    bool taskCombat;
    bool taskCube;
    
    // Start is called before the first frame update
    void Start()
    {
        // Set the player's speed to zero
        player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null) Debug.Log("Tutorialmanager could not find the player!");
        SetPlayerSpeed(0);

        // first part of the dialogue
        dialogueManager.dialogue = DialogueReader.ReadXML<Dialogue>(filePath + "/tutorial.xml");
        dialogueManager.onEnter = true;
        dialogueManager.gameObject.SetActive(true);

        magicCube.SetActive(false);

        taskWalk = false;
        taskWeapon = false;
        taskCombat = false;
        taskCube = false;
    }

    // Update is called once per frame
    void Update()
    {
        // when the player should be able to move freely and complete tasks
        if (!dialogueManager.isActiveAndEnabled)
        {
            // pick up weapon
            if (!taskWalk)
            {
                SetPlayerSpeed(10f);
                taskWalk = true;
            }
            // kill enemy
            else if (!taskCombat && taskWeapon)
            {
                SetPlayerSpeed(10f);
                enemy.GetComponent<simpleEnemyAI>().Speed = 8f;
                taskCombat = true;
            }
            // touch the cube to start the game
            else if (taskCube)
            {
                SetPlayerSpeed(10f);
                magicCube.SetActive(true);
            }
        }
        
        // freeze the player while they go through the dialogue
        if (!taskWeapon && player.GetComponent<PlayerController>().Weapon != null)
        {
            NewDialogue("/tutorial2.xml");
            taskWeapon = true;
        } 
        else if (!taskCube && taskCombat && !enemy.GetComponent<Character>().enabled)
        {
            NewDialogue("/tutorial3.xml");
            taskCube = true;
        }
    }

    /// <summary>
    /// Load new dialogue to the dialogue manager and freeze the player.
    /// </summary>
    /// <param name="fileName">Name of the file that contains the dialogue</param>
    void NewDialogue(string fileName)
    {
        player.GetComponent<PlayerController>().Speed = 0f;
        dialogueManager.dialogue = DialogueReader.ReadXML<Dialogue>(filePath + fileName);
        dialogueManager.gameObject.SetActive(true);
    }

    /// <summary>
    /// Update the player's speed.
    /// </summary>
    /// <param name="newSpeed">The number the speed should be set to</param>
    void SetPlayerSpeed(float newSpeed)
    {
        player.GetComponent<PlayerController>().Speed = newSpeed;
    }
}
