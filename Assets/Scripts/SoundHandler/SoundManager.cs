using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

/// <summary>
/// Manages sounds.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public enum Audio 
    { 
        Effects,
        Music,
        Master
    }

    /// <summary>
    /// A simple struct to keep track of a sound source's properties.
    /// </summary>
    private struct SoundSourceProperties {
        public float Volume;
        public bool  Muted;
    }

    const String MUSIC_MIXER = "MusicVolume";
    private Dictionary<Audio, List<AudioSource>>     soundSources    = new Dictionary<Audio, List<AudioSource>>();
    private Dictionary<Audio, SoundSourceProperties> soundProperites = new Dictionary<Audio, SoundSourceProperties>();
    private bool masterMuted = false;

    public static SoundManager Instance;
    void Awake() 
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    /// <summary>
    /// Initializes a new audiosource type.
    /// </summary>
    /// <param name="type">The type to initialize.</param>
    private void InitType(Audio type) {
        soundSources.Add(type, new List<AudioSource>());
        soundProperites.Add(type, new SoundSourceProperties{Volume = 0.5f, Muted = false});
    }

    /// <summary>
    /// Play an AudioClip from the SoundManager
    /// </summary>
    /// <param name="clip">the clip to be played</param>
    /// <param name="type">Audio source, default is set to Effects</param> 
    public AudioSource PlaySound(AudioClip clip, Audio type = Audio.Effects) {
        // If key isn't initialized, initialize it with an empty list.
        if (!soundSources.ContainsKey(type)) InitType(type);

        // Create a new audiosource for the clip and set its properties.
        AudioSource newSource = Instantiate(Resources.Load<GameObject>("Sounds/SoundSourcePrefab").GetComponent<AudioSource>());
        newSource.volume      = soundProperites[type].Volume;
        newSource.mute        = soundProperites[type].Muted;

        // Lastly, play the sound and add it to the list.
        //newSource.PlayOneShot(clip);
        newSource.clip = clip;
        newSource.Play();
        soundSources[type].Add(newSource);
        return newSource;
    }


    /// <summary>
    /// Changes the volume of the different sources of volume (Music, Effects, Master)
    /// default: Master
    /// </summary>
    /// <param name="val">Volume value</param>
    /// <param name="type">Audio source, default is set to Master</param>
    public void ChangeVolume(float val, Audio type = Audio.Master) {
        // If the type is master, set audiolistener's volume and return
        if (type == Audio.Master) { AudioListener.volume = val; return; }

        // If the given type isn't initialized, return
        if (!soundSources.ContainsKey(type)) return;

        // Set volume property (TODO: workaround using set/get so we don't have to dupe the prop.)
        SoundSourceProperties tProp = soundProperites[type];
        tProp.Volume = val;
        soundProperites[type] = tProp;

        // Otherwise, iterate all sources within the type and set volume.
        foreach (AudioSource source in soundSources[type]) {
            source.volume = val;
        }
    }

    /// <summary>
    /// Toggles the mute of an audio type
    /// </summary>
    /// <param name="type">Audio source, default is set to Master</param>
    public void ToggleAudio(Audio type = Audio.Master) {
        // If type is master, flip masterMuted and call this function with all other types.
        if (type == Audio.Master) {
            masterMuted = !masterMuted;
            foreach (Audio audioType in soundSources.Keys)
                ToggleAudio(audioType);
            return;
        }

        // If the given type isn't initialized, return
        if (!soundSources.ContainsKey(type)) return;

        // Set muted property (TODO: workaround using set/get so we don't have to dupe the prop.)
        SoundSourceProperties tProp = soundProperites[type];
        tProp.Muted = !tProp.Muted;
        soundProperites[type] = tProp;

        // Otherwise, iterate all sources within the type and mute.
        foreach (AudioSource source in soundSources[type])
            source.mute = !source.mute;
    }

    /// <summary>
    /// Stops activity in specified Audio Source
    /// </summary>
    /// <param name="type">Audio source, default is set to Master</param>
    public void InterruptAudioSource(Audio type = Audio.Master)
    {
        // If type is master, call this function with all other types.
        if (type == Audio.Master) {
            foreach (Audio audioType in soundSources.Keys)
                InterruptAudioSource(audioType);
            return;
        }

        // If the given type isn't initialized, return
        if (!soundSources.ContainsKey(type)) return;

        // Otherwise, iterate all sources within the type and interrupt.
        foreach (AudioSource source in soundSources[type])
            source.Stop();
    }

    /// <summary>
    /// Stops activity in specified Audio Source after a given amount of time.
    /// </summary>
    /// <param name="wait">The amount of time in seconds.</param>
    /// <param name="type">The type to interrupt.</param>
    /// <returns>Yield.</returns>
    public IEnumerator InterruptAfter(float wait, Audio type = Audio.Effects ) {
        yield return new WaitForSeconds(wait);
        InterruptAudioSource(type);
    }

    /// <summary>
    /// Delete audio sources after they're done playing.
    /// </summary>
    void FixedUpdate() {
        // Find sources to remove...
        List<(AudioSource, Audio)> removeSources = new List<(AudioSource, Audio)>();
        foreach (Audio type in soundSources.Keys) {
            foreach (List<AudioSource> sources in soundSources.Values) {
                foreach (AudioSource source in sources)
                    if (!source.isPlaying) removeSources.Add((source, type));
            }
        }

        // ...and remove them.
        foreach ((AudioSource, Audio) removeSource in removeSources) {
            soundSources[removeSource.Item2].Remove(removeSource.Item1);
            Destroy(removeSource.Item1);
        }
    }
}
