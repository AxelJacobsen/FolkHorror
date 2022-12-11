using System.Collections;
using System.Collections.Generic;
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
    public int startNum = Statistics.GreenTrollsDied;
    public int killGoal;
    public int killCount;
    public Enemy enemy;

    /// <summary>
    /// Checks if the quest is completed.
    /// </summary>
    /// <returns></returns>
    public bool IsCompleted()
    {
        switch (enemy)
        {
            case Enemy.GreenTroll:
                killCount += Statistics.GreenTrollsDied - startNum;
                break;
            case Enemy.RedTroll:
                killCount += Statistics.RedTrollDied - startNum;
                break;
            case Enemy.Nisse:
                killCount += Statistics.NisseDied - startNum;
                break;
        }

        if (killCount >= killGoal) return true;
        return false;
    }
}
