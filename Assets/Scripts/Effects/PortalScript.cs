using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalScript : MonoBehaviour
{
    public string SceneName;
    public string BiomeName;
    public bool isEntrance = false;
    public bool resetSpeed = false;
    
    // Start is called before the first frame update
    void Start()
    {
        //Activates the portal, insurance for entrance to set player death state
        this.transform.parent.gameObject.SetActive(true);
        //If this is an entrance marker then find player and teleport them to the entrance
        if (isEntrance) {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            Vector3 newPos = new Vector3(transform.position.x, playerObject.transform.position.y, transform.position.z);
            playerObject.transform.position = newPos;
        }

        if (resetSpeed)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            playerObject.GetComponent<PlayerController>().Speed = 10f;
        }
    }

    void OnTriggerEnter (Collider hit) { 
        //If it its an exit then teleport the player to the next stage on touch
        if (isEntrance && hit.transform.parent.tag == "Player") {
            this.transform.parent.gameObject.SetActive(false);
            return; 
        }
        else if (isEntrance || hit.transform.parent == null || !hit.transform.parent.gameObject.CompareTag("Player")) return;
        
        //Iterates open scenes to check where theplayer is, then updates biome and currentstage
        Scene[] openScenes = SceneManager.GetAllScenes();
        foreach (Scene sc in openScenes) {
            if (sc.name == "TownScene") {
                hit.transform.parent.GetComponent<PlayerController>().currentBiome = BiomeName;
            }
            if (sc.name == "MapGenScene") {
                hit.transform.parent.GetComponent<PlayerController>().currentStage++;
            }
        }
        ChangeScene();
    }

    /// <summary>
    /// Change the current scene.
    /// </summary>
    public void ChangeScene()
    {
        GameObject sceneLoaderObject = GameObject.FindGameObjectWithTag("SceneLoader");
        SceneLoader sceneLoader = sceneLoaderObject.GetComponent<SceneLoader>();
        sceneLoader.ChangeScene(SceneName);
    }
}
