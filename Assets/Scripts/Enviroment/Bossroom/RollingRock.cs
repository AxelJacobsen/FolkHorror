using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for rocks rolling in a direction, hitting <c>characters<c/>.
/// </summary>
public class RollingRock : MonoBehaviour
{
    public Vector3          Velocity;
    public float            Lifetime = 5f;
    public float            Damage = 50f;
    public float            Knockback = 50f;
    public float            Rockfragments = 10f;
    public EffectEmitter    DustEffectEmitter;
    public List<GameObject> IgnoreList = new();

    [Header("Sounds")]
    [SerializeField] private AudioClip RollingSound;
    [SerializeField] private AudioClip CollisionSound;

    private GameObject      spriteObject,
                            rockfragment;
    private Rigidbody       rb;
    private float           livedFor = 0f;
    private AudioSource     rollingSoundSource;
    private EffectEmitter   myDustEffectEmitter;

    // Start is called before the first frame update
    void Start()
    {
        // Fetch components
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError(gameObject.name + "(RollingRock) could not find its rigidbody!");

        // Find required child objects
        foreach (Transform child in gameObject.transform)
        {
            if      (child.gameObject.name == "sprite")         spriteObject = child.gameObject;
            else if (child.gameObject.name == "rockfragment")   rockfragment  = child.gameObject;
        }

        // Play rolling sound
        rollingSoundSource = SoundManager.Instance.PlaySound(RollingSound, gameObject.transform);

        // Dynamically create a copy of the given scriptableObject (dustemitter) which belongs to ONLY US
        myDustEffectEmitter = Instantiate(DustEffectEmitter);
        myDustEffectEmitter._Hitbox = GetComponent<BoxCollider>();
        myDustEffectEmitter._Active = true;
        myDustEffectEmitter.SizeFunc = f => f*0.7f + 0.3f;
        myDustEffectEmitter.AlphaFunc = f => 0.5f - f*0.5f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (myDustEffectEmitter != null) myDustEffectEmitter.Emit(Time.deltaTime);
        livedFor += Time.deltaTime;
        
        // Check if the stone has lived its life to the fullest
        if (livedFor > Lifetime)
        {
            Destroy(gameObject);
            return;
        }

        // Calculate the stone's angular velocity...
        float   radius = spriteObject.transform.localScale.magnitude / 125f,
                circumference = 2f * Mathf.PI * radius,
                direction = Velocity.x / Mathf.Abs(Velocity.x),
                angularVelocity = Velocity.magnitude / circumference * direction;

        // ...and apply it.
        Quaternion ang = Quaternion.AngleAxis(angularVelocity * livedFor, new Vector3(0, 1, -1).normalized ) * Quaternion.Euler(45, 0, 0);
        spriteObject.transform.rotation = ang;

        // Set its velocity, but only x/z.
        Vector3 v = rb.velocity;
        v.x = Velocity.x; v.z = Velocity.z;
        rb.velocity = v;
    }

    /// <summary>
    /// When the rock collides with something, blow up.
    /// </summary>
    void OnCollisionEnter(Collision hit)
    {
        // If the hit collider belongs to a hitbox, use its parent instead.
        GameObject hitObj = hit.gameObject;
        if (hitObj.tag == "Hitbox") { hitObj = hitObj.transform.parent.gameObject; }

        // Check if the hit object is in the ignore list, if so, return
        foreach(GameObject ignoreObj in IgnoreList)
            if (ignoreObj == hitObj) return;

        // Check if it hit a character. If so, damage them.
        Character characterHit = hitObj.GetComponent<Character>();
        if (characterHit != null) { CollideWithCharacter(characterHit); }
    }

    /// <summary>
    /// Collide with a character.
    /// </summary>
    /// <param name="character">The character.</param>
    void CollideWithCharacter(Character character)
    {
        character.Hurt(gameObject, Damage);
        character.Knockback( Velocity.normalized * Knockback );

        SelfDestruct();
    }

    void SelfDestruct()
    {
        // Screenshake and explosion sound
        CameraFollow cameraFollowScript = Camera.main.GetComponent<CameraFollow>();
        cameraFollowScript.Screenshake(0.5f, 0.5f);
        SoundManager.Instance.PlaySound(CollisionSound, gameObject.transform);
        if (rollingSoundSource != null) rollingSoundSource.Stop();

        for (int i=0; i < Rockfragments; i++)
        {
            Vector3 vel = Velocity*1.5f + new Vector3( Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f) ).normalized * Velocity.magnitude * 0.5f;

            GameObject rockfragmentSpawned = Instantiate(rockfragment, gameObject.transform.position, Quaternion.identity);
            rockfragmentSpawned.SetActive(true);
            rockfragmentSpawned.GetComponent<Rigidbody>().velocity = vel;
            rockfragmentSpawned.GetComponent<Rockfragment>()._StartAng = Random.Range(0f, 360f);
            rockfragmentSpawned.GetComponent<Rockfragment>()._SpinSpeed = Random.Range(-600f, 600f);

            float sizeMult = Random.Range(0.4f, 0.7f);
            rockfragmentSpawned.transform.localScale *= sizeMult;
        }

        Destroy(gameObject);
    }
}
