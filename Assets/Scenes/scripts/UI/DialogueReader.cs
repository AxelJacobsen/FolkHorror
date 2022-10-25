using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

/// <summary>
/// Class <c>DialogueReader</c> reads dialogue from XML file.
/// </summary>
public class DialogueReader : MonoBehaviour
{
    /// <summary>
    /// Read an xml file.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T ImportXml<T>(string path)
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                return (T)serializer.Deserialize(stream);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Exception importing xml file: " + e);
            return default;
        }
    }
}
