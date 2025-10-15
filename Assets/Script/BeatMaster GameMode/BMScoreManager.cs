using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BMScoreManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject performanceReportPanel;

    [SerializeField] BMBeatManager bmBeatManager;
    [SerializeField] BMBeatController bmBeatController;

    [Header("Player Performance Panel")]
    [SerializeField] TextMeshProUGUI scoreValText;
    [SerializeField] TextMeshProUGUI scoreRatingValText;
    [SerializeField] TextMeshProUGUI detailSuksesValText;
    [SerializeField] TextMeshProUGUI detailGagalValText;
    [SerializeField] TextMeshProUGUI detailAkurasiValText;
    [SerializeField] Image ratingImageComponent;
    [SerializeField] Image ratingImageBgComponent;
    [SerializeField] float tweenDuration = 3f;
    [SerializeField] List<Sprite> ratingImages;
    [SerializeField] List<Sprite> ratingBgImages;
    [SerializeField] float imageDuration = 1f;
    int ratingImageIndex = 0;
    bool imageLoop = true;

    public List<BMPlayerPerformanceScore> playerPerformanceList;

    [Header("Back to menu")]

    [SerializeField] Button backButton;


    private void Start()
    {
        Debug.Log("SAVE DIRECTORY : " + Application.persistentDataPath.ToString());

        backButton.onClick.AddListener(() => GameManager.Instance.sceneController.SwitchSceneMainMenu());

    }

    public void ShowGameResult()
    {
        GameManager.Instance.SetGameStateToPaused();
        CalculateScore(bmBeatController.score, GetHitNoteData(bmBeatManager.trackMeasureSequence));
        SaveGameData(playerPerformanceList);
        ShowGameplayResult();
    }

    private void SaveGameData(List<BMPlayerPerformanceScore> playerPerformanceList)
    {
        Debug.Log("LOKASI SAVE :" + Application.persistentDataPath);

        BMPlayerPerformanceScore data = playerPerformanceList[0];

        if (!GameManager.Instance.isMusicalJourneyMode) // Jika Game Mode ini dimulai dari "Beat Master Level Selector"
        {
            GameManager.Instance.saveManager.SaveScoreBM(data.performanceScore, data.noteHitSuccess, data.noteHitMiss, data.noteHitSuccessRate, data.performanceRating, GetFormattedDate());
            Debug.Log("BERHASIL DI SAVE");
        }
        else // Jika Game Mode ini dimulai dari "Musical Journey Level Selector"
        {
            float noteHitSuccessRate = data.noteHitSuccessRate;
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

    }



    // Score Capture
    // Simpan score untuk di save (write) dan ditampilkan di "game result"
    public void CalculateScore(int performanceScore, int noteHitTotal)
    {
        string performanceRating = "C"; //Default rating
        float score = ((float)performanceScore / (float)noteHitTotal) * 100f;
        float scorePercentage = 0f;

        switch (performanceScore)
        {
            case > 0:
                scorePercentage = performanceScore / (float)noteHitTotal * 100f;
                break;
            default:
                scorePercentage = 0;
                break;
        }

        // Rating/Accuracy
        switch (scorePercentage)
        {
            case >= 90f:
                performanceRating = "S";
                break;
            case >= 80f:
                performanceRating = "A";
                break;
            case >= 60f:
                performanceRating = "B";
                break;
            default:
                performanceRating = "C";
                break;
        }

        List<BMPlayerPerformanceScore> playerPerformanceList = new List<BMPlayerPerformanceScore>();
        BMPlayerPerformanceScore data = new BMPlayerPerformanceScore();

        // Kirim data ke UI untuk ditampilkan
        data.performanceScore = performanceScore;
        data.noteHitSuccess = performanceScore;
        data.noteHitMiss = noteHitTotal - performanceScore;
        data.noteHitSuccessRate = scorePercentage;
        data.performanceRating = performanceRating;
        playerPerformanceList.Add(data);

        this.playerPerformanceList = playerPerformanceList;
    }


    private int GetHitNoteData(List<TrackSequence> trackMeasureSequence)
    {
        


        // Dapatkan total hitPoint yang bernilai "true"
        int totalHitNote = 0; // Jumlah note "gameobject.active == true

        foreach (var seq in trackMeasureSequence)
        {
            if ((seq.isTransitionTrackA == false || seq.isTransitionTrackB == false) && seq.trackPattern != null)
            {
                foreach (var item in seq.trackPattern)
                {
                    if (item == true)
                    {
                        totalHitNote++;
                    }
                }
            }
        }

        Debug.Log("Total HitNote : " + totalHitNote);

        // Perhitungan dilakukan dengan menghitung semua hitNote (Baik mode Listen & mode Capture), lalu nilai totalHitNote dibagi 2, karena hanya perlu menghitung total dari hitNote pada mode Capture
        return totalHitNote / 2; 
    }

    public void ShowGameplayResult()
    {
        //this.playerPerformanceList = playerPerformanceList;

        ApplyplayerPerformanceValue();
        ratingImageBgComponent.sprite = null;
        performanceReportPanel.SetActive(true);
    }

    private void ApplyplayerPerformanceValue()
    {
        LoopImages();
        StartCoroutine(WaitAndExecute());
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
    IEnumerator WaitAndExecute()
    {
        // Animasi Score dari 0 ke performanceScore
        AnimatePerformanceValue(0, playerPerformanceList[0].performanceScore, scoreValText, Ease.InCubic, tweenDuration);
        yield return new WaitForSeconds(0.5f);
        AnimatePerformanceValue(0, playerPerformanceList[0].noteHitSuccess, detailSuksesValText, Ease.InCubic, tweenDuration);
        yield return new WaitForSeconds(0.5f);
        AnimatePerformanceValue(0, playerPerformanceList[0].noteHitMiss, detailGagalValText, Ease.InCubic, tweenDuration);
        yield return new WaitForSeconds(0.5f);
        AnimatePerformanceInPercentFloatVal(0, playerPerformanceList[0].noteHitSuccessRate, detailAkurasiValText, Ease.InCubic, tweenDuration);
        yield return new WaitForSeconds(3f);
        imageLoop = false;
        ApplyRatingImage();
    }

    private void SetDelay(float v)
    {
        DOVirtual.DelayedCall(v, () =>
        {
            // Logika yang ingin dijalankan setelah 5 detik
            Debug.Log("5 detik telah berlalu!");
        });
    }

    private void ApplyRatingImage()
    {
        scoreRatingValText.text = playerPerformanceList[0].performanceRating; // rating pada text

        switch (playerPerformanceList[0].performanceRating) // rating logo
        {
            case "S":
                ratingImageComponent.sprite = ratingImages[0];
                ratingImageBgComponent.sprite = ratingBgImages[0];
                break;
            case "A":
                ratingImageComponent.sprite = ratingImages[1];
                ratingImageBgComponent.sprite = ratingBgImages[1];
                break;
            case "B":
                ratingImageComponent.sprite = ratingImages[2];
                ratingImageBgComponent.sprite = ratingBgImages[2];
                break;
            default:
                ratingImageComponent.sprite = ratingImages[3];
                ratingImageBgComponent.sprite = ratingBgImages[3];
                break;
        }
    }

    void LoopImages()
    {
        // Hentikan loop jika ImageLoop bernilai false
        if (!imageLoop) return;

        // Set gambar saat ini
        ratingImageComponent.sprite = ratingImages[ratingImageIndex];

        // Update indeks gambar untuk gambar berikutnya
        ratingImageIndex = (ratingImageIndex + 1) % ratingImages.Count;

        // Tunggu durasi tertentu sebelum mengubah gambar berikutnya
        DOVirtual.DelayedCall(imageDuration, () => {
            // Panggil ulang LoopImages untuk mengubah gambar berikutnya
            LoopImages();
        });
    }
    private void AnimatePerformanceValue(int startValue, int endValue, TextMeshProUGUI textObj, Ease tweenEase, float tweenDuration) // untuk nilai int
    {
        DOTween.To(() => startValue, x =>
        {
            startValue = x;
            textObj.text = startValue.ToString(); // Update teks dengan nilai yang baru
        }, endValue, tweenDuration).SetEase(tweenEase);
    }

    private void AnimatePerformanceInPercentFloatVal(float startValue, float endValue, TextMeshProUGUI textObj, Ease tweenEase, float tweenDuration) // untuk persentase dengan nilai float
    {
        DOTween.To(() => startValue, x =>
        {
            startValue = x;
            textObj.text = startValue.ToString("F2") + "%"; // Update teks dengan nilai yang baru
        }, endValue, tweenDuration)
        .SetEase(tweenEase);
    }

    [System.Serializable]
    public class BMPlayerPerformanceScore
    {
        public int performanceScore;
        public int noteHitSuccess;
        public int noteHitMiss;
        public float noteHitSuccessRate;
        public string performanceRating;

    }
}
