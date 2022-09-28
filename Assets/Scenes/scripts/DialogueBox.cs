using UnityEngine;

public class DialogueBox : MonoBehaviour
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
        
    }

    public void ToggleTextBox()
    {
        foreach (GameObject gameObj in gameObjects)
        {
            gameObj.SetActive(!gameObj.activeSelf);
        }
    }
}
