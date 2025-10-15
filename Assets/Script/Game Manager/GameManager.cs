using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    //public SaveManager SaveManager { get; private set; }
    public SceneController sceneController { get; private set; }
    public BeatmapLoader beatmapLoader { get; private set; }
    // Status permainan
    public enum GameState
    {
        MainMenu,
        LevelSelection,
        Playing,
        Paused,
        GameOver
        // Add more game states as needed...
    }
    // Status Game Mode yang dipilih saat ini
    public enum GameModeState
    {
        ChordMasterMode,
        BeatMasterMode,
        MusicalIntervalMasterMode

        // Add more game states as needed...
    }
    
    public GameState CurrentGameState { get; private set; }
    public GameModeState CurrentGameModeState { get; private set; }

    // Inspector

    [Header("Debug")]
    [SerializeField] private string CurrentRunningGameState; //Debug
    [SerializeField] private string CurrentRunningGameModeState; // Debug

    [Header("Level Loader")]
    public int selectedGameLevelIndex; // dimodifikasi oleh Level Selector Panel
    public bool isMusicalJourneyMode; // dimodifikasi oleh 'play Button' di Selector Panel Musical Journey
    public int selectedMusicalJourneyLevelIndex; // dimodifikasi oleh 'play Button' di Selector Panel Musical Journey
    public int selectedMusicalJourneyLevelIndexPage; // dimodifikasi oleh 'play Button' di Selector Panel Musical Journey
    //public BeatmapLoader beatmapLoader; // biarkan kosong / jangan di assign di inspector
    public SaveManager saveManager;
    public VolumeManager volumeManager;

    [Header("Global Audio")]
    public AudioSource clickSoundAudioSource;
    public AudioClip clickSoundAudioClip;
    //public SaveManager saveManager; // biarkan kosong / jangan di assign di inspector
    //public SceneController sceneController; // biarkan kosong / jangan di assign di inspector

    // UI
    

    // End of Inspector
    private void FixedUpdate()
    {
        // Debug
        CurrentRunningGameState = CurrentGameState.ToString();
        CurrentRunningGameModeState = CurrentGameModeState.ToString();
    }

    private void Awake()
    {
        Debug.Log("------------------------------GAME MANAGER INSTANTITED-----------------------------");

        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            //SaveManager = new SaveManager();
            //SceneController = gameObject.AddComponent<SceneController>();
            //SceneController.loadingPanelPrefab = Resources.Load<GameObject>("Assets/Prefab/Loading Screen/Loading Panel Canvas.prefab");
            //SceneController = new SceneController();

            if (sceneController == null)
            {
                sceneController = GetComponent<SceneController>(); // Ensure it's correctly referenced
                if (sceneController == null)
                {
                    Debug.LogError("bceneController component is missing!");
                }
            }

            if (beatmapLoader == null)
            {
                beatmapLoader = GetComponent<BeatmapLoader>(); // Ensure it's correctly referenced
                if (beatmapLoader == null)
                {
                    Debug.LogError("beatmapLoader component is missing!");
                }
            }

        }
        else if (instance != this)
        {
            Debug.Log("------------------------------GameManager is already exist! Deleting new instantiated GameManager------------------------------");
            Destroy(gameObject);
        }

        // Initialize game state
        CurrentGameState = GameState.MainMenu;

        // Assign global audio
        clickSoundAudioSource = gameObject.AddComponent<AudioSource>();
        clickSoundAudioSource.clip = clickSoundAudioClip;
    }

    // Method untuk memulai permainan
    public void StartGame()
    {
        CurrentGameState = GameState.Playing;
    }

    // UNTUK START GAME MODE PADA MAIN MENU
    public void StartGameMode() {
        CurrentGameState = GameState.Playing;

        if (CurrentGameModeState == GameModeState.ChordMasterMode)
        {
            //Debug.Log("CM Selected with level Index : " + selectedGameLevelIndex);
            sceneController.SwitchSceneGameModeChordMaster();
            //throw new NotImplementedException();
        }
        else if (CurrentGameModeState == GameModeState.BeatMasterMode)
        {
            //Debug.Log("CM Selected with level Index : " + selectedGameLevelIndex);
            throw new NotImplementedException();
        }
        else
        {
            //Debug.Log("NOT IN ANY STATE, with level Index : " + selectedGameLevelIndex);
            throw new NotImplementedException();
        }
    }


    // Method untuk mengakhiri permainan
    public void EndGame()
    {
        CurrentGameState = GameState.GameOver;
        // Add logic to end the game
    }

    // Method untuk melakukan pause permainan
    public void PauseGame()
    {
        if (CurrentGameState == GameState.Playing)
        {
            CurrentGameState = GameState.Paused;
            Time.timeScale = 0f;
        }
    }

    public void ResumeGame()
    {
        if (CurrentGameState == GameState.Paused)
        {
            CurrentGameState = GameState.Playing;
            Time.timeScale = 1f;
            // Add logic to resume the game
        }
    }

    public void SetGameStateToPlaying()
    {
        CurrentGameState = GameState.Playing;
    }

    public void SetGameStateToPaused()
    {
        CurrentGameState = GameState.Paused;
    }

    // Method untuk masuk kedalam daftar pilihan level pada modeChord Master
    public void SetGameModeToChordMasterMode()
    {
        CurrentGameModeState = GameModeState.ChordMasterMode;
        // Add logic to start the game
    }
    public void SetGameModeToBeatMasterMode()
    {
        CurrentGameModeState = GameModeState.BeatMasterMode;
        // Add logic to start the game
    }
    public void SetGameModeToMusicalIntervalMasterMode()
    {
        CurrentGameModeState = GameModeState.MusicalIntervalMasterMode;
        // Add logic to start the game
    }

    public void SetGameModeToMusicalJouneyMode()
    {
        isMusicalJourneyMode = true;
    }

    public void UnsetGameModeToMusicalJouneyMode()
    {
        isMusicalJourneyMode = false;
    }


    //public void SetGameModeToMusicalJourneyMode()
    //{
    //    CurrentGameModeState = GameModeState.MusicalJourneyMode;
    //    // Add logic to start the game
    //}


    // Method untuk melanjutkan permainan


    // Method untuk restart permainan
    public void RestartGame()
    {
        // Add logic to restart the game
        StartGame();
    }

    // Method untuk keluar dari permainan
    public void QuitGame()
    {
        // Add logic to quit the game
        Application.Quit();
    }

    // Method untuk menangani perubahan skor
    //public void UpdateScore(int score)
    //{
    //    // Add logic to update the score
    //}


    // Method untuk menyimpan status permainan
    //public void SaveGame()
    //{
    //    // Add logic to save game state
    //}

    //// Method untuk memuat status permainan
    //public void LoadGame()
    //{
    //    // Add logic to load game state
    //}

    // Play game berdasarkan index yang dipilih dan state yang sedang berjalan
    public void SwitchScene()
    {
        SceneManager.LoadScene("Gameplay ChordMaster");
    }

}
