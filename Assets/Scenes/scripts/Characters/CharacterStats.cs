using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

/// <summary>
/// A function that alters stats, taking one as input and returning the updated version.
/// </summary>
public delegate CharacterStats AlterStats(CharacterStats stats);

/// <summary>
/// A struct for storing and managing character stats.
/// </summary>
public class CharacterStats : MonoBehaviour
{

    [field: SerializeField] public float Speed      { get; set; }
    [field: SerializeField] public float MaxHealth  { get; set; }

    /// <summary>
    /// Gets the class' properties (fields).
    /// </summary>
    /// <returns>An array containing the properties of this class, which aren't inherited and is public.</returns>
    private PropertyInfo[] GetProps() {
        return typeof(CharacterStats).GetProperties(
            System.Reflection.BindingFlags.Public | 
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.DeclaredOnly
        );
    }

    /// <summary>
    /// Creates a shallow copy of this.
    /// </summary>
    /// <returns>A shallow copy of this.</returns>
    public CharacterStats Copy()
    {
        return (CharacterStats)this.MemberwiseClone();
    }

    /// <summary>
    /// Takes two stats, returning a new one where each property is equal to math(firstStat, secondStat).
    /// I.e.: math = (aField, bField) => aField + bField means adding every stat in a and b.
    /// </summary>
    /// <param name="a">The first statblock.</param>
    /// <param name="b">The second statblock.</param>
    /// <param name="math">A function taking two floats and returning a third.</param>
    /// <returns>A statblock where each element corresponds to math(a.field, b.field).</returns>
    private static CharacterStats aMathb(CharacterStats a, CharacterStats b, Func<float, float, float> math)
    {
        // Create a copy to work on
        CharacterStats delta = a.Copy();

        // Fetch properties and compare them
        PropertyInfo[] properties = a.GetProps();
        foreach (var property in properties)
        {
            // Fetch properties from a and b
            PropertyInfo prop = typeof(CharacterStats).GetProperty(property.Name);
            object  aVal = prop.GetValue(a, null),
                    bVal = prop.GetValue(b, null);

            // Depending on a's type, convert to float and use math() to get the final value.
            switch (aVal)
            {
                case float aFloat:
                    prop.SetValue(delta, math(aFloat, (float)bVal), null);
                    break;

                case int aInt:
                    prop.SetValue(delta, (int)math((float)aInt, (float)((int)bVal)), null);
                    break;

                default:
                    break;
            }
        }

        // Return
        return delta;
    }

    public static CharacterStats operator -(CharacterStats a, CharacterStats b)
    {
        return aMathb(a, b, (x,y) => x-y );
    }

    public static CharacterStats operator +(CharacterStats a, CharacterStats b)
    {
        return aMathb(a, b, (x, y) => x + y);
    }

    /// <summary>
    /// Applies a set of changes to a copy of this, then returns it.
    /// </summary>
    /// <param name="alterStatsList">A list of stat-changing functions.</param>
    /// <returns>The updated stats.</returns>
    public CharacterStats CalculateStats(List<AlterStats> alterStatsList)
    {
        // Create a copy to work on.
        CharacterStats postChange = Copy();

        // Apply each stat-altering function (in order) to the copy.
        foreach (AlterStats alterStats in alterStatsList)
        {
            postChange = alterStats(postChange);
        }

        // Return
        return postChange;
    }

    /// <summary>
    /// Sets these stats to be equal to the new stats.
    /// </summary>
    /// <param name="newStats">The new stats.</param>
    protected void SetStats(CharacterStats newStats)
    {
        // Fetch properties and iterate them, setting each of this' to the newStat equivalent.
        PropertyInfo[] properties = GetProps();
        foreach (var property in properties)
        {
            PropertyInfo prop = this.GetType().GetProperty(property.Name);
            prop.SetValue(this, property.GetValue(newStats, null), null);
        }
    }
}
