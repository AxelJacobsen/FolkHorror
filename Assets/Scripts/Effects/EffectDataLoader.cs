using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OnTrigger
{
    public string   triggerID;
    public int      flatDamage;
    public int      percentDamage;
    public string   targetTag;
}

[System.Serializable]
public class EffectData
{
    //these variables are case sensitive and must match the strings "firstName" and "lastName" in the JSON.
    //public string effectName;
    public string effectName;
    public OnTrigger[] triggers;

    public OnTrigger GetTrigger(string triggerID){
        // Iterate triggers, looking this specific one
		OnTrigger trigger = null;
		foreach(OnTrigger trigger_t in triggers) {
			if (trigger_t.triggerID == triggerID) {
				trigger = trigger_t;
				break;
			}
		}

        // Return
        return trigger;
    }
}
 
[System.Serializable]
public class EffectDataCollection
{
    //employees is case sensitive and must match the string "employees" in the JSON.
    public EffectData[] effectDataArray;
}

/// <summary>
/// Loads effects from file.
/// </summary>
public class EffectDataLoader : MonoBehaviour
{
    public TextAsset jsonFile;
    static public EffectDataCollection effectDataCollection;

    void Start()
    {
        effectDataCollection = JsonUtility.FromJson<EffectDataCollection>(jsonFile.text);
    }

    /// <summary>
    /// Get the effectdata for a given effect(name).
    /// </summary>
    /// <param name="effectName">The name of the effect.</param>
    /// <returns>The effectdata of that effect.</returns>
    public EffectData GetEffectData(string effectName) {
        // Iterate loaded effects, looking this specific one
		EffectData effectData = null;
		foreach(EffectData effectData_t in effectDataCollection.effectDataArray) {
			if (effectData_t.effectName == effectName) {
				effectData = effectData_t;
				break;
			}
		}

        // Return
        return effectData;
    }
}