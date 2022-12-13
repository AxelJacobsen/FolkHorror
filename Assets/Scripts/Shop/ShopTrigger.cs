using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class <c>DialogueTriggerNew</c> enables dialogue when the user is inside a certain area.
/// </summary>
public class ShopTrigger : MonoBehaviour
{
    private GameObject shopCanvas;
    private ShopManager shop;

    private void Start()
    {
        transform.parent.gameObject.SetActive(false);

        shopCanvas = GameObject.FindWithTag("Shop");
        shop = shopCanvas.GetComponent<ShopManager>();

        PlayerController pCon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if ((pCon.currentStage+1) % 3 == 0) { transform.parent.gameObject.SetActive(true); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent == null || !other.transform.parent.gameObject.CompareTag("Player")) return;
        shop.toggleView();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent == null || !other.transform.parent.gameObject.CompareTag("Player")) return;

        shop.toggleView();
    }
}
