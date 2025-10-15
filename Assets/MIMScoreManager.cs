using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MIMScoreManager : MonoBehaviour
{
    // Start is called before the first frame update
    MIMGameController gameController;
    List<MIMSequenceData> sequenceData;
    int currentSequenceIndex = 0;

    [Header("Animation")]
    [SerializeField] GameObject gameResultPanel;
    [SerializeField] Animator gameResult_StarRatingAnimator;
    [SerializeField] GameObject answerCorrectGameObj;
    [SerializeField] GameObject answerWrongGameObj;
    [SerializeField] Animator answerCorrect_Animator;
    [SerializeField] Animator answerWrong_Animator;

    [Header("Answer Validation Animation Properties")]
    [SerializeField] Vector2 hiddenPosition = new Vector2(0f, -300f);         // Posisi awal panel (di luar layar)
    [SerializeField] Vector2 shownPosition = new Vector2(0f, 0f);          // Posisi akhir panel (di dalam layar)
    [SerializeField] float duration = 0.5f;          // Durasi animasi

    //Audio
    AudioSource audioCorrectAnswerSource;
    AudioSource audioWrongAnswerSource;
    [SerializeField] AudioClip audioCorrectAnswerClip;
    [SerializeField] AudioClip audioWrongAnswerClip;

    [Header("Game Scores")]
    public int totalWrongAnswer = 0; // total kesalahan jawaban. Untuk keperluan highscore
    public int totalSequence = 0; // total sequence di beatmap
    //public List<int> unansweredSequenceIndexList;

    [Header("Game Result Panel")]
    [SerializeField] Button exitButton;

    // Save game
    int gameResultStarRating = 1; // 1-3 rating star/bintang

    // Other
    // Untuk keperluan perbandingan di  ValidateAnswerPattern()
    Dictionary<string, string> enharmonicPairs = new Dictionary<string, string>
    {
        // string yang berpasangan akan dibandingkan, jika salah satu benar maka answer dianggap valid.
        // Misal jika 'C#' tidak valid, namun ternyata jawaban yang dipilih player adalah 'Db', maka jawaban menjadi valid.
        // Bentuk enharmonic
        { "C#", "Db" }, { "Db", "C#" },
        { "D#", "Eb" }, { "Eb", "D#" },
        { "F#", "Gb" }, { "Gb", "F#" },
        { "G#", "Ab" }, { "Ab", "G#" },
        { "A#", "Bb" }, { "Bb", "A#" }
    };

    void Start()
    {
        gameController = gameObject.GetComponent<MIMGameController>();
        sequenceData = gameObject.GetComponent<MIMBeatmapLoader>().sequenceData;
        totalSequence = sequenceData.Count;

        // AUDIO
        audioCorrectAnswerSource = gameObject.AddComponent<AudioSource>();
        audioWrongAnswerSource = gameObject.AddComponent<AudioSource>();
        audioCorrectAnswerSource.clip = audioCorrectAnswerClip;
        audioWrongAnswerSource.clip = audioWrongAnswerClip;

        // GAMESCORE EXIT BUTTON
        exitButton.onClick.AddListener(() => ExitGameMode());

    }

    private void ExitGameMode()
    {
        GameManager.Instance.sceneController.SwitchSceneMainMenu();
    }

    public void ValidateAnswerPattern()
    {
        currentSequenceIndex = gameController.currentSequenceIndex;
        var beatmapSequenceKeysData = sequenceData[currentSequenceIndex].keysData;
        var answerPlaceholder = gameController.answerPlaceholder;

        // Pastikan jumlah slot dan kunci sama
        if (beatmapSequenceKeysData.Count != answerPlaceholder.childCount)
        {
            Debug.Log("Jumlah slot dan sequence key tidak sama!");
            return;
        }

        bool allSlotsFilled = true;
        bool allKeysCorrect = true;

        for (int i = 0; i < beatmapSequenceKeysData.Count; i++)
        {
            var sequenceKeySO = beatmapSequenceKeysData[i].keySO;
            var slot = answerPlaceholder.GetChild(i); // Akses slot dari child

            if (slot.childCount == 1)
            {
                var itemKeySO = slot.GetChild(0).GetComponent<DragableItem>().keySO;

                // cek pasangan enharmonic
                if (sequenceKeySO.keyName != itemKeySO.keyName && (!enharmonicPairs.ContainsKey(sequenceKeySO.keyName) || enharmonicPairs[sequenceKeySO.keyName] != itemKeySO.keyName))
                {
                    Debug.Log($"Key tidak cocok! Slot: {slot.name}, Jawaban: {itemKeySO.keyName}, Kunci Benar: {sequenceKeySO.keyName}");
                    allKeysCorrect = false;
                }
            }
            else
            {
                Debug.Log($"Slot {slot.name} kosong!");
                allSlotsFilled = false;
            }
        }

        if (!allSlotsFilled)
        {
            // jika ada 1 slot atau lebih yang kosong.

            EmptyAnswerAction(); // Tindakan untuk slot yang belum terisi
            Debug.Log("[END] Tidak boleh ada slot yang kosonh");
            //return false;
            return;
        }

        if (!allKeysCorrect)
        {
            // jika satu atau lebih jawaban salah saat semua slot terisi

            WrongAnswerAction(); // Tindakan untuk kunci yang salah
            Debug.Log("[END] Tidak boleh ada jawaban yang salah");
            return;
            //return false;
        }

        CorrectAnswerAction(); // Semua slot terisi dengan kunci yang benar
        Debug.Log("[END] Semua jawaban benar!");
        //return true;
    }

    private void CorrectAnswerAction()
    {
        gameController.unansweredSequenceIndexList.RemoveAt(0); // jika jawaban benar, hapus index 0

        RectTransform rect = answerCorrectGameObj.GetComponent<RectTransform>();
        rect.anchoredPosition = hiddenPosition;
        rect.gameObject.SetActive(true);
        rect.DOAnchorPos(shownPosition, duration).SetEase(Ease.OutBack);
        answerCorrect_Animator.SetTrigger("PlayAnim");
        audioCorrectAnswerSource.Play();

        gameController.LoadNewSequences();
        Debug.Log("Jawaban Benar!");
    }

    private void EmptyAnswerAction()
    {
        Debug.Log("Jawaban Salah!");
    }

    private void WrongAnswerAction()
    {
        // Jika jawaban salah, pindahkan value pada index 0 ke index akhir
        // Simpan elemen di indeks ke-0
        int firstElement = gameController.unansweredSequenceIndexList[0];
        // Hapus elemen di indeks ke-0
        gameController.unansweredSequenceIndexList.RemoveAt(0);
        // Tambahkan elemen tersebut ke akhir list
        gameController.unansweredSequenceIndexList.Add(firstElement);

        RectTransform rect = answerWrongGameObj.GetComponent<RectTransform>();
        rect.anchoredPosition = hiddenPosition;
        rect.gameObject.SetActive(true);
        rect.DOAnchorPos(shownPosition, duration).SetEase(Ease.OutBack);
        answerWrong_Animator.SetTrigger("PlayAnim");
        audioWrongAnswerSource.Play();

        gameController.LoadNewSequences();
        totalWrongAnswer++;

        Debug.Log("Jawaban Salah!");
    }

    public void ShowGameResultPanel()
    {
        gameResultPanel.SetActive(true);

        if (totalWrongAnswer == 0)
        {
            gameResult_StarRatingAnimator.SetTrigger("ThreeStar_Trigger");
            gameResultStarRating = 3;
        }
        else if (totalWrongAnswer == 1 || totalWrongAnswer == 2)
        {
            gameResult_StarRatingAnimator.SetTrigger("TwoStar_Trigger");
            gameResultStarRating = 2;
        }
        else
        {
            gameResult_StarRatingAnimator.SetTrigger("OneStar_Trigger");
            gameResultStarRating = 1;
        }

        //Debug.Log("totalWrongAnswer : " + totalWrongAnswer);

        if (!GameManager.Instance.isMusicalJourneyMode) // Jika Game Mode ini dimulai dari "MIM Level Selector"
        {
            GameManager.Instance.saveManager.SaveScoreMIM(gameResultStarRating);
        }
        else // Jika Game Mode ini dimulai dari "Musical Journey Level Selector"
        {
            GameManager.Instance.saveManager.SaveLevelDataMJ(GameManager.Instance.selectedMusicalJourneyLevelIndex, gameResultStarRating, GameManager.Instance.selectedMusicalJourneyLevelIndexPage);
        }
    }

}
