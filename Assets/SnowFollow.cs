using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowFollow : MonoBehaviour
{
    [Header("Player movement prediction")]
    public  Vector3     Offset = new Vector3(0, 10, 17);
    public  float       PredictTime = 1f;

    [Header("Sound")]
    [SerializeField] private AudioClip windSound;

    private GameObject  player;
    private Rigidbody   playerRB;
    private bool        active = false;
    private AudioSource windSource = null;

    // Start is called before the first frame update
    void Start()
    {
        // Fetch components
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) 
        {
            playerRB = player.GetComponent<Rigidbody>();
            if (playerRB != null) active = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!active) return;
        
        if (windSource == null || !windSource.isPlaying)
        {
            windSource = SoundManager.Instance.PlaySound(windSound, player.transform);
            windSource.volume = 0.2f;
        }
        transform.position = player.transform.position + playerRB.velocity * PredictTime + Offset;
    }

    void OnDisable()
    {
        if (windSource != null || windSource.isPlaying)
            windSource.Stop();
    }
}
