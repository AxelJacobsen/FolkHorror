
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class ShopManager : MonoBehaviour
{

    // Public
   
    public GameObject Entity;                   // the temp holder for Character Script component loading
    public enum Rarity
    {
        Common,
        Rare,
        Legendary
    }
   
    [Serializable]
    public struct itemProperties 
    {
        public GameObject item;
        public Rarity rarity;
    }
    [Header("Base price of rarity")]
    public bool addRandomRange;
    public int common, rare, legendary, range;
    [Header("Color of rarity")]
    public bool enableCustomColor;
    public Color commonBox, rareBox, legendaryBox, commonText, rareText, legendaryText;
    [Header("Testing shop item input")]
    public bool isActive;
    public int amount;

    // Private
    [SerializeField] private AudioClip buySound;
    [SerializeField] private itemProperties[] items;
    [Header("Item template")]
    [SerializeField] private GameObject itemTemplatePrefab;
    private PlayerController entityCharacterScript;    // Loaded character script holder
    private Dictionary<Rarity, int> price = new Dictionary<Rarity, int>();
    private Transform container;
    private int numRange;
    

    void Awake()
    {
        updateView();
    }

    private void FixedUpdate()
    {
        updateView();
    }

    public void toggleView() {
        isActive = !isActive;
        updateView();
    }
    private void updateView() {
        container = transform.Find("Container");
        container.gameObject.SetActive(isActive);
        for (int i = 0; i < container.childCount; i++) container.GetChild(i).gameObject.SetActive(isActive);
    }
    private void Start()
    {
        entityCharacterScript = Entity.GetComponent<PlayerController>();
        if (Entity.GetComponent<PlayerController>() == null)       // Debugs if failed to load Character component
            Debug.LogWarning("HealthBarScript could not find the Character Script belonging to the Entity");

        createItems();

    }

    private void destroyItems()
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
    private void createItems()
    {
        if (amount > items.Length || amount < 0) return;
        int[] itemlist = new int[amount];
        setRarityPrice();
        for (int i = 0; i < amount; i++) itemlist[i] = i;


     

        var rng = new System.Random();
        var keys = items.Select(e => rng.NextDouble()).ToArray();

        Array.Sort(keys, items);

        foreach (int index in itemlist) { CreateItemSelection(items[index].item, items[index].rarity, index); }
    }

    private void CreateItemSelection(GameObject item, Rarity rarity, int positionIndex)
    {
        Transform itemTransform = Instantiate(itemTemplatePrefab.transform, container);
        RectTransform itemRectTransform = itemTransform.GetComponent<RectTransform>();
        Button itemButton = itemTransform.GetComponent<Button>();
        float itemHeight = 60f;
        if (addRandomRange) numRange = range;
        else numRange = 0;
        int price = getRarityPrice(rarity) + new System.Random().Next(0, numRange);
        itemRectTransform.anchoredPosition = new Vector2(0, -itemHeight * (positionIndex-2));
        
        itemTransform.Find("ItemName").GetComponent<TextMeshProUGUI>().SetText(item.name);
        itemTransform.Find("ItemPrice").GetComponent<TextMeshProUGUI>().SetText(price.ToString());
        itemTransform.Find("ItemImage").GetComponent<Image>().sprite = item.GetComponent<SpriteRenderer>().sprite;

        KeyValuePair<Color, Color> colorKvp = getRarityColor(rarity);
        itemTransform.Find("Background").GetComponent<Image>().color = colorKvp.Key;
        itemTransform.Find("ItemName").GetComponent<TextMeshProUGUI>().color = colorKvp.Value;


        itemButton.onClick.AddListener(() => { // do whatever when buying items
            if (entityCharacterScript.getCoinAmount() >= price) {
                entityCharacterScript.tryRemoveCoinAmount(price);
                SoundManager.Instance.PlaySound(buySound, Entity.transform, isSpatial:false);
                print(Entity);
                Instantiate(item, Entity.transform.position, Quaternion.identity);
            }
        });

    }
    private void setRarityPrice()
    {
        price.Add(Rarity.Legendary, legendary);
        price.Add(Rarity.Rare, rare);
        price.Add(Rarity.Common, common);
    }
    private int getRarityPrice(Rarity innKey)
    {
        price.TryGetValue(innKey, out var returnPrice);
        return returnPrice;
    }
    private KeyValuePair<Color, Color> getRarityColor(Rarity rarity)
    {

        Color BGC, TC; // Background color and text color
        switch (rarity)
        {
            default:
                if (enableCustomColor){ BGC = commonBox; TC = commonText;}
                else {
                    BGC = new Color(229f / 255f, 245f / 255f, 229f / 255f);
                    TC = Color.white;
                }
     
                break;
            case Rarity.Rare:
                if (enableCustomColor){ BGC = rareBox; TC = rareText;}
                else
                {
                    BGC = new Color(80f / 256f, 129f / 256f, 171f / 256f);
                    TC = Color.blue;
                }
                break;
            case Rarity.Legendary:
                if (enableCustomColor){ BGC = legendaryBox; TC = legendaryText; }
                else
                {
                    BGC = new Color(171f / 256f, 80f / 256f, 91f / 256f);
                    TC = Color.red;
                }
                break;

        }
        return new KeyValuePair<Color, Color>(BGC, TC);
    }


}
