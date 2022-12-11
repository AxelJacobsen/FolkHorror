using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public static class ButtonExtension
{
    public static void AddEventLIstener<T>(this Button button, T param, Action<T> onClick)
    {
        button.onClick.AddListener(delegate ()
        {
            onClick(param);
        });
    }

}

/// <summary>
/// Class <c>QuestManager</c> controls the quests and quest UI.
/// </summary>
public class QuestManager : MonoBehaviour
{
    public static List<Quest> changedQuests = new();    // used to indicate change in quests

    public Canvas questCanvas;
    public GameObject questPrefab;
    public GameObject questList;
    public GameObject questOverview;

    private List<Quest> quests;
    private List<KillQuest> killQuests = new();
    private List<GameObject> questPrefabs = new();
    private GameObject currentQuest;
    private bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        quests = new();
        questCanvas.gameObject.SetActive(false);
        isOpen = false;
    }

    private void Update()
    {
        if (PauseController.isPaused && !isOpen) return;

        // modify list of quests if a change has been made
        if (changedQuests.Count > 0)
        {
            DeselectQuest();
            foreach (Quest q in changedQuests)
                ModifyQuest(q);
            changedQuests.Clear();
            return;
        }

        // check for quest completion
        CheckComplete();

        // menu is opened, which makes the quest UI close
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isOpen = false;
            DeselectQuest();
            questCanvas.gameObject.SetActive(false);
        }

        // manage quest UI
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isOpen = !isOpen;
            questCanvas.gameObject.SetActive(!questCanvas.gameObject.activeSelf);

            if (isOpen)
            {
                DeselectQuest();
                PauseController.PauseGame();
            } else
            {
                PauseController.ResumeGame();
            }
        }
    }

    /// <summary>
    /// Modify the changed quest. 
    /// </summary>
    /// <param name="changedQuest">The quest to be modified</param>
    private void ModifyQuest(Quest changedQuest)
    {
        // check if the changed quest is already in the list of enabled quests
        if (quests.Contains(changedQuest))
        {
            print("Quest " + changedQuest.title + " is completed");

            // if it is, make it inactive
            int index = quests.FindIndex(q => q.title.Equals(changedQuest.title));
            questPrefabs[index].GetComponent<Image>().color = new Color(76f / 255f, 82f / 255f, 81f / 255f);
            questPrefabs[index].GetComponent<Button>().onClick.RemoveAllListeners();
        }
        else
        {
            print("Quest " + changedQuest.title + " is started");

            // if it is not, add it to the list and instantiate a new prefab
            quests.Add(changedQuest);
            // add to its own list if it is a kill quest, used for checking if it has been completed
            if (changedQuest.type == QuestType.Kill) killQuests.Add((KillQuest)changedQuest);
            GameObject obj;
            obj = Instantiate(questPrefab, questList.transform);
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = quests[quests.Count - 1].title;
            obj.GetComponent<Button>().AddEventLIstener(quests.Count - 1, OnQuestClick);
            questPrefabs.Add(obj);
        }
    }

    /// <summary>
    /// Checks if kill quest(s) has/have been completed.
    /// </summary>
    private void CheckComplete()
    {
        if (killQuests.Count <= 0) return;

        // temporary list for storing the ones that should be deleted from the other list
        List<KillQuest> completedQuests = new();
        foreach (KillQuest quest in killQuests)
        {
            if (quest.IsCompleted())
            {
                quest.Status = 1;
                ModifyQuest(quest);
                completedQuests.Add(quest);
            }
        }

        // remove the completd ones from the "official" list
        killQuests = killQuests.Except(completedQuests).ToList();
    }

    /// <summary>
    /// Deselects the current quest.
    /// </summary>
    private void DeselectQuest()
    {
        if (currentQuest != null)
            currentQuest.GetComponent<Image>().color = new Color(106f / 255f, 123f / 255f, 100f / 255f);
        questOverview.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        questOverview.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
        currentQuest = null;
    }

    /// <summary>
    /// Handles quest on click by making it the currently active quest.
    /// </summary>
    /// <param name="index">Index of the clicked quest</param>
    private void OnQuestClick(int index)
    {
        DeselectQuest();

        // set new current quest
        currentQuest = questPrefabs[index];
        currentQuest.GetComponent<Image>().color = new Color(67f / 255f, 87f / 255f, 60f / 255f);

        // update the overview
        questOverview.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = quests[index].title;
        questOverview.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = quests[index].description;

    }
}
