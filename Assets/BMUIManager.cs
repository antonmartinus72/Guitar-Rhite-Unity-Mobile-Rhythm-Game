using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BMUIManager : MonoBehaviour
{

    [SerializeField] RectTransform readyPanel;
    [HideInInspector] public AudioSource gameplayMusicAudioSource;
    private void OnEnable()
    {
        GameManager.Instance.SetGameStateToPlaying();
        PauseGame();
        readyPanel.gameObject.SetActive(true);
    }
    void Start()
    {
        
        //PauseGame();
    }

    public void PauseGame()
    {
        gameplayMusicAudioSource.Pause();
        GameManager.Instance.PauseGame();
    }

    public void ResumeGame()
    {
        //GameManager.Instance.SetGameStateToPaused();
        gameplayMusicAudioSource.Play();
        GameManager.Instance.ResumeGame();
    }

    public void SetPauseGameState()
    {
        GameManager.Instance.SetGameStateToPaused();
    }

    public void RestartGameMode() 
    {
        GameManager.Instance.sceneController.ReloadCurrentScene();
    }

    public void BackToMainMenu()
    {
        GameManager.Instance.sceneController.SwitchSceneMainMenu();
    }
}
