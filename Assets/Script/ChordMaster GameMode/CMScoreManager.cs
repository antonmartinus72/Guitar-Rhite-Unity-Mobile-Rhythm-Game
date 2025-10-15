using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;


public class CMScoreManager : MonoBehaviour
{
    [Header("UI in Scene ref")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI scoreTotalPossibleText;
    [SerializeField] TextMeshProUGUI noteTotalText;
    [SerializeField] TextMeshProUGUI noteSuccessText;
    

    [SerializeField] AudioSource musicSource;
    [SerializeField] MusicNoteLoader musicNoteLoader;
    [SerializeField] MusicBeatLoader musicBeatLoader;
    [SerializeField] CMNoteDetectorController noteDetectorLeftObj;
    [SerializeField] CMNoteDetectorController noteDetectorRightObj;
    //[SerializeField] List<GameObject> notesList;
    [SerializeField] CMUIManager UIController;
    //[SerializeField] CMStyleController styleController;

    [Header("Other")]
    private GameObject lastNote;
    [SerializeField] private int totalScorePossible;
    [SerializeField] private int currentScore;
    [SerializeField] private int totalNote;
    [SerializeField] private int currentSuccessNote;

    private int multiplierValue = 10;
    private float BPM;
    private float intervalTime;
    private int subBeatVal;

    [SerializeField] bool enableScoreDebug;

    private bool hasStartedPlaying = false;
    public bool isPaused = false;


    // Start is called before the first frame update
    void Start()
    {
        BPM = GameManager.Instance.beatmapLoader.beatmapCMList[GameManager.Instance.selectedGameLevelIndex].data.BPM;
        subBeatVal = musicBeatLoader.subBeat;

        intervalTime = ((60f / BPM)) / subBeatVal;

        //Hitung Total Note Dari 
        CountLongNoteTotal();

        StartCoroutine(CheckIfAudioFinished());

        Debug.Log("Music Lenght : " + musicSource.clip.length);
        //musicSource.time = 2.8f; // DEBUG


    }

    private void CountLongNoteTotal()
    {
        var notesList = musicNoteLoader.notesList;
        foreach (var note in notesList)
        {
            if (note.GetComponent<CMNoteScript>() != null)
            {
                totalNote++;
            }
        }
        noteTotalText.text = "Total Note : " + totalNote.ToString();
    }

    IEnumerator CheckIfAudioFinished()
    {
        while (true)
        {
            bool isPaused = (GameManager.Instance.CurrentGameState == GameManager.GameState.Paused);

            if (!musicSource.isPlaying && !isPaused && Mathf.Approximately(musicSource.time, musicSource.clip.length))
            {
                Debug.Log("CMSCORE_Audio finished playing.");

                SendPlayerScoreData();
                Debug.Log("CMSCORE_Data dikirim.");
                yield return new WaitForSeconds(3f);
                Debug.Log("CMSCORE_Data selesai dikirim.");

                yield break; // Exit the coroutine
            }

            yield return null; // Wait for the next frame
        }
    }

    void FixedUpdate()
    {
        DebugScore(); //Debug

        // SCORE UPDATER
        // FOR LEFT SCORE
        if (UIController.isLeftNoteKeyMatch && noteDetectorLeftObj.collidedLeftNoteScoreObject != null)
        {
            StartCoroutine(UpdateScore());
            CountTotalSuccessNote();
        }
        else
        {
            noteDetectorLeftObj.collidedLeftNoteScoreObject = null;
            StopCoroutine(UpdateScore());
        }

        // FOR RIGHT NOTE
        if (UIController.isRightNoteKeyMatch && noteDetectorRightObj.collidedRightNoteScoreObject != null)
        {
            StartCoroutine(UpdateScore());
            CountTotalSuccessNote();
        }
        else
        {
            noteDetectorRightObj.collidedRightNoteScoreObject = null;
            StopCoroutine(UpdateScore());
        }

        // FOR LEFT TOTAL SCROE
        if (noteDetectorLeftObj.collidedLeftNoteObject != null)
        {
            StartCoroutine(UpdateTotalScorePossible());
        }
        else
        {
            StopCoroutine(UpdateTotalScorePossible());
        }

        // FOR RIGHT TOTAL SCROE
        if (noteDetectorRightObj.collidedRightNoteObject != null)
        {
            StartCoroutine(UpdateTotalScorePossible());
        }
        else
        {
            StopCoroutine(UpdateTotalScorePossible());
        }
    }

    private void DebugScore()
    {
        if (Input.GetKeyDown(KeyCode.X)) // DEBUG
        {
            currentScore = 78000;
            totalScorePossible = 100000;
            totalNote = 1000;
            currentSuccessNote = 600;
            // Panggil metode ketika tombol "x" ditekan
            musicSource.time = musicSource.clip.length - 5f;
        }
        //Debug.Log("Music Time : " + musicSource.time);
    }

    private void SendPlayerScoreData()
    {
        string performanceRating = "C";
        float noteHitSuccessRate = ((float)currentSuccessNote / (float)totalNote) * 100f;
        float scorePercentage = 0f;
        if (currentScore > 0)
        {
             scorePercentage = ((float)currentScore / (float)totalScorePossible) * 100f;
        }
        else
        {
            scorePercentage = 0;
        }

        // Rating/Accuracy
        if (scorePercentage >= 90f)
        {
            performanceRating = "S";
        }
        else if (scorePercentage >= 80f)
        {
            performanceRating = "A";
        }
        else if (scorePercentage >= 60f)
        {
            performanceRating = "B";
        }
        else
        {
            performanceRating = "C";
        }

        List<CMPlayerPerformanceScore> playerPerformanceList = new List<CMPlayerPerformanceScore>();
        CMPlayerPerformanceScore data = new CMPlayerPerformanceScore();

        // Kirim data ke UI untuk ditampilkan
        data.performanceScore = currentScore;
        data.noteHitSuccess = currentSuccessNote;
        data.noteHitMiss = totalNote - currentSuccessNote;
        data.noteHitSuccessRate = noteHitSuccessRate;
        data.performanceRating = performanceRating;
        playerPerformanceList.Add(data);

        // Save Data
        Debug.Log("LOKASI SAVE :" + Application.persistentDataPath);

        if (!GameManager.Instance.isMusicalJourneyMode) // Jika Game Mode ini dimulai dari "Beat Master Level Selector"
        {
            GameManager.Instance.saveManager.SaveScoreCM(data.performanceScore, data.noteHitSuccess, data.noteHitMiss, data.noteHitSuccessRate, data.performanceRating, GetFormattedDate());
        }
        else
        {
            //float noteHitSuccessRate = data.noteHitSuccessRate;
            int starNumber = 0;
            switch (noteHitSuccessRate)
            {
                case >= 70f:
                    starNumber = 3;
                    break;
                case >= 50f:
                    starNumber = 2;
                    break;
                case >= 0f:
                    starNumber = 1;
                    break;
                default:
                    starNumber = 1;
                    break;
            }

            GameManager.Instance.saveManager.SaveLevelDataMJ(GameManager.Instance.selectedMusicalJourneyLevelIndex, starNumber, GameManager.Instance.selectedMusicalJourneyLevelIndexPage);
        }


        

        //GameManager.Instance.PauseGame(); // Caused score animation stopped because the animation is using dotween
        UIController.ShowGameplayResult(playerPerformanceList);

    }

    string GetFormattedDate()
    {
        // Mendapatkan tanggal saat ini
        System.DateTime now = System.DateTime.Now;

        // Menggunakan StringBuilder untuk membentuk string tanggal
        StringBuilder sb = new StringBuilder();

        // Menambahkan hari (dua digit)
        sb.Append(now.Day.ToString("00"));

        // Menambahkan bulan (dua digit)
        sb.Append(now.Month.ToString("00"));

        // Menambahkan tahun (dua digit)
        sb.Append((now.Year % 100).ToString("00"));

        // Mengembalikan string yang telah diformat
        return sb.ToString();
    }
    private void CountTotalSuccessNote()
    {
        if (lastNote != noteDetectorLeftObj.collidedLeftNoteScoreObject)
        {
            currentSuccessNote++;
        }

        lastNote = noteDetectorLeftObj.collidedLeftNoteScoreObject;
        noteSuccessText.text = "Succes Note : " + currentSuccessNote.ToString();
    }

    private IEnumerator UpdateScore() {
        currentScore += 1 * multiplierValue;
        scoreText.text = currentScore.ToString();
        yield return new WaitForSeconds(intervalTime);
    }
    private IEnumerator UpdateTotalScorePossible() {
        totalScorePossible += 1 * multiplierValue;
        scoreTotalPossibleText.text = "TScore : " + totalScorePossible.ToString();
        yield return new WaitForSeconds(intervalTime);
    }
}
