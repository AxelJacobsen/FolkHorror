using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using TMPro;
using UnityEngine;

/// <summary>
/// Class <c>InfoScreen</c> controls the info screen and its related game objects.
/// </summary>
public class InfoScreen : MonoBehaviour
{
    public string fileName;
    public Canvas infoCanvas;
    public DialogueInteraction infoScreenInteraction;
    public DialogueController infoScreenController;

    // Start is called before the first frame update
    void Start()
    {
        infoScreenInteraction.dialogue = XmlLoader.LoadXML<Dialogue>(fileName);
        infoScreenInteraction.autoStart = true;
        infoScreenInteraction.controller = infoScreenController;
        ToggleInfoScreen(false);
    }

    /// <summary>
    /// Actives/deactiviates the info screen.
    /// </summary>
    /// <param name="state">True to activate and false to deactivate</param>
    public void ToggleInfoScreen(bool state)
    {
        infoCanvas.gameObject.SetActive(state);
        infoScreenController.gameObject.SetActive(state);
        infoScreenInteraction.gameObject.SetActive(state);
    }
}
