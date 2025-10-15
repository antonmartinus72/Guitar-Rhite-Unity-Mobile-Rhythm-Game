using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UniversalAnimation : MonoBehaviour
{
    [Header("Show/Hide Panel")]
    public Ease ease = Ease.OutBack;
    public Vector2 hiddenPosition = new Vector2(0f, 500f);         // Posisi awal panel (di luar layar)
    public Vector2 shownPosition = new Vector2(0f, 0f);          // Posisi akhir panel (di dalam layar)
    public float duration = 0.15f;          // Durasi animasi


    [Header("Button Animation")]
    public Button button;         // Assign button dari Inspector
    public RectTransform buttonTransform;  // Assign RectTransform button dari Inspector
    public Color normalColor = Color.white;   // Warna default button
    public Color hoverColor = Color.blue;    // Warna saat di hover
    public float scaleDuration = 0.2f;    // Durasi animasi skala
    public float scaleFactor = 1.2f;      // Besar skala saat hover

    private Tween scaleTween;     // Tween untuk skala
    private Tween colorTween;     // Tween untuk warna

    [Header("Other")]
    public bool playButtonClickSound = true;
    AudioSource buttonClickAudioSource;

    private void Awake()
    {
        buttonClickAudioSource = GameManager.Instance.clickSoundAudioSource;
    }

    private void Start()
    {
        if (button != null && buttonTransform != null)
        {
            button.onClick.AddListener(OnButtonClick);
            buttonTransform.localScale = Vector3.one;
        }
        
    }

    public void ShowPanel(RectTransform panelTransform)
    {
        PlayButtonClickSound();
        panelTransform.anchoredPosition = hiddenPosition;
        panelTransform.gameObject.SetActive(true);
        panelTransform.DOAnchorPos(shownPosition, duration).SetEase(ease).SetUpdate(true);
    }

    // Fungsi untuk menyembunyikan panel dengan animasi
    public void HidePanel(RectTransform panelTransform)
    {
        PlayButtonClickSound();
        panelTransform.DOAnchorPos(hiddenPosition, duration).SetEase(ease).SetUpdate(true);
        panelTransform.gameObject.SetActive(false);
    }

    public void ShowPanelNoAnimation(RectTransform panelTransform)
    {
        PlayButtonClickSound();
        panelTransform.anchoredPosition = hiddenPosition;
        panelTransform.gameObject.SetActive(true);
        panelTransform.DOAnchorPos(shownPosition, 0f).SetEase(Ease.Unset).SetUpdate(true);
    }

    public void HidePanelNoAnimation(RectTransform panelTransform)
    {
        PlayButtonClickSound();
        panelTransform.DOAnchorPos(hiddenPosition, 0f).SetEase(Ease.Unset).SetUpdate(true);
        panelTransform.gameObject.SetActive(false);
    }

    // Fungsi untuk animasi saat button di hover
    public void OnHoverEnter()
    {
        if (scaleTween == null || !scaleTween.IsPlaying()) // Cek jika animasi skala sedang tidak berjalan
        {
            scaleTween = buttonTransform.DOScale(scaleFactor, scaleDuration).SetEase(Ease.OutBack).SetUpdate(true);
        }

        if (colorTween == null || !colorTween.IsPlaying()) // Cek jika animasi warna sedang tidak berjalan
        {
            colorTween = button.GetComponent<Image>().DOColor(hoverColor, scaleDuration).SetUpdate(true);
        }
    }

    // Fungsi untuk animasi saat hover keluar dari button
    public void OnHoverExit()
    {
        if (scaleTween == null || !scaleTween.IsPlaying()) // Cek jika animasi skala sedang tidak berjalan
        {
            scaleTween = buttonTransform.DOScale(1f, scaleDuration).SetEase(Ease.InBack).SetUpdate(true);
        }

        if (colorTween == null || !colorTween.IsPlaying()) // Cek jika animasi warna sedang tidak berjalan
        {
            colorTween = button.GetComponent<Image>().DOColor(normalColor, scaleDuration).SetUpdate(true);
        }
    }

    // Fungsi animasi saat button di klik
    public void OnButtonClick()
    {
        if (scaleTween == null || !scaleTween.IsPlaying()) // Cek jika animasi skala sedang tidak berjalan
        {
            scaleTween = buttonTransform.DOPunchScale(new Vector3(0.1f, 0.1f, 0), 0.3f, 10, 1);
            Debug.Log("Button Clicked");
        }
    }

    public void ActivateSelf()
    {
        gameObject.SetActive(true);
    }
    public void DeactivateSelf()
    {
        gameObject.SetActive(false);
    }

    public void PlayButtonClickSound()
    {
        if (playButtonClickSound)
        {
            buttonClickAudioSource.Play();
        }
    }
}


