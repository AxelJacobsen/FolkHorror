using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public enum Audio 
    { 
        Effects,
        Music,
        Master
    }
    const String MUSIC_MIXER = "MusicVolume";
    [SerializeField] private AudioSource m_musicSource, m_effectsSource;
    [SerializeField] private AudioMixer m_mixer;

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
    /// Play an AudioClip from the SoundManager
    /// </summary>
    /// <param name="clip">the clip to be played</param>
    ///  <param name="type">Audio source, default is set to Effects</param> 
    public void PlaySound(AudioClip clip, Audio type = Audio.Effects) {

        if (getSoundSource(type) == null) getSoundSource(Audio.Effects).PlayOneShot(clip); // sets master to effect if trying to play from master
        getSoundSource(type).PlayOneShot(clip);
    
    }


    /// <summary>
    /// Changes the volume of the different sources of volume (Music, Effects, Master)
    /// default: Master
    /// </summary>
    /// <param name="val">Volume value</param>
    /// <param name="type">Audio source, default is set to Master</param>
    public void ChangeVolume(float val, Audio type = Audio.Master) {
        if (getSoundSource(type) == null) { AudioListener.volume = val; return; }
        getSoundSource(type).volume = val;
    }

    /// <summary>
    /// Toggles the mute of an audio type
    /// </summary>
    /// <param name="type">Audio source, default is set to Master</param>
    public void ToggleAudio(Audio type = Audio.Master) {
        if (type != Audio.Master) { getSoundSource(type).mute  = !getSoundSource(type).mute; return; }
        if (m_effectsSource.mute.Equals(m_musicSource.mute))
        { ToggleAudio(Audio.Music); ToggleAudio(Audio.Effects); }
        else
            (m_effectsSource.mute, m_musicSource.mute) = (!m_effectsSource.mute, !m_musicSource.mute);
    }

    /// <summary>
    /// Stops activity in specified Audio Source
    /// </summary>
    /// <param name="type">Audio source, default is set to Master</param>
    public void InterruptAudioSource(Audio type = Audio.Master)
    {
        if (type != Audio.Master) { getSoundSource(type).Stop(); return; }
        InterruptAudioSource(Audio.Effects);
        InterruptAudioSource(Audio.Music);
    }


    public IEnumerator InterruptAfter(float wait, Audio type = Audio.Effects ) {
        yield return new WaitForSeconds(wait);
        InterruptAudioSource(type);
    }

    /// <summary>
    /// Gets the corresponding Audio source for the Audio type
    /// </summary>
    /// <param name="type">Sound manager's Audio type holder (master, effects, music)</param>
    /// <returns>The slider which corresponds to the Audio type</returns>
    private AudioSource getSoundSource(Audio type) =>
        type switch
        {
            Audio.Music => m_musicSource,
            Audio.Effects => m_effectsSource,
            _ => null,
        };

}
