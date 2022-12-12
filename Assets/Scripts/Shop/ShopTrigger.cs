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
        shopCanvas = GameObject.FindWithTag("Shop");
        shop = shopCanvas.GetComponent<ShopManager>();
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
