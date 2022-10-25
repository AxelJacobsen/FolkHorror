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
[XmlRoot(ElementName = "dialogue")]
public class Dialogue
{
    [XmlElement(ElementName = "name")]
    public string name;

    [XmlElement(ElementName = "sentences")]
    public string sentences;

    [XmlArray("test"), XmlArrayItem("value")]
    public string[] test;
}


