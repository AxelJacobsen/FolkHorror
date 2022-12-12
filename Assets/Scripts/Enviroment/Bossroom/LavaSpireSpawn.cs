using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for a lava spire spawning from the ground, dealing damage.
/// </summary>
public class LavaSpireSpawn : MonoBehaviour
{
    // Public vars
    [Header("Stats")]
    public float    Damage    = 50f;
    public float    Knockup   = 25f;
    public float    Stun      = 1f;
    public float    Radius    = 5f;
    public float    Delay     = 2f;
    public float    Lifetime  = 5f;
    public float    RiseBy    = 2.7f;
    public float    RockfragmentRate = 150f;

    [Header("Sounds")]
    [SerializeField] private AudioClip RumblingSound;
    [SerializeField] private AudioClip EruptionSound;

    [Header("Effects")]
    public EffectData OnHitEffect;

    [Header("Set by scripts")]
    public string _TargetTag = "Player";

    // Private vars
    private float       livedFor = 0f,
                        rockfragmentsToSpawn = 0f;
    private GameObject  lavaspire,
                        hotfloor,
                        rockfragment;
    private Vector3     lavaspireSpawnPos;
    private EffectData  myOnHitEffect;

    // Counters, etc
    private bool rumblingStarted = false;
    private bool screenShakeStarted = false;
    private AudioSource rumblingSoundSource;

    void Start()
    {
        // Find required child objects
        foreach (Transform child in gameObject.transform)
        {
            if      (child.gameObject.name == "lavaspire")      lavaspire = child.gameObject;
            else if (child.gameObject.name == "hotfloor")       hotfloor  = child.gameObject;
            else if (child.gameObject.name == "rockfragment")   rockfragment  = child.gameObject;
        }

        // Set vars
        lavaspireSpawnPos = lavaspire.transform.position;

        // Clone scriptables
        myOnHitEffect = Instantiate(OnHitEffect);
    }

    void FixedUpdate()
    {
        // Count up livedfor
        livedFor += Time.deltaTime;

        // First phase, make hot floor increase in size and opacity
        if (livedFor < Delay)
        {
            if (!rumblingStarted) 
            {
                rumblingStarted = true;
                rumblingSoundSource = SoundManager.Instance.PlaySound(RumblingSound, gameObject.transform);
            }

            float t = livedFor / Delay;
            t = t*t*(3 - 2*t); //sstep3
            hotfloor.transform.localScale = new Vector3(t, t, t) * 0.7f;
            if (rumblingSoundSource != null) rumblingSoundSource.volume = t;
            if (screenShakeStarted) screenShakeStarted = false; //reset
        } 
        // Second phase, lava spire rises from the ground
        else if (livedFor < Delay + Lifetime)
        {
            float   timeToRise = 0.1f,
                    t = Mathf.Min( (livedFor-Delay) / timeToRise, 1f);
            t = t*t*(3 - 2*t); //sstep3

            lavaspire.transform.position = lavaspireSpawnPos + new Vector3(0, RiseBy * t, 0);

            // Spawn rock fragment(s) and shake screen
            if (livedFor < Delay + timeToRise)
            {
                // Screenshake and explosion sound
                if (!screenShakeStarted)
                {
                    CameraFollow cameraFollowScript = Camera.main.GetComponent<CameraFollow>();
                    cameraFollowScript.Screenshake(timeToRise, 1f);
                    screenShakeStarted = true;
                    SoundManager.Instance.PlaySound(EruptionSound, gameObject.transform);
                    rumblingSoundSource.Stop();

                    // Damage close enemies
                    GameObject[] possibleTargets = GameObject.FindGameObjectsWithTag(_TargetTag);
                    foreach (GameObject target in possibleTargets)
                    {
                        // Ignore anything else than characters
                        Character targetCharacterScript = target.GetComponent<Character>();
                        if (targetCharacterScript == null) return;

                        if (Vector3.Distance(target.transform.position, transform.position) < Radius)
                        {
                            targetCharacterScript.Hurt(gameObject, Damage);
                            Rigidbody targetRB = target.GetComponent<Rigidbody>();
                            if (targetRB != null) targetRB.velocity += new Vector3(0,Knockup,0);
                            targetCharacterScript.ApplyEffect(myOnHitEffect, gameObject);
                        }
                    }
                }

                rockfragmentsToSpawn += RockfragmentRate * Time.deltaTime;
                while (rockfragmentsToSpawn >= 1f)
                {
                    rockfragmentsToSpawn -= 1f;
                    Vector3     vel = new Vector3( Random.Range(-1f, 1f), Random.Range(0.5f, 2f), Random.Range(-1f, 1f) ).normalized * 15f;

                    GameObject rockfragmentSpawned = Instantiate(rockfragment, gameObject.transform.position, Quaternion.identity);
                    rockfragmentSpawned.SetActive(true);
                    rockfragmentSpawned.GetComponent<Rigidbody>().velocity = vel;
                    rockfragmentSpawned.GetComponent<Rockfragment>()._StartAng = Random.Range(0f, 360f);
                    rockfragmentSpawned.GetComponent<Rockfragment>()._SpinSpeed = Random.Range(-600f, 600f);

                    float sizeMult = Random.Range(0.5f, 1f);
                    rockfragmentSpawned.transform.localScale *= sizeMult;
                }
            }
        }
        // lastly, destroy self.
        else
        {
            DestroySelf();
        }
    }

    /**
     *  Destroys this lava spire.
     */
    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
