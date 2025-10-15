using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;
public class BMBeatManager : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] BMBeatmapLoader beatmapLoader;
    [SerializeField] BMBeatController beatController;
    [SerializeField] BMScoreManager scoreManager;
    [SerializeField] BMUIManager UIManager;
    AudioSource metronomeAudioSource;
    AudioSource musicAudioSource;
    [SerializeField] SpriteRenderer noteSprite;
    //Transform noteSpriteTransform;
    [SerializeField] AudioClip beatMetronomeAudioClip; // Audio clip untuk beat metronome
    [SerializeField] AudioClip defaultNormalTrackMusicA; // Audio clip untuk normal track
    [SerializeField] AudioClip defaultNormalTrackMusicB; // Audio clip untuk normal track
    [SerializeField] AudioClip defaultTransitionTrackMusicA; // Audio clip untuk transisi track
    [SerializeField] AudioClip defaultTransitionTrackMusicB; // Audio clip untuk transisi track

    [Header("Gameobjects Container")]
    [SerializeField] Transform hitPointsContainer;
    [SerializeField] Transform beatPointsContainer;
    [SerializeField] Transform timeSignaturePointsContainer;

    [Header("Variables")]
    [SerializeField] bool isGameRunning = false; // Set awal game tidak berjalan

    [SerializeField] Transform startPosition; // Posisi awal game object 
    [SerializeField] Transform endPosition; // Posisi akhir game object
    [SerializeField] Transform[] beatPoints; // Posisi semua beat yang ada di dalam 1 measure
    [SerializeField] Transform[] timeSignaturePoints; // Posisi semua time signature points
    [SerializeField] GameObject beatPrefab; // Prefab untuk beat
    [SerializeField] GameObject timeSignaturePrefab; // Prefab untuk time signature points
    [SerializeField] Transform beatCounter; // Beat Counter
    [SerializeField] GameObject hitPointPrefab; // Prefab untuk hit points
    [SerializeField] bool enableMetronome = true; // Toggle untuk metronome

    float defaultBpm = 60f; // BPM default untuk interval 1 detik
    [SerializeField] float currentBPM = 120f; // BPM yang di-set oleh player atau di load dari file config
    [SerializeField] float nextBPM = 120f; // BPM untuk next measure
    float beatCounterSpeed; // Kecepatan Beat Counter
    float pitchScale = 1; // Skala untuk mengubah pitch audio berdasarkan BPM
    int beatsPerMeasure = 32; // Jumlah ketukan dalam satu measure. SAAT INI HANYA SUPPORT 32 beat per measure.
    [SerializeField] int timeSignature = 4; // Time signature
    [SerializeField] BMBeatmapDataSO.TimeSignatureType timeSignatureType; // Digunakan untuk menentukan beatsPerMeasure
    [SerializeField] int numberTimeSignaturePointPerSequence = 4; // Time signature
    [SerializeField] int shownBeatPointsPerMeasure = 4; // jumlah beat yang ditampilkan, juga digunakan untuk melakukan perhitungan play Metronome.
    [SerializeField] float beatTime = 0f; // Waktu yang disimpan untuk beat



    [SerializeField] List<Transform> hitPoints;

    //private AudioSource metronomeAudioSource;
    private int previousMetronomeIndex = -1;

    [SerializeField] int currentBeat = 1; // Beat saat ini
    [SerializeField] int elapsedBeat = 0; // Total beat yang telah berlalu
    [SerializeField] int elapsedMeasure = 0; // Total measure yang telah berlalu
    //[SerializeField] private int elapsedMeasureLengthInOneTrack = 1;

    // TRACK
    [SerializeField] bool transitionMode = false;
    [SerializeField] bool listenTrackMode = false;
    [SerializeField] bool captureTrackMode = false;


    public List<TrackSequence> trackMeasureSequence; // Sequence yang berisi data hitpoint,bpm,note sprite, dll.


    // BM Game State
    void Awake()
    {
        metronomeAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource = gameObject.AddComponent<AudioSource>();
        UIManager.gameplayMusicAudioSource = musicAudioSource; // Assign musicAudioSource ke UIManager untuk keperluan pause
        musicAudioSource.volume = 0.7f;
        //noteSpriteTransform = noteSprite.transform;
    }

    void Start()
    {
        LoadBeatmapInformation();
        UpdatePitchScale();
        InitializeBeatPoints();
        InitializeTimeSignaturePoints();
        InitializeHitPoints(); // intansiasi hitpoints
        InitializeTrackMeasureSequence(); // kontruksi track sequence
        StartCoroutine(BeatCounterRoutine());
        DisableHitPoints(); // for first time only
        //UpdateHitPoints(elapsedMeasure); // update pola hitpoints untuk pertama kali (setelah sequence selesai dikonsturksi)
        //UpdateHitPointsTest(); // test only
    }

    private void LoadBeatmapInformation()
    {
        defaultBpm = beatmapLoader.bpm; // Set value ke default bpm milik beatmap saat index pertama sequence di pakai/putar.
        currentBPM = beatmapLoader.bpm; // Set value ke default bpm milik beatmap saat index pertama sequence di pakai/putar.
        nextBPM = beatmapLoader.trackPatternData[0].trackBPM; // Set value ke default bpm milik beatmap saat index pertama sequence di pakai/putar.
        timeSignature = beatmapLoader.timeSignature;
        numberTimeSignaturePointPerSequence = beatmapLoader.numberTimeSignaturePointPerSequence;
        defaultNormalTrackMusicA = beatmapLoader.defaultNormalMusicA;
        defaultNormalTrackMusicB = beatmapLoader.defaultNormalMusicB;
        defaultTransitionTrackMusicA = beatmapLoader.defaultTransitionMusicA;
        defaultTransitionTrackMusicB = beatmapLoader.defaultTransitionMusicB;
        shownBeatPointsPerMeasure = beatmapLoader.shownBeatPointsPerMeasure;
        timeSignatureType = beatmapLoader.timeSignatureType;

        switch (timeSignatureType)
        {
            case BMBeatmapDataSO.TimeSignatureType.FourPerFour:
                beatsPerMeasure = 32;
                break;
            case BMBeatmapDataSO.TimeSignatureType.SixPerEight:
                beatsPerMeasure = 24;
                break;
            case BMBeatmapDataSO.TimeSignatureType.TwoPerFour:
                beatsPerMeasure = 16;
                break;
            case BMBeatmapDataSO.TimeSignatureType.ThreePerFour:
                beatsPerMeasure = 24;
                break;
        }
        //beatsPerMeasure = 32; // Set paksa nilai ke 32 (karena hanya support 32 note)
    }

    private void DisableHitPoints()
    {
        foreach (var point in hitPoints)
        {
            point.gameObject.SetActive(false);
        }
    }

    private void InitializeTrackMeasureSequence()
    {
        trackMeasureSequence = new List<TrackSequence>();

        int transitionTrackLength = 1; // Selalu bernilai 1. Jangan diganti.
        int listenTrackLength = beatmapLoader.listenTrackLength;
        int captureTrackLength = beatmapLoader.captureTrackLength;
        int lastTransitionTrackBPM = beatmapLoader.bpm; // Setiap index sequence memiliki bpm yang sama dengan bpm yang dimiliki transition sequence pada index sebelumnya
        BMBeatmapTrackData[] trackPatternData = beatmapLoader.trackPatternData;

        for (int i = 0; i < trackPatternData.Length; i++)
        {
            if (trackPatternData[i].isTransitionTrack == false)
            {
                // Listen Track
                for (int j = 0; j < listenTrackLength; j++)
                {
                    AddNewTrackMeasureSequence(false, false, true, false, lastTransitionTrackBPM, trackPatternData[i].optionalNormalMusicA, trackPatternData[i].optionalNormalMusicB, trackPatternData[i].optionalTransitionMusicA, trackPatternData[i].optionalTransitionMusicB, trackPatternData[i].noteSprite, trackPatternData[i].trackPattern);

                }

                // Capture Track
                for (int j = 0; j < captureTrackLength; j++)
                {
                    AddNewTrackMeasureSequence(false, false, false, true, lastTransitionTrackBPM, trackPatternData[i].optionalNormalMusicA, trackPatternData[i].optionalNormalMusicB, trackPatternData[i].optionalTransitionMusicA, trackPatternData[i].optionalTransitionMusicB, trackPatternData[i].noteSprite, trackPatternData[i].trackPattern);
                }
            }
            else
            {
                // Transition A
                for (int j = 0; j < transitionTrackLength; j++)
                {
                    AddNewTrackMeasureSequence(true, false, false, false, trackPatternData[i].trackBPM, trackPatternData[i].optionalNormalMusicA, trackPatternData[i].optionalNormalMusicB, trackPatternData[i].optionalTransitionMusicA, trackPatternData[i].optionalTransitionMusicB, trackPatternData[i].noteSprite, trackPatternData[i].trackPattern);
                    lastTransitionTrackBPM = trackPatternData[i].trackBPM;

                    Debug.Log("Transition A index: " + i);
                    Debug.Log(trackPatternData[i].optionalTransitionMusicA);
                    Debug.Log(trackPatternData[i].optionalTransitionMusicB);
                }

                // Transition B
                for (int j = 0; j < transitionTrackLength; j++)
                {
                    AddNewTrackMeasureSequence(false, true, false, false, trackPatternData[i].trackBPM, trackPatternData[i].optionalNormalMusicA, trackPatternData[i].optionalNormalMusicB, trackPatternData[i].optionalTransitionMusicA, trackPatternData[i].optionalTransitionMusicB, trackPatternData[i].noteSprite, trackPatternData[i].trackPattern);
                    //lastTransitionTrackBPM = trackPatternData[i].trackBPM;

                    Debug.Log("Transition B index: " + i);
                    Debug.Log(trackPatternData[i].optionalTransitionMusicA);
                    Debug.Log(trackPatternData[i].optionalTransitionMusicB);

                }
            }
        }
    }

    public void AddNewTrackMeasureSequence(bool isTransitionA, bool isTransitionB, bool isListen, bool isCapture, float bpm, AudioClip normalMusicA, AudioClip normalMusicB, AudioClip transitionMusicA, AudioClip transitionMusicB, Sprite noteSprite, bool[] trackPattern)
    {
        // Buat objek baru TrackSequence
        TrackSequence newTrack = new TrackSequence();
        newTrack.isTransitionTrackA = isTransitionA;
        newTrack.isTransitionTrackB = isTransitionB;
        newTrack.isListenTrack = isListen;
        newTrack.isCaptureTrack = isCapture;
        

        if (bpm == 0)
        {
            newTrack.trackBPM = defaultBpm;
        }
        else
        {
            newTrack.trackBPM = bpm;
        }

        // Listen & Capture

        if (newTrack.isListenTrack  && !newTrack.isCaptureTrack )
        {
            newTrack.normalMusic = normalMusicA; // jika track tidak mempunyai music  sendiri
        }
        else if (!newTrack.isListenTrack && newTrack.isCaptureTrack)
        {
            newTrack.normalMusic = normalMusicB; // jika track tidak mempunyai music  sendiri
        }

        //if (newTrack.normalMusic != null)
        //{

        //    if (newTrack.isListenTrack == true && newTrack.isCaptureTrack == false)
        //    {
        //        newTrack.normalMusic = normalMusicA; // jika track tidak mempunyai music  sendiri
        //    }
        //    else if (newTrack.isListenTrack == false && newTrack.isCaptureTrack == true)
        //    {
        //        newTrack.normalMusic = normalMusicB; // jika track tidak mempunyai music  sendiri
        //    }
        //}
        //else
        //{
        //    if (newTrack.isListenTrack == true && newTrack.isCaptureTrack == false)
        //    {
        //        newTrack.normalMusic = defaultNormalTrackMusicA; // jika track tidak mempunyai music  sendiri
        //    }
        //    else if (newTrack.isListenTrack == false && newTrack.isCaptureTrack == true)
        //    {
        //        newTrack.normalMusic = defaultNormalTrackMusicB; // jika track tidak mempunyai music  sendiri
        //    }

        //    //newTrack.normalMusic = defaultNormalTrackMusic; // jika track tidak mempunyai music  sendiri
        //}


        // Transition A & B

        // Transition A & B tanpa pengecekan null
        if (newTrack.isTransitionTrackA && !newTrack.isTransitionTrackB)
        {
            newTrack.transitionMusic = transitionMusicA; // Menggunakan music A jika track transition adalah A
        }
        else if (!newTrack.isTransitionTrackA && newTrack.isTransitionTrackB)
        {
            newTrack.transitionMusic = transitionMusicB; // Menggunakan music B jika track transition adalah B
        }


        //if (newTrack.transitionMusic != null)
        //{
        //    if (newTrack.isTransitionTrackA == true && newTrack.isTransitionTrackB == false)
        //    {
        //        newTrack.transitionMusic = transitionMusicA; // jika track mempunyai music sendiri

        //        Debug.Log("!= Null A: " + newTrack.transitionMusic);
        //    }
        //    else if (newTrack.isTransitionTrackA == false && newTrack.isTransitionTrackB == true)
        //    {
        //        newTrack.transitionMusic = transitionMusicB; // jika track mempunyai music sendiri

        //        Debug.Log("!= Null: B" + newTrack.transitionMusic);


        //    }

        //    //newTrack.transitionMusic = transitionMusic; // jika track mempunyai music sendiri
        //}
        //else if (newTrack.transitionMusic == null)
        //{
        //    if (newTrack.isTransitionTrackA == true && newTrack.isTransitionTrackB == false)
        //    {
        //        newTrack.transitionMusic = defaultTransitionTrackMusicA; // jika track mempunyai music sendiri

        //        Debug.Log("Null A: " + newTrack.transitionMusic);



        //    }
        //    else if (newTrack.isTransitionTrackA == false && newTrack.isTransitionTrackB == true)
        //    {
        //        newTrack.transitionMusic = defaultTransitionTrackMusicB; // jika track mempunyai music sendiri

        //        Debug.Log("Null B: " + newTrack.transitionMusic);



        //    }
        //    //newTrack.transitionMusic = defaultTransitionTrackMusicA; // jika track tidak mempunyai music  sendiri
        //}

        newTrack.noteSprite = noteSprite;
        newTrack.trackPattern = trackPattern;

        // Tambahkan ke List
        trackMeasureSequence.Add(newTrack);
    }

    void UpdatePitchScale()
    {
        pitchScale = currentBPM / defaultBpm;
        // Update pitch audio di sini jika diperlukan
        musicAudioSource.pitch = pitchScale;
        Debug.Log("Pitch Scale di ubah ke : " + pitchScale);
    }

    void InitializeBeatPoints()
    {
        beatPoints = new Transform[beatsPerMeasure];

        for (int i = 0; i < beatsPerMeasure; i++)
        {
            GameObject beat = Instantiate(beatPrefab);
            float t = (float)i / beatsPerMeasure;
            beat.transform.position = Vector3.Lerp(startPosition.position, endPosition.position, t);
            beatPoints[i] = beat.transform;
            beat.transform.SetParent(beatPointsContainer);
        }
    }

    void InitializeTimeSignaturePoints()
    {
        timeSignaturePoints = new Transform[numberTimeSignaturePointPerSequence + 1];
        //timeSignaturePoints = new Transform[timeSignature + 1];
        Debug.Log("NUMBER OF TIME SIGNATURE POINTS : " + numberTimeSignaturePointPerSequence);
        for (int i = 0; i <= numberTimeSignaturePointPerSequence; i++)
        {
            GameObject timeSigPoint = Instantiate(timeSignaturePrefab);
            //float t = (float)i / timeSignature;
            float t = (float)i / numberTimeSignaturePointPerSequence;
            timeSigPoint.transform.position = Vector3.Lerp(startPosition.position, endPosition.position, t);
            timeSignaturePoints[i] = timeSigPoint.transform;
            timeSigPoint.transform.SetParent(timeSignaturePointsContainer);

            Debug.Log("INSTANTIATED TIME SIGNATURE POINT");
        }
    }

    void InitializeHitPoints()
    {
        InstansiateHitPoints();
        //UpdateHitPoints();
    }

    private void InstansiateHitPoints()
    {
        for (int i = 0; i < beatPoints.Length; i++)
        {
            GameObject hitPoint = Instantiate(hitPointPrefab);
            //hitPoint.name = "HitPoint";
            hitPoint.transform.position = beatPoints[i].position;
            hitPoints.Add(hitPoint.transform);
            hitPoint.transform.SetParent(hitPointsContainer);
        }
    }

    void UpdateHitPoints(int measure)
    {
        Debug.Log("===========================================");
        Debug.Log("[UpdateHitPoints] Measure saat UPDATE HIT POINT : " + measure); // debug
        int TARGET_SEQUENCE_INDEX = measure - 1;
        Debug.Log("[UpdateHitPoints] Target INDEX saat UPDATE HIT POINT : " + TARGET_SEQUENCE_INDEX); // debug


        // ===================================================== CHECK INDEX SEQUENCE ===============================================
        if (trackMeasureSequence.Count == TARGET_SEQUENCE_INDEX) // jika index trackMeasureSequence selanjutnya == total sequance, maka coroutine akan di stop
        {
            Debug.Log("[UpdateHitPoints] [ELSE] totalSeq : " + (trackMeasureSequence.Count) + ", currSeqIndex : " + TARGET_SEQUENCE_INDEX); // debug
            Debug.Log("=== LAST SEQUENCE'S INDEX REACHED, FINISHED PLAYING ==="); // debug
            isGameRunning = false;
            StopAllCoroutines();
            metronomeAudioSource.Stop();
            musicAudioSource.Stop();


            scoreManager.ShowGameResult(); // Display game result UI dan save data
            return;
        }
        else
        {
            Debug.Log("[UpdateHitPoints] [TRUE] totalSeq : " + (trackMeasureSequence.Count) + ", currSeqIndex : " + TARGET_SEQUENCE_INDEX); // debug

            // Merubah nextBPM
            if (trackMeasureSequence.Count == TARGET_SEQUENCE_INDEX + 1) // jika "TARGET_SEQUENCE_INDEX + 1" melebihi index
            {
                nextBPM = currentBPM;
            }
            else
            {
                nextBPM = trackMeasureSequence[TARGET_SEQUENCE_INDEX + 1].trackBPM;
            }
        }


        // ===================================================== PLAY SEQUENCE =====================================================
        Debug.Log("totalSeq : " + trackMeasureSequence.Count + ", currSeqIndex : " + TARGET_SEQUENCE_INDEX); // debug
        if (measure >= 1)
        {
            if (trackMeasureSequence[TARGET_SEQUENCE_INDEX].isListenTrack == true  && (trackMeasureSequence[TARGET_SEQUENCE_INDEX].isTransitionTrackA == false || trackMeasureSequence[TARGET_SEQUENCE_INDEX].isTransitionTrackB == false))
            {
                //beatController.isAllowToPress = false;
                beatController.SetState(BMBeatController.GameState.ListenMode);

                for (int i = 0; i < trackMeasureSequence[TARGET_SEQUENCE_INDEX].trackPattern.Length; i++)
                {
                    if (trackMeasureSequence[TARGET_SEQUENCE_INDEX].trackPattern[i] == true)
                    {
                        hitPoints[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        hitPoints[i].gameObject.SetActive(false);
                    }
                }

                PlayNormalSequenceMusic(TARGET_SEQUENCE_INDEX);
                DisplayTrackNoteSprite(TARGET_SEQUENCE_INDEX);

            }
            else if (trackMeasureSequence[TARGET_SEQUENCE_INDEX].isCaptureTrack == true && (trackMeasureSequence[TARGET_SEQUENCE_INDEX].isTransitionTrackA == false || trackMeasureSequence[TARGET_SEQUENCE_INDEX].isTransitionTrackB == false))
            {
                //beatController.isAllowToPress = true;
                beatController.SetState(BMBeatController.GameState.CaptureMode);

                PlayNormalSequenceMusic(TARGET_SEQUENCE_INDEX);
                DisplayTrackNoteSprite(TARGET_SEQUENCE_INDEX);
            }
            else if ((trackMeasureSequence[TARGET_SEQUENCE_INDEX].isTransitionTrackA == true || trackMeasureSequence[TARGET_SEQUENCE_INDEX].isTransitionTrackB == true))
            {
                //beatController.isAllowToPress = false;
                beatController.SetState(BMBeatController.GameState.TransitionMode);
                beatController.PlayAvatarAnimation_Idle();

                foreach (var point in hitPoints)
                {
                    point.gameObject.SetActive(false);
                }

                PlayTransitionSequenceMusic(TARGET_SEQUENCE_INDEX);
                DisplayTrackNoteSprite(TARGET_SEQUENCE_INDEX);

            }
        }
    }

    private void DisplayTrackNoteSprite(int TARGET_SEQUENCE_INDEX)
    {
        if (trackMeasureSequence[TARGET_SEQUENCE_INDEX].noteSprite != noteSprite)
        {
            noteSprite.sprite = trackMeasureSequence[TARGET_SEQUENCE_INDEX].noteSprite;
        }
    }

    private void PlayNormalSequenceMusic(int TARGET_SEQUENCE_INDEX)
    {
        if (trackMeasureSequence[TARGET_SEQUENCE_INDEX].normalMusic != musicAudioSource.clip)
        {
            musicAudioSource.clip = trackMeasureSequence[TARGET_SEQUENCE_INDEX].normalMusic;
        }

        musicAudioSource.Play();
    }

    private void PlayTransitionSequenceMusic(int TARGET_SEQUENCE_INDEX)
    {
        if (trackMeasureSequence[TARGET_SEQUENCE_INDEX].transitionMusic != musicAudioSource.clip)
        {
            musicAudioSource.clip = trackMeasureSequence[TARGET_SEQUENCE_INDEX].transitionMusic;
        }

        musicAudioSource.Play();
    }

    void UpdateHitPointsTest()
    {
        Debug.Log("BEAT saat UPDATE HIT POINT TEST : " + elapsedBeat);
        Debug.Log("Measure saat UPDATE HIT POINT TEST : " + elapsedMeasure);

        int TARGET_SEQUENCE_INDEX = 3;

        if ((trackMeasureSequence[TARGET_SEQUENCE_INDEX].isTransitionTrackA == false || trackMeasureSequence[TARGET_SEQUENCE_INDEX].isTransitionTrackB == false))
        {
            for (int i = 0; i < trackMeasureSequence[TARGET_SEQUENCE_INDEX].trackPattern.Length; i++)
            {
                if (trackMeasureSequence[TARGET_SEQUENCE_INDEX].trackPattern[i] == true)
                {
                    hitPoints[i].gameObject.SetActive(true);
                }
                else
                {
                    hitPoints[i].gameObject.SetActive(false);
                }
            }
        }
        else if ((trackMeasureSequence[TARGET_SEQUENCE_INDEX].isTransitionTrackA == true || trackMeasureSequence[TARGET_SEQUENCE_INDEX].isTransitionTrackB == true))
        {
            foreach (var point in hitPoints)
            {
                point.gameObject.SetActive(false);
            }
        }

        //for (int i = 0; i < trackMeasureSequence[0].trackPattern.Length; i++)
        //{
        //    if (trackMeasureSequence[0].trackPattern[i] == true && trackMeasureSequence[0].trackPattern.Length > 0)
        //    {
        //        hitPoints[i].gameObject.SetActive(true);
        //    }
        //    else if (trackMeasureSequence[0].trackPattern[i] == false && trackMeasureSequence[0].trackPattern.Length <= 0)
        //    {
        //        foreach (var point in hitPoints)
        //        {
        //            point.gameObject.SetActive(false);
        //        }
        //        //hitPoints[i].gameObject.SetActive(false);
        //    }
        //}
    }

    IEnumerator BeatCounterRoutine()
    {
        while (true)
        {
            while (isGameRunning)
            {
                beatCounterSpeed = Vector3.Distance(startPosition.position, endPosition.position) / (60f / currentBPM * timeSignature);
                yield return MoveBeatCounter();
                ResetBeatCounters();
                beatCounter.position = startPosition.position;
                beatTime = 0f;

                if (currentBPM != nextBPM)
                {
                    currentBPM = nextBPM;
                    UpdatePitchScale();
                    //UpdateHitPoints(elapsedMeasure);
                }

                UpdateHitPoints(elapsedMeasure); // Update hitpoint (active/nonactive gameobject)

            }
            yield return null; // wait until the game is unpaused
        }
    }

    IEnumerator MoveBeatCounter()
    {
        float duration = (60f / currentBPM) * timeSignature;
        float startTime = Time.time;
        Vector3 initialPosition = startPosition.position;
        Vector3 finalPosition = endPosition.position;

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            beatCounter.position = Vector3.Lerp(initialPosition, finalPosition, t);

            // Check if beatCounter is passing a timeSignaturePoint
            int metronomeIndex = GetCurrentMetronomeIndex();
            //int  = trackMeasureSequence.Count;

            if (metronomeIndex != previousMetronomeIndex)
            {
                UpdateBeatCounters(metronomeIndex);
                if (elapsedMeasure <= 1)
                {
                    PlayBeatMetronome(metronomeIndex);
                }
                previousMetronomeIndex = metronomeIndex;
            }

            yield return null;
        }
        //UpdateHitPoints(elapsedMeasure); // Update hitpoint (active/nonactive gameobject)

        beatCounter.position = endPosition.position;

    }

    int GetCurrentMetronomeIndex()
    {
        for (int i = 0; i < shownBeatPointsPerMeasure; i++)
        {
            if (Vector3.Distance(beatCounter.position, timeSignaturePoints[i].position) < 0.1f)
            {
                return i;
            }
        }
        return -1;
    }

    void PlayBeatMetronome(int index)
    {
        if (enableMetronome && index >= 0 && index < shownBeatPointsPerMeasure)
        {
            metronomeAudioSource.clip = beatMetronomeAudioClip;
            metronomeAudioSource.pitch = (index == 0) ? 1.5f : 1f;
            metronomeAudioSource.PlayOneShot(metronomeAudioSource.clip);
        }

        Debug.Log("Elapsed Metronome Index" + index);
    }

    void UpdateBeatCounters(int metronomeIndex)
    {
        if (metronomeIndex >= 0 && metronomeIndex < timeSignature)
        {
            currentBeat = metronomeIndex + 1;
            elapsedBeat++;
            if (currentBeat == 1)
            {
                elapsedMeasure++;
            }
        }
        //Debug.Log("beat updated at : " + beatTime);

    }

    void ResetBeatCounters()
    {
        currentBeat = 1;
    }

    void UpdateBeatPoints()
    {
        for (int i = 0; i < beatsPerMeasure; i++)
        {
            float t = (float)i / beatsPerMeasure;
            beatPoints[i].position = Vector3.Lerp(startPosition.position, endPosition.position, t);
        }
    }

    void UpdateTimeSignaturePoints()
    {
        for (int i = 0; i <= numberTimeSignaturePointPerSequence; i++)
        {
            float t = (float)i / numberTimeSignaturePointPerSequence;
            timeSignaturePoints[i].position = Vector3.Lerp(startPosition.position, endPosition.position, t);
        }

        //for (int i = 0; i <= timeSignature; i++)
        //{
        //    float t = (float)i / timeSignature;
        //    timeSignaturePoints[i].position = Vector3.Lerp(startPosition.position, endPosition.position, t);
        //}
    }

    void Update()
    {
        if (isGameRunning)
        {
            //UpdateBeatPoints();
            //UpdateTimeSignaturePoints();
            beatTime += Time.deltaTime;
        }
    }

    //public void StartGame()
    //{
    //    isGameRunning = true;
    //}

    //public void PauseGame()
    //{
    //    isGameRunning = false;
    //}
}

[System.Serializable]
public class BMHitPoint
{
    public Transform hitObject;
    public float hitTime;
}

[System.Serializable]
public class TrackSequence
{
    public bool isTransitionTrackA;
    public bool isTransitionTrackB;
    public bool isListenTrack;
    public bool isCaptureTrack;
    public float trackBPM;
    public AudioClip normalMusic;
    public AudioClip transitionMusic;
    public Sprite noteSprite;
    public bool[] trackPattern;
}