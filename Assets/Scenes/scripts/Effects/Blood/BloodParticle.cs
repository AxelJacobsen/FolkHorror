using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for fire particles.
/// </summary>
public class BloodParticle : MonoBehaviour
{
    // Public vars
    [Header("Set by scripts")]
    public float    _LifeTime = 2f,
                    _Size;

    // Private vars
    private float   livedFor = 0f;

    void Start()
    {

    }

    void FixedUpdate()
    {
        // Count up lifetime and destroy if it exceeds set lifetime.
        livedFor += Time.deltaTime;
        if (livedFor >= _LifeTime) {
            Destroy(gameObject);
        }

        // Update posize
        transform.localScale = Vector3.one * _Size * (1f - (livedFor / _LifeTime));
        Color col = gameObject.GetComponent<SpriteRenderer>().color;
        col.a = 1;//Mathf.Sqrt((1f - (livedFor / _LifeTime)));
        gameObject.GetComponent<SpriteRenderer>().color = col;
    }
}
