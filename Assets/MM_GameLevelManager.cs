using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

/// <summary>
/// SCRIPT Untuk melakukan load level di mode Chord Master
/// </summary>
public class MM_GameLevelManager : MonoBehaviour
{
    [SerializeField] MainMenuUIManager mainMenuUIManager;
    //[SerializeField] GameMode gameMode;
    //public enum GameMode
    //{
    //    ChordMaster,
    //    BeatMaster,
    //    MusicalIntervalMaster
    //}
    [Header("Musical Journey References")]
    [SerializeField] List<MJSaveData> levelDataList_MJ; // Berisi De-serialisasi file save JSON (Sisitem save belum di implementasi)
    public List<GameObject> levelGameObjectList_MJ; // Berisi object level yang ada di "Musical Journet Level Selector"
    //[SerializeField] RectTransform musicalJourneyPanelRect;
    [SerializeField] GameObject musicalJourneyPanelGameobject;
    [SerializeField] Sprite starFillSprite_MJ;
    [SerializeField] UISwipeController uiSwipeControllerComponent;



    [Header("Chord Master References")]
    [SerializeField] Transform levelRow_ContentContainerCM;
    [SerializeField] Transform scoreRow_ContentContainerCM;
    [SerializeField] GameObject levelTitleRowPrefabCM;
    [SerializeField] GameObject levelScoreRowPefabCM;
    [SerializeField] Button playButtonCMSelector;

    [SerializeField] List<CMBeatmapDataSO> localBeatmapCMSO;
    //[SerializeField] List<CMFiles> localBeatmapCMFiles;
    [SerializeField] List<CMBeatmapData> localBeatmapCMList;

    [Header("Beat Master References")]
    [SerializeField] Transform levelRow_ContentContainerBM;
    [SerializeField] Transform scoreRow_ContentContainerBM;
    [SerializeField] GameObject levelTitleRowPrefabBM;
    [SerializeField] GameObject levelScoreRowPefabBM;
    [SerializeField] Button playButtonBMSelector;

    [SerializeField] List<BMBeatmapDataSO> localBeatmapBMList;


    [Header("Musical Interval Master References")]
    [SerializeField] Transform levelRow_ContentContainerMIM;
    //[SerializeField] Transform scoreRow_ContentContainerMIM;
    [SerializeField] GameObject levelTitleRowPrefabMIM;
    //[SerializeField] GameObject levelScoreRowPefabMIM;
    [SerializeField] List<Image> starRankObjList;
    [SerializeField] Sprite blankStarImage;
    [SerializeField] Sprite filledStarImage;
    [SerializeField] TextMeshProUGUI completionCaption;
    [SerializeField] Button playButtonMIMSelector;



    [SerializeField] List<MIMBeatmapDataSO> localBeatmapMIMFiles;


