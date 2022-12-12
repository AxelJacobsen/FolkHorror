using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

/// <summary>
/// Class <c>Dialogue</c> stores information about a dialogue interaction.
/// </summary>
[CreateAssetMenu]
public class Dialogue : ScriptableObject
{
    public string name;
    public List<string> sentences;
    public bool isQuest;
}
