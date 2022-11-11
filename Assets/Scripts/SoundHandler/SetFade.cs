using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetFade : MonoBehaviour
{
    [SerializeField] private float duration, targetVolume;
    [SerializeField] private String VolumeHolder;
    [SerializeField] private AudioMixer m_mixer;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeMixerGroup.StartFade(m_mixer, VolumeHolder, duration, targetVolume));
    }

}
