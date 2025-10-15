using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("UI Global Settings (Show Panel)")]
    [SerializeField] float panelShowTweenTime;
    [SerializeField] float panelCloseTweenTime;
    [SerializeField] Ease panelShowEaseType;
    [SerializeField] Ease panelCloseEaseType;
    [SerializeField] Vector2 panelShowPosition;
    [SerializeField] Vector2 panelClosePosition;



    [Header("Main Menu Navigation Bar")]
    [SerializeField] Vector2 mainMenuNavBarShowPosition;
    [SerializeField] Vector2 mainMenuNavBarClosePosition;
    [SerializeField] Ease mainMenuNavBarShowEaseType;
    [SerializeField] Ease mainMenuNavBarCloseEaseType;
    [SerializeField] float mainMenuNavShowTweenTime;
    [SerializeField] float mainMenuNavCloseTweenTime;



    [Header("Setting Panel Audio Slider")]
    // References
    AudioSource audioSource_MusicVolume;
    AudioSource audioSource_SfxVolume;

    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;

    [SerializeField] TextMeshProUGUI masterVolumeText;
    [SerializeField] TextMeshProUGUI musicVolumeText;
    [SerializeField] TextMeshProUGUI sfxVolumeText;

    //Range value
    [Range(0f, 1.5f)]
    [SerializeField] float audioMasterVolume = 1f;
    [Range(0f, 1.5f)]
    [SerializeField] float audioMusicVolume = 1f;
    [Range(0f, 1.5f)]
    [SerializeField] float audioSfxVolume = 1f;



    [Header("Any Button UI")]
    [SerializeField] private AudioClip tapSoundClip; // AudioClip yang akan dimainkan saat tombol ditekan
    [SerializeField] private AudioClip clickSoundClip; // AudioClip yang akan dimainkan saat tombol ditekan
    public AudioSource soundSource; // Referensi ke AudioSource


    private void Awake()
    {
        // Add AudioSource for button click sound
        if (soundSource == null)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            soundSource = audioSource;
        }

        // Audio Settings Panel
    }

    private void Start()
    {
        InitializeAudioSettings();
        Debug.Log("Load Time [MainMenuUIManager] : " + Time.time);

    }

    public void ShowPanel(RectTransform panelTarget)
    {
        panelTarget.gameObject.SetActive(true);
        Vector2 pos = panelShowPosition;
        panelTarget.DOAnchorPos(pos, panelShowTweenTime).SetEase(panelShowEaseType);
        //panelTarget.DOLocalMove(pos, panelShowTweenTime).SetEase(panelShowEaseType);
    }

    public void ClosePanel(RectTransform panelTarget)
    {
        Vector2 pos = panelClosePosition;
        panelTarget.DOAnchorPos(pos, panelCloseTweenTime).SetEase(panelCloseEaseType).OnComplete(() => panelTarget.gameObject.SetActive(false)); ;
    }

    public void MainMenuSidebarShow(RectTransform panelTarget)
    {
        panelTarget.gameObject.SetActive(true);
        Vector2 pos = mainMenuNavBarShowPosition;
        panelTarget.DOAnchorPos(pos, mainMenuNavShowTweenTime).SetEase(mainMenuNavBarShowEaseType);
        //panelTarget.DOLocalMove(pos, panelShowTweenTime).SetEase(panelShowEaseType);
    }
    
    public void MainMenuSidebarClose(RectTransform panelTarget)
    {
        panelTarget.gameObject.SetActive(true);
        Vector2 pos = mainMenuNavBarClosePosition;
        panelTarget.DOAnchorPos(pos, mainMenuNavCloseTweenTime).SetEase(mainMenuNavBarCloseEaseType);
        //panelTarget.DOLocalMove(pos, panelShowTweenTime).SetEase(panelShowEaseType);
    }

    public void playTapSound()
    {
        // Memeriksa apakah audio clip tidak null dan audio source sudah diinisialisasi
        if (tapSoundClip != null && soundSource != null)
        {
            // Memainkan audio clip sekali
            soundSource.PlayOneShot(tapSoundClip);
        }
        else
        {
            Debug.LogWarning("Tap sound clip or audio source is not set!");
        }
    }

    public void playClickSound()
    {
        // Memeriksa apakah audio clip tidak null dan audio source sudah diinisialisasi
        if (clickSoundClip != null && soundSource != null)
        {
            // Memainkan audio clip sekali
            soundSource.PlayOneShot(clickSoundClip);
        }
        else
        {
            Debug.LogWarning("Click sound clip or audio source is not set!");
        }
    }

    public void playButtonAnim(RectTransform transformTarget)
    {
        Vector3 originalSize = new Vector3(1f,1f,1f);
        Vector3 transformSize = new Vector3(1.050f, 1.050f, 1.050f);
        transformTarget.DOScale(transformSize, 0.15f).SetEase(Ease.InOutCubic).OnComplete(() => transformTarget.DOScale(originalSize, 0.10f).SetEase(Ease.OutCubic));
    }


    // ================================================================== AUDIO SETTINGS PANEL ==================================================================

    public void SaveAudioVolumeSettingToJson()
    {
        GameManager.Instance.saveManager.SaveAudioSettings(masterVolumeSlider.value, musicVolumeSlider.value, sfxVolumeSlider.value);
    }
    private void InitializeAudioSettings()
    {
        audioSource_MusicVolume = GameManager.Instance.volumeManager.audioSource_MusicVolume;
        audioSource_SfxVolume = GameManager.Instance.volumeManager.audioSource_SfxVolume;

        ResetToDefaultVolumeValueOnStart();
        GameManager.Instance.saveManager.LoadAudioSettings(); // load audio setting data dari file json
        // Set value awal untuk audio volume menggunakan json
        if (GameManager.Instance.saveManager.globalAudioConfigDataList.Count > 0)
        {
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.value = GameManager.Instance.saveManager.globalAudioConfigDataList[0].audioMasterVolume;
                masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            }

            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = GameManager.Instance.saveManager.globalAudioConfigDataList[0].audioMusicVolume;
                musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
            }

            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = GameManager.Instance.saveManager.globalAudioConfigDataList[0].audioSfxVolume;
                sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
            }
        }
        else
        {
            ResetToDefaultVolumeValue();
        }

        UpdateAudioVolumes();
    }
    public void InitializeAudioSettingsOnButton()
    {
        audioSource_MusicVolume = GameManager.Instance.volumeManager.audioSource_MusicVolume;
        audioSource_SfxVolume = GameManager.Instance.volumeManager.audioSource_SfxVolume;

        //ResetToDefaultVolumeValueOnStart();
        GameManager.Instance.saveManager.LoadAudioSettings(); // load audio setting data dari file json

        // Set value awal untuk audio volume menggunakan json
        if (GameManager.Instance.saveManager.globalAudioConfigDataList.Count > 0)
        {
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.value = GameManager.Instance.saveManager.globalAudioConfigDataList[0].audioMasterVolume;
                masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
            }

            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = GameManager.Instance.saveManager.globalAudioConfigDataList[0].audioMusicVolume;
                musicVolumeSlider.onValueChanged.AddListener(SetMusicVolumeMute);
            }

            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = GameManager.Instance.saveManager.globalAudioConfigDataList[0].audioSfxVolume;
                sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolumeMute);
            }
        }
        else
        {
            ResetToDefaultVolumeValueOnStart();
        }

        UpdateAudioVolumes();
    }

    public void ResetToDefaultVolumeValue()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = 1;
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = 1;
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = 1;
            sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
        }
    }
    public void ResetToDefaultVolumeValueOnStart() // Hanya untuk start (tanpa play audio music & sfx)
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = 1;
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = 1;
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolumeMute);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = 1;
            sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolumeMute);
        }
    }

    private void UpdateAudioVolumes()
    {
        audioSource_MusicVolume.volume = audioMusicVolume * audioMasterVolume;
        audioSource_SfxVolume.volume = audioSfxVolume * audioMasterVolume;
    }
    public void SetMasterVolume(float value)
    {
        audioMasterVolume = Mathf.Round(value * 10f) / 10f;
        masterVolumeText.text = audioMasterVolume.ToString();
        UpdateAudioVolumes();
    }
    public void SetMusicVolume(float value)
    {
        audioMusicVolume = Mathf.Round(value * 10f) / 10f;
        musicVolumeText.text = audioMusicVolume.ToString();
        UpdateAudioVolumes();
        PlayMusicPreview();
    }
    public void SetSfxVolume(float value)
    {
        audioSfxVolume = Mathf.Round(value * 10f) / 10f;
        sfxVolumeText.text = audioSfxVolume.ToString();
        UpdateAudioVolumes();
        PlaySfxPreview();
    }
    public void SetMusicVolumeMute(float value) // tanpa play audio dari SetMusicVolume()
    {
        audioMusicVolume = Mathf.Round(value * 10f) / 10f;
        musicVolumeText.text = audioMusicVolume.ToString();
        UpdateAudioVolumes();
    }
    public void SetSfxVolumeMute(float value)
    {
        audioSfxVolume = Mathf.Round(value * 10f) / 10f; // tanpa play audio dari SetSfxVolume()
        sfxVolumeText.text = audioSfxVolume.ToString();
        UpdateAudioVolumes();
    }
    private void PlayMusicPreview()
    {
        if (audioSource_MusicVolume.clip != null)
        {
            if (audioSource_MusicVolume.isPlaying)
            {
                audioSource_MusicVolume.Stop(); // Stop terlebih dahulu jika sudah sedang diputar
            }
            audioSource_MusicVolume.Play();
        }
    }
    private void PlaySfxPreview()
    {
        if (audioSource_SfxVolume.clip != null)
        {
            if (audioSource_SfxVolume.isPlaying)
            {
                audioSource_SfxVolume.Stop(); // Stop terlebih dahulu jika sudah sedang diputar
            }
            audioSource_SfxVolume.Play();
        }
    }
}
