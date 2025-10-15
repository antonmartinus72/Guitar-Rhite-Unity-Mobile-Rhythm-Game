using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    public AudioClip demoFile_audioMusicVolume; // file audio untuk musik
    public AudioClip demoFile_audioSfxVolume;   // file audio untuk SFX

    public AudioSource audioSource_MusicVolume;
    public AudioSource audioSource_SfxVolume;

    private void Awake()
    {
        audioSource_MusicVolume = gameObject.AddComponent<AudioSource>();
        audioSource_SfxVolume = gameObject.AddComponent<AudioSource>();
        audioSource_MusicVolume.clip = demoFile_audioMusicVolume;
        audioSource_SfxVolume.clip = demoFile_audioSfxVolume;

        Debug.Log("Load Time [VolumeManager] : " + Time.time);
    }
}