    private void Awake()
    {
        //levelDataList_MJ = GameManager.Instance.saveManager.LoadLevelDataMJ();
    }
    void Start()
    {
        localBeatmapCMSO = GameManager.Instance.beatmapLoader.beatmapCMSOFiles;
        //localBeatmapCMFiles = GameManager.Instance.beatmapLoader.beatmapCMFiles;
        localBeatmapCMList = GameManager.Instance.beatmapLoader.beatmapCMList;
        localBeatmapMIMFiles = GameManager.Instance.beatmapLoader.beatmapMIMSOFiles;

        localBeatmapBMList = GameManager.Instance.beatmapLoader.beatmapBMSOFiles;

        // Reset position y di 'Content'
        musicalJourneyPanelGameobject.SetActive(false);
        //=============================================================


        //GameManager.Instance.saveManager.MJ);


        //switch (gameMode)
        //{
        //    case GameMode.ChordMaster:
        //        InstansiateLevelRowCM(localBeatmapCMList);
        //        break;
        //    case GameMode.MusicalIntervalMaster:
        //        InstansiateLevelRowMIM(localBeatmapMIMFiles);
        //        break;
        //}

        InstansiateLevelRowCM(localBeatmapCMList);
        InstansiateLevelRowBM(localBeatmapBMList);
        InstansiateLevelRowMIM(localBeatmapMIMFiles);









        // Nonaktifkan semua object game level
        foreach (var obj in levelGameObjectList_MJ)
        {
            obj.SetActive(false);
        }

        // Tampung MJ Save Data ke sini
        List<MJSaveData> loadedMJSaveData = GameManager.Instance.saveManager.LoadLevelDataMJ();

        // Mencari map level page tempat level terakhir yang telah dibuka
        int lastMapPage = 1;
        foreach (var level in loadedMJSaveData)
        {
            if (level.levelMapPage > lastMapPage)
            {
                lastMapPage = level.levelMapPage;
            }
        }

        // Lompat ke halaman milik map level page level terakhir yang telah dibuka
        uiSwipeControllerComponent.JumpToPage(lastMapPage);

        // Set Tampilan MJ Map Level
        if (loadedMJSaveData.Count != 0)
        {
            // Ubah value pada List "levelDataList_MJ"
            for (int i = 0; i < loadedMJSaveData.Count; i++)
            {
                levelDataList_MJ[i].isLevelCompleted = loadedMJSaveData[i].isLevelCompleted;
                levelDataList_MJ[i].starNumber = loadedMJSaveData[i].starNumber;
                levelDataList_MJ[i].levelMapPage = loadedMJSaveData[i].levelMapPage;
            }

            for (int i = 0; i < levelDataList_MJ.Count; i++)
            {
                // CARA 2
                if (i == 0)
                {
                    MJSaveData data = levelDataList_MJ[i];
                    GameObject levelObj = levelGameObjectList_MJ[i];

                    MJLevelController levelController = levelObj.GetComponent<MJLevelController>();
                    levelController.levelCompleted = data.isLevelCompleted;

                    if (data.isLevelCompleted)
                    {
                        levelObj.SetActive(true);

                        // Update Sprite Star Rating
                        if (data.starNumber == 1)
                        {
                            levelController.gameLevelRating.transform.GetChild(0).GetComponent<Image>().sprite = starFillSprite_MJ;
                        }
                        else if (data.starNumber == 2)
                        {
                            levelController.gameLevelRating.transform.GetChild(0).GetComponent<Image>().sprite = starFillSprite_MJ;
                            levelController.gameLevelRating.transform.GetChild(1).GetComponent<Image>().sprite = starFillSprite_MJ;
                        }
                        else if (data.starNumber == 3)
                        {
                            levelController.gameLevelRating.transform.GetChild(0).GetComponent<Image>().sprite = starFillSprite_MJ;
                            levelController.gameLevelRating.transform.GetChild(1).GetComponent<Image>().sprite = starFillSprite_MJ;
                            levelController.gameLevelRating.transform.GetChild(2).GetComponent<Image>().sprite = starFillSprite_MJ;
                        }

                        // Simpan index
                        //lastCompletedLevelIndex = i;
                    }
                }
                else
                {
                    MJSaveData data = levelDataList_MJ[i];
                    GameObject levelObj = levelGameObjectList_MJ[i];
                    GameObject levelObjLast = levelGameObjectList_MJ[i - 1]; //index Game Level sebelum saat ini (Misal index saat ini = 5, berarti di variabel ini = 4)

                    MJLevelController levelController = levelObj.GetComponent<MJLevelController>();
                    MJLevelController levelControllerLast = levelObjLast.GetComponent<MJLevelController>();
                    levelController.levelCompleted = data.isLevelCompleted;

                    if (levelControllerLast.levelCompleted)
                    {
                        levelObj.SetActive(true);
                    }

                    if (data.isLevelCompleted)
                    {
                        levelObj.SetActive(true);
                        if (data.starNumber == 1)
                        {
                            levelController.gameLevelRating.transform.GetChild(0).GetComponent<Image>().sprite = starFillSprite_MJ;
                        }
                        else if (data.starNumber == 2)
                        {
                            levelController.gameLevelRating.transform.GetChild(0).GetComponent<Image>().sprite = starFillSprite_MJ;
                            levelController.gameLevelRating.transform.GetChild(1).GetComponent<Image>().sprite = starFillSprite_MJ;
                        }
                        else if (data.starNumber == 3)
                        {
                            levelController.gameLevelRating.transform.GetChild(0).GetComponent<Image>().sprite = starFillSprite_MJ;
                            levelController.gameLevelRating.transform.GetChild(1).GetComponent<Image>().sprite = starFillSprite_MJ;
                            levelController.gameLevelRating.transform.GetChild(2).GetComponent<Image>().sprite = starFillSprite_MJ;
                        }
                    }


                }
            }
        }
        else
        {
            GameObject levelObj = levelGameObjectList_MJ[0];
            levelObj.SetActive(true);
        }

        StopAllCoroutines();
    }

