using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for particles.
/// </summary>
public class Particle : MonoBehaviour
{
    // Public vars
    [Header("Set by scripts")]
    public float    _LifeTime,
                    _SizeMultiplier,
                    _Wiggle,
                    _WiggleSpeed,
                    _WiggleOffset,
                    _SpinRadius,
                    _SpinSpeed,
                    _SpinOffset;
    public Vector3  _Vel;

    // Private vars
    private SpriteRenderer sr;
    private float   livedFor = 0f;
    private Vector3 baseSize,
                    basePos;
    private Color   baseColor;
    private Vector3 spinShift;

    // Start is called before the first frame update
    void Start()
    {
        // Fetch components
        sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr == null) Debug.Log("Particle could not find its spriterenderer!");

        basePos = transform.position;
        baseSize = transform.localScale;
        baseColor = sr.color;
        sr.color = new Color(0,0,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        // Count up lifetime and destroy if it exceeds set lifetime.
        livedFor += Time.deltaTime;
        if (livedFor >= _LifeTime) {
            Destroy(gameObject);
        }

        // Update position
        Vector3 wiggleShift = new Vector3(_Wiggle * Mathf.Sin((transform.position.y - basePos.y) * _WiggleSpeed + _WiggleOffset), 0, 0);
        transform.position += _Vel * Time.deltaTime + wiggleShift;

        if (spinShift != null) transform.position -= spinShift;
        spinShift = new Vector3(Mathf.Sin(livedFor * _SpinSpeed + _SpinOffset), 0, Mathf.Cos(livedFor * _SpinSpeed + _SpinOffset)) * _SpinRadius;
        transform.position += spinShift;

        // Update size and color
        transform.localScale = baseSize * _SizeMultiplier * (1f - (livedFor / _LifeTime));
        Color col = baseColor;
        col.a = baseColor.a * (1f - (livedFor / _LifeTime));
        sr.color = col;
    }
}
