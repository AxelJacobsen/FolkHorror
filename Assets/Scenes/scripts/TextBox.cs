using UnityEngine;

public class TextBox : MonoBehaviour
{
    private GameObject[] gameObjects;

    // Start is called before the first frame update
    void Start()
    {
        gameObjects = GameObject.FindGameObjectsWithTag("TextBox");
        // text box is initially invisible
        // TODO: currently does not work, but did earlier?
        ToggleTextBox();
    }

    // Update is called once per frame
    void Update()
    {
        ToggleTextBox();
    }

    /// <summary>
    /// Toggles the text box on and off.
    /// </summary>
    public void ToggleTextBox()
    {
        if (Input.GetKeyDown(KeyCode.E)) {
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.SetActive(!gameObject.activeSelf);
            }
        }
    }
}
