using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using UnityEngine;

/// <summary>
/// Class <c>DialogueReader</c> reads dialogue from XML file.
/// </summary>
public class XmlLoader : MonoBehaviour
{
    /// <summary>
    /// Load XML files into an object.
    /// </summary>
    /// <param name="fileName">Name of the XML file</param>
    /// <returns>Object with values</returns>
    public static T LoadXML<T>(string fileName)
    {
        ResourceRequest r = Resources.LoadAsync(fileName);
        TextAsset textAsset = (TextAsset)r.asset;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);
        return DeserializeXmlDocument<T>(doc);
    }

    public static T DeserializeXmlDocument<T>(XmlDocument doc)
    {
        XmlReader reader = new XmlNodeReader(doc);
        var serializer = new XmlSerializer(typeof(T));
        T result = (T)serializer.Deserialize(reader);
        return result;
    }
}
