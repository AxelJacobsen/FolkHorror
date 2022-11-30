using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetHeader(menu = "Item picker")]
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

    public bool spawnItem = false;
    private int weightTest = 0;
    public bool isEnemyItem = false;
    private GameObject spawnedItem;

    void Start() {
        HandleItem(weightTest);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.F4)) {
            weightTest++;
            HandleItem(weightTest);
        }
    }

    // should be overwritten by seperate shops 
    public virtual void HandleItem(int weight) { 
        GameObject pickedItem = GetRandomItem(weight);
        if (spawnItem) {
            Destroy(spawnedItem);
            spawnedItem = Instantiate(pickedItem, transform.position, Quaternion.Euler(45, 0, 0));
            if (isEnemyItem) {
                Weapon itemComp = spawnedItem.GetComponent<Weapon>();
                itemComp.PickedUpByTag = "Enemy";
            }
            spawnedItem.SetActive(true);
        }
    }

    /*
        Handles fetching items from all lists
     */
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

    /*
        Fetches item from specific list
     */
    private GameObject GetRandomItemFromList(List<GameObject> itemList) {
        if (itemList.Count == 0) { print("Item list empty"); return new GameObject { }; };
        return itemList[Random.Range(0, itemList.Count)];
    }
}
