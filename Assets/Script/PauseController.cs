using UnityEngine;


public class PauseController : MonoBehaviour
{

    public bool isBGMReadyToPlay = false;

    public void ShowPausePanel(GameObject pausePanel)
    {
        pausePanel.SetActive(true);
        GameManager.Instance.PauseGame();
        Debug.Log("GAME STATE : " + GameManager.Instance.CurrentGameState);
    }

    public void HidePausePanel(GameObject pausePanel)
    {
        pausePanel.SetActive(false);
        GameManager.Instance.ResumeGame();
        Debug.Log("GAME STATE : " + GameManager.Instance.CurrentGameState);
    }

    public void PlayBGM(GameObject bgmAudio)
    {

        var bgmAudioComponent = bgmAudio.GetComponent<AudioSource>();

        if (bgmAudioComponent.isPlaying == false)
        {
            bgmAudioComponent.Play();
        }
    }

    public void PlayBGMIfReady(GameObject bgmAudio)
    {

        var bgmAudioComponent = bgmAudio.GetComponent<AudioSource>();

        if (isBGMReadyToPlay)
        {
            bgmAudioComponent.Play();
        }
    }
    public void PauseBGM(GameObject bgmAudio)
    {
        var bgmAudioComponent = bgmAudio.GetComponent<AudioSource>();
        bgmAudioComponent.Pause();
    }
}
