using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MJLevelController : MonoBehaviour
{
    [Header("Level Data")]
    [SerializeField] GameManager.GameModeState gameModeType;
    [SerializeField] int beatmapIndex = 0;

    [Header("Visual")]
    //[SerializeField] bool enableGameLevelStepAnimation = true;
    public bool levelCompleted = false;
    [SerializeField] float stepAnimDuration = 0.25f;
    [SerializeField] float stepIntervalDuration = 0.15f;
    [SerializeField] float levelAnimDuration = 0.25f;
    [SerializeField] float levelIntervalDuration = 0.5f;
    //[SerializeField] bool enableLevelAnimation;
    [Header("References")]
    [SerializeField] List<GameObject> gameLevelStepsList;
    [SerializeField] GameObject gameLevel;
    [SerializeField] GameObject gameLevelAvatar;
    public GameObject gameLevelRating;
    [SerializeField] GameObject gameLevelCaption;
    [SerializeField] Transform stepTransform;
    [SerializeField] Transform confirmPanel;
    [SerializeField] TextMeshProUGUI confirmationCaption;
    [SerializeField] Button playConfirmButton;
    [SerializeField] MM_GameLevelManager MMGameLevelManager;


    private void OnEnable()
    {
        foreach (Transform child in stepTransform)
        {
            GameObject obj = child.gameObject;
            gameLevelStepsList.Add(obj);
        }

        if (levelCompleted)
        {
            foreach (var obj in gameLevelStepsList)
            {
                obj.SetActive(true);
            }
            gameLevel.SetActive(true);
            gameLevelAvatar.SetActive(false);
            gameLevelRating.SetActive(true);
            gameLevelCaption.SetActive(true);
        }
        else
        {
            foreach (var obj in gameLevelStepsList)
            {
                obj.SetActive(false);
            }
            gameLevel.SetActive(false);
            gameLevelAvatar.SetActive(false);
            gameLevelRating.SetActive(false);
            gameLevelCaption.SetActive(false);

            StartCoroutine(EnableObjectsCoroutine());
        }
        
    }

    public void ChangeGameModeState()
    {
        switch (gameModeType)
        {
            case GameManager.GameModeState.ChordMasterMode:
                GameManager.Instance.SetGameModeToChordMasterMode();
                break;
            case GameManager.GameModeState.BeatMasterMode:
                GameManager.Instance.SetGameModeToBeatMasterMode();
                break;
            case GameManager.GameModeState.MusicalIntervalMasterMode:
                GameManager.Instance.SetGameModeToMusicalIntervalMasterMode();
                break;
        }
    }

    private void OnDisable()
    {
        gameLevelStepsList.Clear();
    }

    IEnumerator EnableObjectsCoroutine()
    {
        foreach (GameObject obj in gameLevelStepsList)
        {
            obj.SetActive(true);
            obj.transform.localScale = Vector3.zero;
            obj.transform.DOScale(Vector3.one, stepAnimDuration).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.25f); // Menunggu selama 0.5 detik
        }

        PlayPopupAnimation(gameLevel);
        yield return new WaitForSeconds(0.25f); // Menunggu selama 0.5 detik
        PlayPopupAnimation(gameLevelAvatar);
        PlayPopupAnimation(gameLevelRating);
        PlayPopupAnimation(gameLevelCaption);


        //gameLevel.SetActive(true);
        //yield return new WaitForSeconds(0.5f); // Menunggu selama 0.5 detik
        //gameLevelAvatar.SetActive(true);
        //gameLevelRating.SetActive(true);
        //gameLevelCaption.SetActive(true);
    }

    private void PlayPopupAnimation(GameObject obj) // step animation
    {
        obj.SetActive(true);
        obj.transform.localScale = Vector3.zero;
        obj.transform.DOScale(Vector3.one, levelAnimDuration).SetEase(Ease.OutBack);
    }

    //public void SetSceneOnClickButton()
    //{
    //    switch (gameModeType)
    //    {
    //        case GameManager.GameModeState.ChordMasterMode:
    //            playConfirmButton.onClick.AddListener(() => GameManager.Instance.sceneController.SwitchSceneGameModeChordMaster());
    //            break;
    //        case GameManager.GameModeState.BeatMasterMode:
    //            playConfirmButton.onClick.AddListener(() => GameManager.Instance.sceneController.SwitchSceneGameModeBeatMaster());
    //            break;
    //        case GameManager.GameModeState.MusicalIntervalMasterMode:
    //            playConfirmButton.onClick.AddListener(() => GameManager.Instance.sceneController.SwitchSceneGameModeMusicalIntervalMaster());
    //            break;
    //    }
    //}

    public void ChangeConfirmationMessage()
    {
        string text = "Mainkan Level " + gameObject.name + "?";
        confirmationCaption.text = text;
    }

    public void ChangeGameManagerSelectedIndex()
    {
        GameManager.Instance.selectedGameLevelIndex = beatmapIndex;
    }

    public int FindGameObjectIndex(GameObject targetObject)
    {
        // Mengembalikan indeks dari targetObject di dalam gameObjectList
        int index = MMGameLevelManager.levelGameObjectList_MJ.IndexOf(targetObject);

        if (index != -1)
        {
            Debug.Log("Index of target object: " + index);
        }
        else
        {
            Debug.LogWarning("Target object not found in the list.");
        }

        return index;
    }

    public void ChangeGameManagerSelectedMusicalJourneyLevelIndex(GameObject targetObject)
    {
        GameManager.Instance.selectedMusicalJourneyLevelIndex = FindGameObjectIndex(targetObject);
        GameManager.Instance.selectedMusicalJourneyLevelIndexPage = GetLevelMapPageNumber(gameObject.name);
    }

    private int GetLevelMapPageNumber(string input)
    {
        // Memisahkan string berdasarkan karakter '-'
        string[] parts = input.Split('-');

        // Mengembalikan bagian pertama (angka sebelum '-')
        int numberBeforeDash = int.Parse(parts[0]);

        return numberBeforeDash;
    }

}
