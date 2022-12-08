using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu]
public class Quest : ScriptableObject
{
    public string title;
    public string description;
    public bool isRunning;
    public bool isCompleted;
}
