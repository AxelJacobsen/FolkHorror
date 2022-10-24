using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    // Public vars
    [Header("Set by scripts")]
    public float    _LifeTime,
                    _SizeMultiplier,
                    _Wiggle,
                    _WiggleSpeed,
                    _WiggleOffset;
    public Vector3  _Vel;

    // Private vars
    private SpriteRenderer sr;
    private float   livedFor = 0f;
    private Vector3 baseSize,
                    basePos;
    private Color   baseColor;

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

        // Update size and color
        transform.localScale = baseSize * _SizeMultiplier * (1f - (livedFor / _LifeTime));
        Color col = baseColor;
        col.a = baseColor.a * (1f - (livedFor / _LifeTime));
        sr.color = col;
    }
}
