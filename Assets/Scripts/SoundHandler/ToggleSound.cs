using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ToggleSound : MonoBehaviour
{
    [SerializeField] private bool m_toggleMusic, m_toggleEffects;

    public void Toggle() {
        if(m_toggleMusic) SoundManager.Instance.ToggleAudio(SoundManager.Audio.Music);
        if (m_toggleEffects) SoundManager.Instance.ToggleAudio(SoundManager.Audio.Effects);
    }

}
