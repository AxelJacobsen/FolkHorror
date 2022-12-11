
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    public bool debug;


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
        bool active = false;
        if (debug) {
            active = debug;
        }
        container = transform.Find("Container");
        container.gameObject.SetActive(active);
    }

    private void Start()
    {
        entityCharacterScript = Entity.GetComponent<PlayerController>();
        if (Entity.GetComponent<PlayerController>() == null)       // Debugs if failed to load Character component
            Debug.LogWarning("HealthBarScript could not find the Character Script belonging to the Entity");

        createItems(2);

    }

    private void destroyItems()
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
    private void createItems(int amount)
    {
        if (amount > items.Length || amount < 0) return;
        setRarityPrice();
        for (int i = 0; i < items.Length; i++)
        {
            CreateItemSelection(items[i].item, items[i].rarity, i);
        }
    }

    private void CreateItemSelection(GameObject item, Rarity rarity, int positionIndex)
    {
        Transform itemTransform = Instantiate(itemTemplatePrefab.transform, container);
        RectTransform itemRectTransform = itemTransform.GetComponent<RectTransform>();
        Button itemButton = itemTransform.GetComponent<Button>();
        float itemHeight = 60f;
        int price = getRarityPrice(rarity);
        itemRectTransform.anchoredPosition = new Vector2(0, -itemHeight * positionIndex);
        
        itemTransform.Find("ItemName").GetComponent<TextMeshProUGUI>().SetText(item.name);
        itemTransform.Find("ItemPrice").GetComponent<TextMeshProUGUI>().SetText(price.ToString());
        itemTransform.Find("ItemImage").GetComponent<Image>().sprite = item.GetComponent<SpriteRenderer>().sprite;

        KeyValuePair<Color, Color> colorKvp = getRarityColor(rarity);
        itemTransform.Find("Background").GetComponent<Image>().color = colorKvp.Key;
        itemTransform.Find("ItemName").GetComponent<TextMeshProUGUI>().color = colorKvp.Value;


        itemButton.onClick.AddListener(() => {
            if (entityCharacterScript.getCoinAmount() >= price) {
                entityCharacterScript.tryRemoveCoinAmount(price);
                SoundManager.Instance.PlaySound(buySound, isSpatial:false);
            }
        });

    }
    private void setRarityPrice()
    {
        if (addRandomRange) numRange = range;
        else numRange = 0;
        price.Add(Rarity.Legendary, legendary + new System.Random().Next(0, numRange));
        price.Add(Rarity.Rare, rare + new System.Random().Next(0, numRange));
        price.Add(Rarity.Common, common + new System.Random().Next(0, numRange));
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