    // =======================================================================MUSICAL INTERVAL MASTER=============================================================

    private void InstansiateLevelRowMIM(List<MIMBeatmapDataSO> localBeatmapCMList)
    {
        DestroyChild(levelRow_ContentContainerMIM);
        int numbering = 1;
        foreach (var seq in localBeatmapMIMFiles)
        {
            string prefabName = numbering + "." + seq.name; // beatmap/level title
            GameObject instantiatedPrefab = Instantiate(levelTitleRowPrefabMIM, transform.position, transform.rotation);
            instantiatedPrefab.name = prefabName;
            instantiatedPrefab.transform.SetParent(levelRow_ContentContainerMIM.transform); // Ubah instantiatedPrefab menjadi child dari object ini

            RectTransform rectTransform = instantiatedPrefab.GetComponent<RectTransform>();
            rectTransform.localScale = new Vector3(1f, 1f, 0f);

            int currentIndex = localBeatmapCMList.IndexOf(seq);
            //instantiatedPrefab.GetComponent<LevelListHeader>().currentIndex = currentIndex;
            //instantiatedPrefab.GetComponent<LevelListHeader>().saveFile = localBeatmapCMFiles[currentIndex].save;
            //instantiatedPrefab.GetComponent<LevelListHeader>().audioFile = localBeatmapCMFiles[currentIndex].audio;
            //instantiatedPrefab.GetComponent<LevelListHeader>().audioFile = localBeatmapCMFiles[currentIndex].audio;
            //instantiatedPrefab.GetComponent<LevelListHeader>().levelScoreRow_Prefab = levelScoreRow_Prefab;
            //instantiatedPrefab.GetComponent<LevelListHeader>().levelScoreRow_ContentContainer = levelScoreRow_ContentContainer;

            TextMeshProUGUI textComponent = instantiatedPrefab.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = prefabName;
            }
            else
            {
                Debug.LogWarning("Text component not found in instantiated prefab.");
            }

            Button button = instantiatedPrefab.GetComponent<Button>();
            button.onClick.AddListener(() => mainMenuUIManager.playButtonAnim(instantiatedPrefab.GetComponent<RectTransform>()));
            button.onClick.AddListener(() => InstantiateStarRankMIM(currentIndex));

            numbering++;
        }

