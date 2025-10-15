using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CMBeatmapDataPreset", menuName = "ScriptableObjects/Chord Master/CM Beatmap Preset")]
public class CMBeatmapDataSO : ScriptableObject
{
    [Header("References")]
    public TextAsset beatmapJSONFile;
    public AudioClip musicAudioFile;

    [Header("Config")]
    public bool displayOnLevelSelector = true; // Jika true, akan tampil di Row Level Selector

    [Header("Optional Config")]
    public string displayTitle; // Opsional, jika di ini akan menampilkan judul ini di Row Level Selector
    public string displayDifficultyCaption; // Opsional, jika di ini akan menampilkan judul ini di Row Level Selector
}