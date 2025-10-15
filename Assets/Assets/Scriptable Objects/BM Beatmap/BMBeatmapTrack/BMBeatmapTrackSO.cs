using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "BMBeatmapDataTrack", menuName = "ScriptableObjects/BM Beatmap Track")]
public class BMBeatmapTrackSO : ScriptableObject
{
    //[Header("Optional Custom Audio")]
    //public AudioClip customMusicNormalAudioFile; // If null play audio in beatmap
    //public AudioClip customMusicTransitionAudioFile; // If null play audio in beatmap

    [Header("Note Image File")] // Music sheet's note
    public Sprite noteSprite;

    [Header("Values")]
    public bool isTransitionTrack = false; // true = transition track, false = normal track
    //public int newBPM = 0; // 0 = ignore change BPM
    //public int maxBeat = 32;
    

    [Header("Tracks Pattern")]
    public bool[] beatTrackPattern = new bool[32]; // default is 32 (max supported beat per measure)

}
