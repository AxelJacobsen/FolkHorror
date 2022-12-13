using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour {
    [Header("Item Lists")]
    public List<GameObject> Common_Items;
    public List<GameObject> Rare_Items,
                            Legendary_Items;

    [Header("Thresholds")]
    public int rareThresh = 65;    // numbers above can be rare or legandary
    public int legendThresh = 95,  // numbers above will be legendary
               upperThresh = 100;  // marks the highest number that can be generated,
                                   // could be increased for overall better items
    public bool isEnemyItem = false;
    private GameObject spawnedItem;

    void Start() {
        int pWeight = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().currentStage;
        if (isEnemyItem) {
            GiveEnemyWeapon();
        } else {
            HandleItem(pWeight);
        }
    }

    /// <summary>
    /// Handles spawning random items
    /// </summary>
    /// <param name="weight"></param>
    public virtual void HandleItem(int weight) {
        GameObject pickedItem = GetRandomItem(weight); ;
        if (pickedItem == null) { return; }
        //Insurance   
        Destroy(spawnedItem);
        spawnedItem = Instantiate(pickedItem, transform.position, Quaternion.Euler(45, 0, 0));
        spawnedItem.GetComponent<Item>().destructable = true;
        spawnedItem.SetActive(true);
    }

    /// <summary>
    /// Gives an enemy its weapon
    /// </summary>
    private void GiveEnemyWeapon() {
        //If an enemy item is to be spawned then force it to whatever bias the enemy has
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject pickedItem = null;
        //Iterates all enemies and looks for the closest one 
        foreach (GameObject enemy in allEnemies) {
            if ((enemy.transform.position - transform.position).magnitude <= 5.0f) {
                // Since they have diffrent heights, this compensates
                GameObject bias = enemy.GetComponent<BaseEnemyAI>().DesiredWeapon;
                if (bias == null) { return; }
                pickedItem = bias;
            }
        }

        if (pickedItem == null) { return; }
        //Insurance
        Destroy(spawnedItem);
        spawnedItem = Instantiate(pickedItem, transform.position, Quaternion.Euler(45, 0, 0));
        Weapon eWeap = spawnedItem.GetComponent<Weapon>();
        eWeap.PickedUpByTag = "Enemy";
        spawnedItem.SetActive(true);
    }

    /// <summary>
    /// Handles fetching items from all lists
    /// </summary>
    private GameObject GetRandomItem(int weight) {
        weight *= 10;
        if (weight < 0) { weight = 0; } else if (upperThresh<weight) { weight = legendThresh; };
        //generate item rarity
        int itemHit = Random.Range(weight, upperThresh);
        //check for item rarity
        if (itemHit < rareThresh) {
            //Generate common item
            return GetRandomItemFromList(Common_Items);
        } else if (itemHit < legendThresh) {
            //Generate rare item
            return GetRandomItemFromList(Rare_Items);
        } else {
            //Generate legendary item
            return GetRandomItemFromList(Legendary_Items);
        }
    }

    /// <summary>
    /// Fetches item from specific list
    /// </summary>
    /// <param name="itemList"></param>
    /// <returns></returns>
    private GameObject GetRandomItemFromList(List<GameObject> itemList) {
        if (itemList.Count <= 0) { print("Item list empty"); return null; };
        return itemList[Random.Range(0, itemList.Count)];
    }
}
