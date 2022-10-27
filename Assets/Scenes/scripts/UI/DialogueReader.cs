using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine;

/// <summary>
/// Class <c>DialogueReader</c> reads dialogue from XML file.
/// </summary>
public class DialogueReader : MonoBehaviour
{
    /// <summary>
    /// Reads XML files into an object.
    /// Code taken from: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/serialization/how-to-read-object-data-from-an-xml-file
    /// </summary>
    /// <typeparam name="T">Object the XML content is added to</typeparam>
    /// <param name="path">Path of the XML file</param>
    /// <returns>Object with values</returns>
    public static T ReadXML<T>(string path)
    {
        System.Xml.Serialization.XmlSerializer reader = 
            new System.Xml.Serialization.XmlSerializer(typeof(T));
        System.IO.StreamReader file = new System.IO.StreamReader(path);
        T obj = (T)reader.Deserialize(file);
        file.Close();

        return obj;
    }
}
