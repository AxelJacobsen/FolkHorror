using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A function that alters stats, taking one as input and returning the updated version.
/// </summary>
public delegate CharacterStats AlterStats(CharacterStats stats);

/// <summary>
/// A struct for keeping 
/// </summary>
public class CharacterStats : MonoBehaviour
{
    [Header("Stats")]
    public float    Speed = 10f;
    public int      MaxHealth = 100;

    /// <summary>
    /// Calculates the difference in stats between this and other.
    /// </summary>
    /// <param name="other">The other set of stats.</param>
    /// <returns>The difference in stats between this and other.</returns>
    public CharacterStats CalculateDiff (CharacterStats other)
    {
        CharacterStats delta = (CharacterStats)this.MemberwiseClone();
        delta.Speed -= other.Speed;
        delta.MaxHealth -= other.MaxHealth;
        return delta;
    }

    /// <summary>
    /// Applies a set of changes to a copy of this, then returns it.
    /// </summary>
    /// <param name="alterStatsList">A list of stat-changing functions.</param>
    /// <returns>The updated stats.</returns>
    public CharacterStats CalculateStats(List<AlterStats> alterStatsList)
    {
        CharacterStats postChange = (CharacterStats)this.MemberwiseClone();
        foreach (AlterStats alterStats in alterStatsList)
        {
            postChange = alterStats(postChange);
        }

        return postChange;
    }
}
