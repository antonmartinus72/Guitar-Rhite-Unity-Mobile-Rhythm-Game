using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CMUIManager : MonoBehaviour
{
    
    [Header("Script ref")]  
    [SerializeField] MusicNoteLoader musicNoteLoader;
    [SerializeField] CMNoteDetectorController noteDetectorLeftObj;
    [SerializeField] CMNoteDetectorController noteDetectorRightObj;
    [SerializeField] GameObject pausePanelObj;
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] GameObject performanceReportPanel;

    [Header("Objects ref")]
    [SerializeField] GameObject noteblockAccent0;
    [SerializeField] GameObject noteblockAccent1;

    [Header("Key Matching Indicator")]
    public bool isLeftNoteKeyMatch = false;
    public bool isRightNoteKeyMatch = false;

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

    [Header("Collection")]
    public List<CMGamepadData> gamepadLeftData;
    public List<CMGamepadData> gamepadRightData;
    [SerializeField] private List<CMKeySet> keySetListCM;
    private List<noteChord> noteLongSO;
    public List<CMPlayerPerformanceScore> playerPerformanceList;
    // Start is called before the first frame update

    private void Awake()
    {
        noteLongSO = musicNoteLoader.noteLongSO;

        LoadKeySetData();
        SetGamepadKeysData();
    }

    void Start()
    {
        GameManager.Instance.StartGame();

        noteblockAccent0.SetActive(false);
        noteblockAccent1.SetActive(false);

        PauseGameOnStart();



        //// Pause at start
        //GameManager.Instance.ResumeGame();
        //var pause = gameObject.GetComponent<PauseController>();
        //pause.ShowPausePanel();

    }
    private void FixedUpdate()
    {
        if (noteDetectorLeftObj.collidedLeftNoteObject == null)
        {
            UnmatchLeftGamepadKeyOnPointerUp();
        }
        else if(noteDetectorRightObj.collidedRightNoteObject == null)
        {
            UnmatchRightGamepadKeyOnPointerUp();
        }


    }
    private void PauseGameOnStart()
    {
        // Start game at pause
        var pauseComponent = GetComponent<PauseController>();
        pauseComponent.ShowPausePanel(pausePanelObj);

        //pauseComponent.PauseBGM()
    }

    // ================================= GAME CONTROL UI AND GAMESTYLE ==========================================
    private void SetGamepadKeysData() // CURRENTLY ONLY SUPPORT 7 GAMEKEYS OR 7 keySet
    {
        foreach (var keyset in keySetListCM)
        {
            if (keyset.isDefaultKeySet == true)
            {
                if (keyset.keys.Count > 7)
                {
                    Debug.Log("Currently Only Support 7 key in keySet OR 7 keypad button");
                }
                else
                {
                    for (int i = 0; i < keyset.keys.Count; i++)
                    {
                        //Debug.Log("keys in keySet count = " + keyset.keys.Count);
                        var currentKey = keyset.keys[i];
                        gamepadLeftData[i].keyPadObj.GetComponent<CMKeypadPointerHandler>().noteSO = GetScriptableObjectByKeyset(noteLongSO, currentKey);
                        gamepadRightData[i].keyPadObj.GetComponent<CMKeypadPointerHandler>().noteSO = GetScriptableObjectByKeyset(noteLongSO, currentKey);
                    }
                }
            }
        }
    }
    private noteChord GetScriptableObjectByKeyset(List<noteChord> longNoteSO, string chordKey)
    {
        noteChord selectedSO = null;
        foreach (var item in noteLongSO)
        {
            if (chordKey == item.name)
            {
                selectedSO = item;
                break;
            }
        }
        return selectedSO;
    }
    private void LoadKeySetData()
    {
        int selectedGameLevelIndex = GameManager.Instance.selectedGameLevelIndex;
        keySetListCM = GameManager.Instance.beatmapLoader.beatmapCMList[selectedGameLevelIndex].data.keySet;
    }
    public void ActivateNoteBlockStyleLeftAccentPressIndicator()
    {
        noteblockAccent0.SetActive(true);
    }
    public void DectivateNoteBlockStyleLeftAccentPressIndicator()
    {
        noteblockAccent0.SetActive(false);
    }
    public void ActivateNoteBlockRightAccentPressIndicator()
    {
        noteblockAccent1.SetActive(true);
    }
    public void DectivateNoteBlockRightAccentPressIndicator()
    {
        noteblockAccent1.SetActive(false);
    }
    public void SetLeftPadKey(string value, int gamepadBtnIndex)
    {
        gamepadLeftData[gamepadBtnIndex].keyValue = value;
        //Debug.Log("Leftkey[" + gamepadBtnIndex + "] = " + value);
    }
    public void SetRightPadKey(string value, int gamepadBtnIndex)
    {
        gamepadRightData[gamepadBtnIndex].keyValue = value;
        //Debug.Log("Rightkey[" + gamepadBtnIndex + "] = " + value);
    }
    public void MatchLeftGamepadKey(int gamepadBtnIndex)
    {
        string noteDetectorKey = noteDetectorLeftObj.GetCurrentNoteKey();

        if (gamepadLeftData[gamepadBtnIndex].keyValue == noteDetectorKey && noteDetectorLeftObj.collidedLeftNoteObject != null)
        {
            isLeftNoteKeyMatch = true;
            //Debug.Log("Left pad key is match");
        }
        else
        {
            isLeftNoteKeyMatch = false;
            //Debug.Log("Left pad key is not match");
        }
    }
    public void MatchRightGamepadKey(int gamepadBtnIndex)
    {
        string noteDetectorKey = noteDetectorRightObj.GetCurrentNoteKey();

        if (gamepadRightData[gamepadBtnIndex].keyValue == noteDetectorKey && noteDetectorRightObj.collidedRightNoteObject != null)
        {
            isRightNoteKeyMatch = true;
        }
        else
        {
            isRightNoteKeyMatch = false;
        }

        //CheckisNoteKeyMatchBool(); // For Debug Only, can be deletef
    }
    public void UnmatchLeftGamepadKeyOnPointerUp() // digunakan saat btn up
    {
         isLeftNoteKeyMatch = false;
    }
    public void UnmatchRightGamepadKeyOnPointerUp() // digunakan saat btn up
    {
        isRightNoteKeyMatch = false;
    }

    // ================================= PLAYE PERFORMANCE SCORE PANEL ==========================================
    public void ShowGameplayResult(List<CMPlayerPerformanceScore> playerPerformanceList)
    {
        this.playerPerformanceList = playerPerformanceList;

        ApplyplayerPerformanceValue();
        ratingImageBgComponent.sprite = null;
        performanceReportPanel.SetActive(true);
    }

    private void ApplyplayerPerformanceValue()
    {
        LoopImages();
        StartCoroutine(WaitAndExecute());
        //LoopImages();
        //// Animasi Score dari 0 ke performanceScore
        //AnimatePerformanceValue(0, playerPerformanceList[0].performanceScore, scoreValText, Ease.InCubic, tweenDuration);
        //SetDelay(0.5f);
        //AnimatePerformanceValue(0, playerPerformanceList[0].noteHitSuccess, detailSuksesValText, Ease.InCubic, tweenDuration);
        //SetDelay(0.5f);
        //AnimatePerformanceValue(0, playerPerformanceList[0].noteHitMiss, detailGagalValText, Ease.InCubic, tweenDuration);
        //SetDelay(0.5f);
        //AnimatePerformanceInPercentFloatVal(0, playerPerformanceList[0].noteHitSuccessRate, detailAkurasiValText, Ease.InCubic, tweenDuration);
        //SetDelay(0.5f);
        //StopLoopAndApplyImageAfterDelay();

        //DOVirtual.DelayedCall(tweenDuration + 0.5f, StopLoopAndApplyImageAfterDelay);




        //scoreValText.text = playerPerformanceList[0].performanceScore.ToString();

        //scoreRatingValText.text = playerPerformanceList[0].performanceRating;
        //detailSuksesValText.text = playerPerformanceList[0].noteHitSuccess.ToString();
        //detailGagalValText.text = playerPerformanceList[0].noteHitMiss.ToString();
        //detailAkurasiValText.text = playerPerformanceList[0].noteHitSuccessRate.ToString() + "%";
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



        //if (playerPerformanceList[0].performanceRating == "S")
        //{
        //    ratingImageComponent.sprite = ratingImages[0];
        //}
        //else if (playerPerformanceList[0].performanceRating == "A")
        //{
        //    ratingImageComponent.sprite = ratingImages[1];
        //}
        //else if (playerPerformanceList[0].performanceRating == "B")
        //{
        //    ratingImageComponent.sprite = ratingImages[2];
        //}
        //else
        //{
        //    ratingImageComponent.sprite = ratingImages[3];
        //}
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

    public void RestartGameMode()
    {
        GameManager.Instance.sceneController.ReloadCurrentScene();
    }

    public void PauseGame()
    {
        GameManager.Instance.PauseGame();

    }

    public void ResumeGame()
    {
        GameManager.Instance.ResumeGame();
    }

    public void GoToMenu()
    {
        GameManager.Instance.ResumeGame();
        GameManager.Instance.sceneController.SwitchSceneMainMenu();
    }

}

public class CMPlayerPerformanceScore
{
    public int performanceScore;
    public int noteHitSuccess;
    public int noteHitMiss;
    public float noteHitSuccessRate;
    public string performanceRating;

}
