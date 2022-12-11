using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;

public class CoinAmountUpdater : MonoBehaviour
{   // public
    public TextMeshProUGUI coinAmountHolder;    // textholder for coin amount
    public GameObject Entity;                   // the temp holder for Character Script component loading
    // private                                   
    private PlayerController entityCharacterScript;    // Loaded character script holder
    public void SetCoinAmount(int amount) {
        coinAmountHolder.SetText(amount.ToString());
    }
    
    
    private void Start()
    {
        entityCharacterScript = Entity.GetComponent<PlayerController>();
        if (Entity.GetComponent<PlayerController>() == null)       // Debugs if failed to load Character component
            Debug.LogWarning("HealthBarScript could not find the Character Script belonging to the Entity");
        SetCoinAmount(entityCharacterScript.getCoinAmount());
    }

    private void FixedUpdate()
    {
        SetCoinAmount(entityCharacterScript.getCoinAmount());
    }
}
