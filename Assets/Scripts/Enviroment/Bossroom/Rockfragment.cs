using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for rock fragment effects.
/// </summary>
public class Rockfragment : MonoBehaviour
{
    // Public vars
    [Header("Stats")]
    public float    LifeTime = 5f;

    [Header("Set by scripts")]
    public float    _SpinSpeed;
    public float    _StartAng;

    // Private vars
    private float   livedFor = 0f;

    void FixedUpdate()
    {
        // Count up livedfor
        livedFor += Time.deltaTime;

        // First phase, make hot floor increase in size and opacity
        if (livedFor < LifeTime)
        {
            Quaternion ang = Quaternion.AngleAxis( _StartAng + _SpinSpeed * livedFor, new Vector3(0, 1, -1).normalized ) * Quaternion.Euler(45, 0, 0);
            transform.rotation = ang;
        } else 
        {
            Destroy(gameObject);
        }
    }
}
