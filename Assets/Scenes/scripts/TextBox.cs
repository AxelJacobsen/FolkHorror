using UnityEngine;

public class TextBox : MonoBehaviour
{
    private GameObject[] gameObjects;

    // Start is called before the first frame update
    void Start()
    {
        gameObjects = GameObject.FindGameObjectsWithTag("TextBox");
        // text box is initially invisible
        ToggleTextBox();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleTextBox();
        }
    }

    /// <summary>
    /// Toggles the text box on and off.
    /// </summary>
    public void ToggleTextBox()
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
