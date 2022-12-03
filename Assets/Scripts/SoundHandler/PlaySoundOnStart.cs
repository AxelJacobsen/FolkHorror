using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaySoundOnStart : MonoBehaviour
{
    [SerializeField] private AudioClip []m_clips;
    // Start is called before the first frame update
    void Start()
    {
        foreach (AudioClip clip in m_clips) {
            SoundManager.Instance.PlaySound(clip, gameObject.transform);
        }
        
    }

    void OnDestroy()
    {
        // Simply unparent sound effects rather than destroying them
        foreach (Transform child in transform) 
            if (child.gameObject.tag == "Sound")
                child.parent = null;
    }
}
