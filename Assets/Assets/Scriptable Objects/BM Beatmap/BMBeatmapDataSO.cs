using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BMBeatmapDataPreset", menuName = "ScriptableObjects/BM Beatmap Preset")]
public class BMBeatmapDataSO : ScriptableObject
{
    [Header("References")]
    public bool showInLevelSelector = false;
    public AudioClip musicNormalAudioFileA;
    public AudioClip musicNormalAudioFileB;
    public AudioClip musicTransitionAudioFileA;
    public AudioClip musicTransitionAudioFileB;

    [Header("Values")]
    public string title;
    public int musicBpm;
    public enum TimeSignatureType
    {
        TwoPerFour,
        ThreePerFour,
        FourPerFour,
        SixPerEight
    }

    public TimeSignatureType timeSignatureType = TimeSignatureType.FourPerFour;

    public int beatsPerTrack;
    public int shownBeatPointsPerMeasure;

    //public int trackLenghtInMeasure = 2; // Contoh : jika nilai ini = 2, maka track akan melakukan 2 putaran measure dalam 1 track.
    public int listenTrackMeasureLength = 1; // Nilai untuk menentukan jumlah measure pada saat mode "listen" / "mendengarkan beat yang akan ditiru oleh player pada mode capture". HANYA BERLAKU UNTUK NORMAL TRACK.
    public int captureTrackMeasureLength = 1; // Nilai untuk menentukan jumlah measure pada saat mode "capture" / "menangkap beat setelah mode listen di lalui". HANYA BERLAKU UNTUK NORMAL TRACK.

    [Header("Tracks")]
    public BMBeatmapTrackList[] trackList; // 2 measure = 1 track. 1 measure untuk play beat track dan 1 measure untuk catch beat track.
}

[System.Serializable]
public class BMBeatmapTrackList
{
    public BMBeatmapTrackSO track;
    public int trackBPM;
    public AudioClip customMusicNormalAudioFileA;
    public AudioClip customMusicNormalAudioFileB;
    public AudioClip customMusicTransitionAudioFileA;
    public AudioClip customMusicTransitionAudioFileB;
}