        // Re-assign button play OnClick()
        playButtonMIMSelector.onClick.AddListener(() => GameManager.Instance.sceneController.SwitchSceneGameModeMusicalIntervalMaster());
        playButtonMIMSelector.onClick.AddListener(() => GameManager.Instance.SetGameModeToMusicalIntervalMasterMode());
    }
    private void InstantiateStarRankMIM(int currentIndex)
    {
        // Reset star rank to empty/blank star
        foreach (Image star in starRankObjList)
        {
            star.sprite = blankStarImage;
        }

        GameManager.Instance.selectedGameLevelIndex = currentIndex; // ganti value di Gamemanager
        List<MIMSaveData> saveDataCollection = saveDataCollection = GameManager.Instance.saveManager.LoadScoreMIM(currentIndex);


        if (saveDataCollection != null && saveDataCollection.Count != 0)
        {
            MIMSaveData saveData = saveDataCollection[0]; // hanya perlu index 0, karena jumlah data di json hanya 1    
            completionCaption.gameObject.SetActive(false);

            if (saveData.starRating == 3)
            {
                starRankObjList[0].sprite = filledStarImage;
                starRankObjList[1].sprite = filledStarImage;
                starRankObjList[2].sprite = filledStarImage;
            }
            else if (saveData.starRating == 2)
            {
                starRankObjList[0].sprite = filledStarImage;
                starRankObjList[1].sprite = filledStarImage;
            }
            else if (saveData.starRating == 1)
            {
                starRankObjList[0].sprite = filledStarImage;
            }

        }
        else 
        {
            completionCaption.gameObject.SetActive(true);

            Debug.Log("No SaveData Found in this level"); 
        }

        //saveCMList = SortByScore(saveCMList);

        //int numbering = 1;
        //foreach (var item in saveDataCollection)
        //{

        //    string prefabName = numbering.ToString();

        //    // Instansiate, rename & setparent prefab
        //    GameObject instantiatedPrefab = Instantiate(levelTitleRowPrefabMIM, transform.position, transform.rotation);
        //    instantiatedPrefab.name = prefabName;
        //    instantiatedPrefab.transform.SetParent(scoreRow_ContentContainerCM); // Ubah instantiatedPrefab menjadi child dari object ini

        //    //Atur ukuran prefab yang di instansiate
        //    RectTransform rectTransform = instantiatedPrefab.GetComponent<RectTransform>();
        //    rectTransform.localScale = new Vector3(1f, 1f, 0f);


        //    foreach (Transform child in instantiatedPrefab.transform)
        //    {
        //        if (child.name == "Rank Text")
        //        {
        //            // Do something with the child object
        //            TextMeshProUGUI textComponent = child.GetComponent<TextMeshProUGUI>();
        //            textComponent.text = item.performanceRating.ToString();
        //        }
        //        else if (child.name == "Score Text")
        //        {
        //            TextMeshProUGUI textComponent = child.GetComponent<TextMeshProUGUI>();
        //            textComponent.text = item.performanceScore.ToString();
        //        }
        //        else if (child.name == "Date Text")
        //        {
        //            TextMeshProUGUI textComponent = child.GetComponent<TextMeshProUGUI>();
        //            string dateFormatted = convertDateFromJson(item.date.ToString());
        //            textComponent.text = dateFormatted;
        //        }
        //    }
        //    numbering++;
        //}

    }

    // =======================================================================Beat MASTER========================================================================
    // Instansiate button row pada Judul level/lagu pada mode BM

    private void InstansiateLevelRowBM(List<BMBeatmapDataSO> localBeatmapBMList)
    {
        DestroyChild(levelRow_ContentContainerBM);
        DestroyChild(scoreRow_ContentContainerBM);


        int numbering = 1;
        foreach (var seq in localBeatmapBMList)
        {
            if (seq.showInLevelSelector) // Jika "showInLevelSelector == true" pada "BeatmapLoader.beatmapBMSOFiles", maka tampilkan beatmap/level di "BM Row Level Selector"
            {
                string prefabName = numbering + "." + seq.title;
                GameObject instantiatedPrefab = Instantiate(levelTitleRowPrefabCM, transform.position, transform.rotation);
                instantiatedPrefab.name = prefabName;
                instantiatedPrefab.transform.SetParent(levelRow_ContentContainerBM.transform); // Ubah instantiatedPrefab menjadi child dari object ini

                RectTransform rectTransform = instantiatedPrefab.GetComponent<RectTransform>();
                rectTransform.localScale = new Vector3(1f, 1f, 0f);

                //instantiatedPrefab.GetComponent<LevelListHeader>().currentIndex = currentIndex;
                //instantiatedPrefab.GetComponent<LevelListHeader>().saveFile = localBeatmapCMFiles[currentIndex].save;
                //instantiatedPrefab.GetComponent<LevelListHeader>().audioFile = localBeatmapCMFiles[currentIndex].audio;
                //instantiatedPrefab.GetComponent<LevelListHeader>().audioFile = localBeatmapCMFiles[currentIndex].audio;
                //instantiatedPrefab.GetComponent<LevelListHeader>().levelScoreRow_Prefab = levelScoreRow_Prefab;
                //instantiatedPrefab.GetComponent<LevelListHeader>().levelScoreRow_ContentContainer = levelScoreRow_ContentContainer;

                TextMeshProUGUI textComponent = instantiatedPrefab.GetComponentInChildren<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = seq.title;
                }
                else
                {
                    Debug.LogWarning("Text component not found in instantiated prefab.");
                }

                int currentIndex = localBeatmapBMList.IndexOf(seq);

                Button button = instantiatedPrefab.GetComponent<Button>();
                button.onClick.AddListener(() => mainMenuUIManager.playButtonAnim(instantiatedPrefab.GetComponent<RectTransform>()));
                button.onClick.AddListener(() => InstantiateScoreRowsBM(currentIndex));


                button.onClick.AddListener(() => ActivateButtonInteractableBM());

                numbering++;
            }
        }

        // Re-assign button play OnClick()
        playButtonBMSelector.onClick.AddListener(() => GameManager.Instance.sceneController.SwitchSceneGameModeBeatMaster());
        playButtonBMSelector.onClick.AddListener(() => GameManager.Instance.SetGameModeToBeatMasterMode());
    }

    private void InstantiateScoreRowsBM(int currentIndex)
    {
        DestroyChild(scoreRow_ContentContainerBM);

        GameManager.Instance.selectedGameLevelIndex = currentIndex; // ganti value di Gamemanager
        List<BMSaveData> saveBMDataCollection = GameManager.Instance.saveManager.LoadScoreBM(currentIndex);

        //saveCMList = SortByScore(saveCMList);

        int numbering = 1;
        if (saveBMDataCollection != null)
        {
            foreach (var item in saveBMDataCollection)
            {
                string prefabName = numbering.ToString();

                // Instansiate, rename & setparent prefab
                GameObject instantiatedPrefab = Instantiate(levelScoreRowPefabBM, transform.position, transform.rotation);
                instantiatedPrefab.name = prefabName;
                instantiatedPrefab.transform.SetParent(scoreRow_ContentContainerBM); // Ubah instantiatedPrefab menjadi child dari object ini

                //Atur ukuran prefab yang di instansiate
                RectTransform rectTransform = instantiatedPrefab.GetComponent<RectTransform>();
                rectTransform.localScale = new Vector3(1f, 1f, 0f);


                // Display Rank
                foreach (Transform child in instantiatedPrefab.transform)
                {
                    if (child.name == "Rank Text")
                    {
                        // Do something with the child object
                        TextMeshProUGUI textComponent = child.GetComponent<TextMeshProUGUI>();
                        textComponent.text = item.performanceRating.ToString();
                    }
                    else if (child.name == "Score Text")
                    {
                        TextMeshProUGUI textComponent = child.GetComponent<TextMeshProUGUI>();
                        textComponent.text = item.performanceScore.ToString();
                    }
                    else if (child.name == "Date Text")
                    {
                        TextMeshProUGUI textComponent = child.GetComponent<TextMeshProUGUI>();
                        string dateFormatted = convertDateFromJson(item.date.ToString());
                        textComponent.text = dateFormatted;
                    }
                }
                numbering++;
            }
        }
        else
        {
            Debug.Log("TIDAK DITEMUKAN SAVE FILE DI FOLDER 'savebm'");
        }
    }


    private void ActivateButtonInteractableBM()
    {
        if (playButtonBMSelector.interactable == false)
        {
            playButtonBMSelector.interactable = true;
        }
    }

    // =======================================================================CHORD MASTER========================================================================
    // Instansiate button row pada Judul level/lagu pada mode CM
    private void InstansiateLevelRowCM(List<CMBeatmapData> localBeatmapCMList)
    {
        DestroyChild(levelRow_ContentContainerCM);
        DestroyChild(scoreRow_ContentContainerCM);

        int numbering = 1;
        foreach (var seq in localBeatmapCMList)
        {
            if (localBeatmapCMSO[seq.beatmapIndex].displayOnLevelSelector == true) // Jika "displayOnLevelSelector == true" pada "BeatmapLoader.beatmapCMSOFiles", maka tampilkan beatmap/level di "CM Row Level Selector"
            {
                string prefabName = numbering + "." + seq.header.songTitle + " by " + seq.header.songAuthor;
                GameObject instantiatedPrefab = Instantiate(levelTitleRowPrefabCM, transform.position, transform.rotation);
                instantiatedPrefab.name = prefabName;
                instantiatedPrefab.transform.SetParent(levelRow_ContentContainerCM.transform); // Ubah instantiatedPrefab menjadi child dari object ini

                RectTransform rectTransform = instantiatedPrefab.GetComponent<RectTransform>();
                rectTransform.localScale = new Vector3(1f, 1f, 0f);

                int currentIndex = seq.beatmapIndex;
                //instantiatedPrefab.GetComponent<LevelListHeader>().currentIndex = currentIndex;
                //instantiatedPrefab.GetComponent<LevelListHeader>().saveFile = localBeatmapCMFiles[currentIndex].save;
                //instantiatedPrefab.GetComponent<LevelListHeader>().audioFile = localBeatmapCMFiles[currentIndex].audio;
                //instantiatedPrefab.GetComponent<LevelListHeader>().audioFile = localBeatmapCMFiles[currentIndex].audio;
                //instantiatedPrefab.GetComponent<LevelListHeader>().levelScoreRow_Prefab = levelScoreRow_Prefab;
                //instantiatedPrefab.GetComponent<LevelListHeader>().levelScoreRow_ContentContainer = levelScoreRow_ContentContainer;

                TextMeshProUGUI textComponent = instantiatedPrefab.GetComponentInChildren<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = prefabName;
                }
                else
                {
                    Debug.LogWarning("Text component not found in instantiated prefab.");
                }

                Button button = instantiatedPrefab.GetComponent<Button>();
                button.onClick.AddListener(() => mainMenuUIManager.playButtonAnim(instantiatedPrefab.GetComponent<RectTransform>()));
                button.onClick.AddListener(() => InstantiateScoreRowsCM(currentIndex));


                button.onClick.AddListener(() => ActivateButtonInteractableCM());

                numbering++;
            }
        }

        // Re-assign button play OnClick()
        playButtonCMSelector.onClick.AddListener(() => GameManager.Instance.sceneController.SwitchSceneGameModeChordMaster());
        playButtonCMSelector.onClick.AddListener(() => GameManager.Instance.SetGameModeToChordMasterMode());
    }

    private void ActivateButtonInteractableCM()
    {
        if (playButtonCMSelector.interactable == false)
        {
            playButtonCMSelector.interactable = true;
        }
    }

    private void InstantiateScoreRowsCM(int currentIndex)
    {
        DestroyChild(scoreRow_ContentContainerCM);

        GameManager.Instance.selectedGameLevelIndex = currentIndex; // ganti value di Gamemanager
        List<CMSaveData> saveCMDataCollection = GameManager.Instance.saveManager.LoadScoreCM(currentIndex);

        //saveCMList = SortByScore(saveCMList);

        int numbering = 1;
        if (saveCMDataCollection != null)
        {
            foreach (var item in saveCMDataCollection)
            {
                string prefabName = numbering.ToString();

                // Instansiate, rename & setparent prefab
                GameObject instantiatedPrefab = Instantiate(levelScoreRowPefabCM, transform.position, transform.rotation);
                instantiatedPrefab.name = prefabName;
                instantiatedPrefab.transform.SetParent(scoreRow_ContentContainerCM); // Ubah instantiatedPrefab menjadi child dari object ini

                //Atur ukuran prefab yang di instansiate
                RectTransform rectTransform = instantiatedPrefab.GetComponent<RectTransform>();
                rectTransform.localScale = new Vector3(1f, 1f, 0f);


                foreach (Transform child in instantiatedPrefab.transform)
                {
                    if (child.name == "Rank Text")
                    {
                        // Do something with the child object
                        TextMeshProUGUI textComponent = child.GetComponent<TextMeshProUGUI>();
                        textComponent.text = item.performanceRating.ToString();
                    }
                    else if (child.name == "Score Text")
                    {
                        TextMeshProUGUI textComponent = child.GetComponent<TextMeshProUGUI>();
                        textComponent.text = item.performanceScore.ToString();
                    }
                    else if (child.name == "Date Text")
                    {
                        TextMeshProUGUI textComponent = child.GetComponent<TextMeshProUGUI>();
                        string dateFormatted = convertDateFromJson(item.date.ToString());
                        textComponent.text = dateFormatted;
                    }
                }
                numbering++;
            }
        }
        else
        {
            Debug.Log("TIDAK DITEMUKAN SAVE FILE DI FOLDER 'savecm'");
        }
    }

    // =======================================================================MUSICAL JOURNEY========================================================================
    public void PlayGameModeScene_MJ()
    {
        switch (GameManager.Instance.CurrentGameModeState)
        {
            case GameManager.GameModeState.ChordMasterMode:
                GameManager.Instance.sceneController.SwitchSceneGameModeChordMaster();
                break;
            case GameManager.GameModeState.BeatMasterMode:
                GameManager.Instance.sceneController.SwitchSceneGameModeBeatMaster();
                break;
            case GameManager.GameModeState.MusicalIntervalMasterMode:
                GameManager.Instance.sceneController.SwitchSceneGameModeMusicalIntervalMaster();
                break;
        }
    }

    // =======================================================================UNIVERSAL===========================================================================
    private void DestroyChild(Transform objTransform)
    {
        foreach (Transform child in objTransform)
        {
            Destroy(child.gameObject);
        }
    }
    private string convertDateFromJson(string jsonDateString)
    {
        try
        {
            DateTime date = DateTime.ParseExact(jsonDateString.Trim(), "ddMMyy", null);
            string dateFormatted = date.ToString("dd/MM/yyyy");
            return dateFormatted;
        }
        catch (FormatException)
        {
            // Handling the exception if the format is not correct
            return "Invalid date format";
        }
        //Debug.Log(jsonDateString);
        //DateTime date = DateTime.ParseExact(jsonDateString, "ddMMyy", null);
        //string dateFormatted = date.ToString("dd/MM/yyyy");
        //return dateFormatted;
    }
}
