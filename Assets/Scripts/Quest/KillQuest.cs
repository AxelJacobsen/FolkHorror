using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public enum Enemy
{
    GreenTroll,
    RedTroll,
    Nisse
}

/// <summary>
/// Class <c>KillQuest</c> stores information of a quest of type kill.
/// </summary>
[CreateAssetMenu]
public class KillQuest : Quest
{
    public List<int> killGoal;
    public List<Enemy> enemy;

    private List<int> startNum = new();
    private List<int> killCount = new();
    private List<bool> killDone = new();

    public override void Initialize()
    {
        startNum.Add(Statistics.GreenTrollsDied);
        startNum.Add(Statistics.RedTrollDied);
        startNum.Add(Statistics.NisseDied);

        killCount = Enumerable.Repeat(0, enemy.Count).ToList();
        killDone = Enumerable.Repeat(false, enemy.Count).ToList();

        base.Initialize();
    }

    /// <summary>
    /// Checks if the quest is completed.
    /// </summary>
    /// <returns></returns>
    public bool IsCompleted()
    {
        for (int i = 0; i < enemy.Count; i++)
        {
            // skip new count if a specific part is completed
            if (killDone[i]) continue;

            switch (enemy[i])
            {
                case Enemy.GreenTroll:
                    killCount[i] =  Statistics.GreenTrollsDied - startNum[i];
                    break;
                case Enemy.RedTroll:
                    killCount[i] += Statistics.RedTrollDied - startNum[i];
                    break;
                case Enemy.Nisse:
                    killCount[i] += Statistics.NisseDied - startNum[i];
                    break;
            }

            if (killCount[i] >= killGoal[i])
                killDone[i] = true;
        }

        if (!killDone.Contains(false)) return true;
        return false;
    }
}
