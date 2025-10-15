using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BMBeatmapLoader : MonoBehaviour
{
    [SerializeField] private BMBeatmapDataSO loadedBeatmap;
    [SerializeField] public AudioClip defaultNormalMusicA;
    [SerializeField] public AudioClip defaultNormalMusicB;
    [SerializeField] public AudioClip defaultTransitionMusicA;
    [SerializeField] public AudioClip defaultTransitionMusicB;
    [SerializeField] public string title;
    [SerializeField] public int timeSignature;
    [SerializeField] public int numberTimeSignaturePointPerSequence;
    [SerializeField] public int bpm;
    [SerializeField] public int shownBeatPointsPerMeasure;
    [SerializeField] public BMBeatmapDataSO.TimeSignatureType timeSignatureType;
    public int listenTrackLength = 1; // Nilai untuk menentukan jumlah measure pada saat mode "listen" / "mendengarkan beat yang akan ditiru oleh player pada mode capture". HANYA BERLAKU UNTUK NORMAL TRACK.
    public int captureTrackLength = 1; // Nilai untuk menentukan jumlah measure pada saat mode "capture" / "menangkap beat setelah mode listen di lalui". HANYA BERLAKU UNTUK NORMAL TRACK.
    public int totalMeasureInOneTrack; // listenTrackLength + captureTrackLength



    public BMBeatmapTrackData[] trackPatternData;

    void Start()
    {
        var beatmapIndex = GameManager.Instance.selectedGameLevelIndex;
        loadedBeatmap = GameManager.Instance.beatmapLoader.beatmapBMSOFiles[beatmapIndex];

        // Inisialisasi array dengan objek-objek data
        LoadBeatmapData();
    }

    private void LoadBeatmapData()
    {
        defaultNormalMusicA = loadedBeatmap.musicNormalAudioFileA;
        defaultNormalMusicB = loadedBeatmap.musicNormalAudioFileB;
        defaultTransitionMusicA = loadedBeatmap.musicTransitionAudioFileA;
        defaultTransitionMusicB = loadedBeatmap.musicTransitionAudioFileB;
        title = loadedBeatmap.title;
        timeSignature = loadedBeatmap.beatsPerTrack;
        numberTimeSignaturePointPerSequence = loadedBeatmap.shownBeatPointsPerMeasure;
        bpm = loadedBeatmap.musicBpm;
        listenTrackLength = loadedBeatmap.listenTrackMeasureLength;
        captureTrackLength = loadedBeatmap.captureTrackMeasureLength;
        totalMeasureInOneTrack = loadedBeatmap.listenTrackMeasureLength + loadedBeatmap.captureTrackMeasureLength;
        shownBeatPointsPerMeasure = loadedBeatmap.shownBeatPointsPerMeasure;
        timeSignatureType = loadedBeatmap.timeSignatureType;

        trackPatternData = new BMBeatmapTrackData[loadedBeatmap.trackList.Length];
        int trackPatternData_INDEX = 0;
        foreach (var track in loadedBeatmap.trackList)
        {
            trackPatternData[trackPatternData_INDEX] = new BMBeatmapTrackData(track.track.isTransitionTrack, track.trackBPM, track.customMusicNormalAudioFileA, track.customMusicNormalAudioFileB, track.customMusicTransitionAudioFileA, track.customMusicTransitionAudioFileB, track.track.noteSprite, track.track.beatTrackPattern);
            trackPatternData_INDEX++;
        }
    }
}


[System.Serializable]
public class BMBeatmapTrackData
{
    public bool isTransitionTrack;
    public int trackBPM;
    public AudioClip optionalNormalMusicA;
    public AudioClip optionalNormalMusicB;
    public AudioClip optionalTransitionMusicA;
    public AudioClip optionalTransitionMusicB;
    public Sprite noteSprite;
    public bool[] trackPattern;
    public BMBeatmapTrackData(bool isTransitionTrack,int trackBPM, AudioClip normalMusicA, AudioClip normalMusicB, AudioClip transitionMusicA, AudioClip transitionMusicB, Sprite noteSprite, bool[] trackPattern)
    {
         this.isTransitionTrack = isTransitionTrack;
         this.trackBPM = trackBPM;
         optionalNormalMusicA = normalMusicA;
         optionalNormalMusicB = normalMusicB;
         optionalTransitionMusicA = transitionMusicA;
         optionalTransitionMusicB = transitionMusicB;
         this.noteSprite = noteSprite;
         this.trackPattern = trackPattern;
    }
}