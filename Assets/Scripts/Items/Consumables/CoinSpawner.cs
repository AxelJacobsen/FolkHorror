using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A coin spawner.
/// </summary>
public class CoinSpawner : MonoBehaviour
{
    // Public vars
    [Header("Settings")]
    public int          AmountOfCoins = 10;
    public float        SpawnForSeconds = 2.5f;
    public GameObject   CoinPrefab;

    // Private vars
    private int   amtSpawned = 0;
    private float coinSpawnTimer = 0f;

    void FixedUpdate()
    {
        // Increment coin spawn timer
        coinSpawnTimer += Time.deltaTime;

        // Check if we can spawn a coin this frame and how many
        float secPerAmt = SpawnForSeconds / AmountOfCoins;
        if (coinSpawnTimer >= secPerAmt)
        {
            while (coinSpawnTimer > secPerAmt && amtSpawned < AmountOfCoins)
            {
                GameObject coin = Instantiate(CoinPrefab, transform.position, Quaternion.identity);
                Coin coinScript = coin.GetComponent<Coin>();
                if (coinScript != null)
                {
                    Vector3 throwDir = new Vector3(
                        Random.Range(-1f, 1f),
                        Random.Range(1f, 3f),
                        Random.Range(-1f, 1f)
                    ).normalized;
                    coinScript.Throw(throwDir * 10f);
                } 

                amtSpawned++;
                coinSpawnTimer -= secPerAmt;
            }
        }

        // If all coins have been spawned, destroy spawner.
        if (amtSpawned >= AmountOfCoins)
        {
            Destroy(gameObject);
        }
    } 
}
