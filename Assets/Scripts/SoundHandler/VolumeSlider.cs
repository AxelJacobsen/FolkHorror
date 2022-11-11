using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] public Slider m_MasterSlider, m_EffectsSlider, m_MusicSlider;

    // Update is called once per frame
    void Update()
    {
        setAudioSliderListener( // sets listeners on sliders for sound and master control
            new SoundManager.Audio[]
            {
                SoundManager.Audio.Master,
                SoundManager.Audio.Effects,
                SoundManager.Audio.Music,
  
            }
        );
    }
    /// <summary>
    /// Gets the corresponding slider for the Audio type
    /// </summary>
    /// <param name="type">Sound manager's Audio type holder (master, effects, music)</param>
    /// <returns>The slider which corresponds to the Audio type</returns>
    private Slider getAudioSlider(SoundManager.Audio type) => 
        type switch {
            SoundManager.Audio.Music => m_MusicSlider,
            SoundManager.Audio.Effects => m_EffectsSlider,
            _ => m_MasterSlider,
        };
    /// <summary>
    /// Sets the audio to be the same as the slider value, then connects the sliders next values on change to the SoundManager
    /// </summary>
    /// <param name="types">the different types of audio to append listener and default value to</param>
    private void setAudioSliderListener(SoundManager.Audio[] types) {
        if (types.Length.Equals(0)) return;

        foreach (SoundManager.Audio type in types) {
            SoundManager.Instance.ChangeVolume(getAudioSlider(type).value, type);
            getAudioSlider(type).onValueChanged.AddListener(val => SoundManager.Instance.ChangeVolume(val, type));
        }
    }

}
