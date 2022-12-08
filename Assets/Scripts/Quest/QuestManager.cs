using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
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
    public Canvas questCanvas;
    public GameObject questPrefab;
    public GameObject questList;
    public GameObject questOverview;
    public List<Quest> quests;

    private List<Quest> activeQuests;
    private List<GameObject> questPrefabs = new();
    private GameObject currentQuest;
    private bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        GameObject obj;
        for (int i = 0; i < quests.Count; i++)
        {
            obj = Instantiate(questPrefab, questList.transform);
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = quests[i].title;
            obj.GetComponent<Button>().AddEventLIstener(i, OnQuestClick);
            questPrefabs.Add(obj);
        }

        questCanvas.gameObject.SetActive(false);
        isOpen = false;
    }

    private void Update()
    {
        if (PauseController.isPaused && !isOpen) return;

        // menu is opened, which makes the quest UI close
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isOpen = false;
            DeselectQuest();
            questCanvas.gameObject.SetActive(false);
        }

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

    private void DeselectQuest()
    {
        if (currentQuest == null) return;
        currentQuest.GetComponent<Image>().color = new Color(106f / 255f, 123f / 255f, 100f / 255f);    // TODO: add colors to its own file
        questOverview.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        questOverview.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
    }

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
