using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public enum SceneList
    {
        MainMenu,
        GameMode_ChordMaster,
        GameMode_BeatMaster,
        GameMode_MusicalIntervalMaster,
        GameMode_Adventure
        // Add more game states as needed...
    }

    public GameObject loadingPanelPrefab;
    public GameObject loadingPanel;      // Panel for loading screen
    Slider loadingSlider;         // Slider for loading progress
    TextMeshProUGUI progressText; // Text for displaying loading percentage

    private void Awake()
    {
        Debug.Log("------------------------------SCENE CONTROLLER INSTANTITED-----------------------------");

        EnsureLoadingPanel();
    }
    public void SwitchScene(SceneList scene)
    {
        string sceneName = scene.ToString(); // Konversi enum ke string yang sesuai dengan nama scene
        SceneManager.LoadScene(sceneName);
    }
    
    public void SwitchSceneGameModeChordMaster()
    {
        //SwitchScene(SceneList.GameModeChordMaster);

        string selectedScene = SceneList.GameMode_ChordMaster.ToString();
        EnsureLoadingPanel();
        loadingPanel.SetActive(true);
        StartCoroutine(LoadAsyncScene(selectedScene));
    }

    public void SwitchSceneGameModeBeatMaster()
    {
        //SwitchScene(SceneList.GameModeChordMaster);

        string selectedScene = SceneList.GameMode_BeatMaster.ToString();
        EnsureLoadingPanel();
        loadingPanel.SetActive(true);
        StartCoroutine(LoadAsyncScene(selectedScene));
    }

    public void SwitchSceneGameModeMusicalIntervalMaster()
    {
        //SwitchScene(SceneList.GameModeChordMaster);

        string selectedScene = SceneList.GameMode_MusicalIntervalMaster.ToString();
        EnsureLoadingPanel();
        //var instanceLoadingPanel = GameManager.Instance.SceneController.loadingPanel;
        loadingPanel.SetActive(true);
        //loadingPanel.SetActive(true);
        StartCoroutine(LoadAsyncScene(selectedScene));
    }

    public void ReloadCurrentScene()
    {
        string selectedScene = SceneManager.GetActiveScene().name;
        EnsureLoadingPanel();
        //var instanceLoadingPanel = GameManager.Instance.SceneController.loadingPanel;
        loadingPanel.SetActive(true);
        //loadingPanel.SetActive(true);
        StartCoroutine(LoadAsyncScene(selectedScene));
    }

    public void SwitchSceneMainMenu()
    {
        string selectedScene = SceneList.MainMenu.ToString();
        EnsureLoadingPanel();
        loadingPanel.SetActive(true);
        Debug.Log("SCENE NAME : " + selectedScene);
        //StopAllCoroutines();

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is null!");
            return;
        }
        if (GameManager.Instance.sceneController == null)
        {
            Debug.LogError("GameManager.Instance.SceneController is null!");
            return;
        }

        StartCoroutine(GameManager.Instance.sceneController.LoadAsyncScene(selectedScene));
    }

    void EnsureLoadingPanel()
    {
        if (GameManager.Instance.sceneController.loadingPanel == null)
        {
            // Instantiate the loading panel only if it's not already instantiated
            loadingPanel = Instantiate(loadingPanelPrefab);

            // Find the slider and text within the panel
            loadingSlider = loadingPanel.GetComponentInChildren<Slider>();
            progressText = loadingPanel.GetComponentInChildren<TextMeshProUGUI>();

            DontDestroyOnLoad(loadingPanel);
            loadingPanel.SetActive(false);
        }

        //if (GameManager.Instance.SceneController.loadingPanel == null)
        //{
        //    // Instantiate loading panel from prefab and set it to inactive
        //    GameManager.Instance.SceneController.loadingPanel = Instantiate(loadingPanelPrefab);

        //    // Find Slider and TextMeshProUGUI components in the instantiated panel
        //    loadingSlider = GameManager.Instance.SceneController.loadingPanel.GetComponentInChildren<Slider>();
        //    progressText = GameManager.Instance.SceneController.loadingPanel.GetComponentInChildren<TextMeshProUGUI>();

        //    loadingSlider = GameManager.Instance.SceneController.loadingPanel.transform.Find("Panel/Slider").GetComponent<Slider>();
        //    progressText = GameManager.Instance.SceneController.loadingPanel.transform.Find("Panel/Slider/LoadingBar Percentage").GetComponent<TextMeshProUGUI>();

        //    DontDestroyOnLoad(GameManager.Instance.SceneController.loadingPanel);
        //    GameManager.Instance.SceneController.loadingPanel.SetActive(false);
        //}
    }



    public IEnumerator LoadAsyncScene(string sceneName)
    {
        // Start loading the scene asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // Prevent the scene from being activated immediately
        operation.allowSceneActivation = false;


        if (loadingSlider == null || progressText == null)
        {
            Debug.LogError("Loading slider or progress text is not properly assigned!");
        }

        // While the scene is still loading
        while (!operation.isDone)
        {
            // Get the current loading progress (ranges from 0 to 1)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            // Update slider and text to reflect the progress
            loadingSlider.value = progress;
            progressText.text = Mathf.RoundToInt(progress * 100) + "%";

            // If the scene is fully loaded (reaches 90% progress)
            if (operation.progress >= 0.9f)
            {
                // Show 100% and wait for user input to continue
                loadingSlider.value = 1f;
                progressText.text = "100%";

                // You can activate the scene automatically or based on a condition
                operation.allowSceneActivation = true;
            }

            // Yielding until the next frame
            yield return null;
        }

        // Once loading is complete, disable the loading panel
        loadingPanel.SetActive(false);
    }
}
