using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.InputSystem;
using UnityEngine;

[CreateAssetMenu]
public class LocationQuest : Quest
{
    public List<Dialogue> startDialogue = new();
    public List<Dialogue> endDialogue = new();
}
