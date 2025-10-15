using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuButtonPress : MonoBehaviour
{
    //--- Sound
    [SerializeField] private AudioClip tapSoundClip; // AudioClip yang akan dimainkan saat tombol ditekan
    [SerializeField] private AudioClip clickSoundClip; // AudioClip yang akan dimainkan saat tombol ditekan
    private AudioSource soundSource; // Referensi ke AudioSource

    //--- Tween
    [SerializeField] private Ease tweenEaseType = Ease.OutQuad;
    [SerializeField] private float tweenScaleDuration = 1f; // Durasi animasi scaling
    private Vector3 initialScale = new Vector3(1f, 1f, 0f); // Skala awal sebelum animasi
    [SerializeField] private Vector3 tweenTargetScale = new Vector3(1f, 1f, 0f); // Skala yang ingin dicapai
    

    private void Start()
    {
        
        //--- Sound

        if (GetComponent<AudioSource>() == null)
        {
            gameObject.AddComponent<AudioSource>();
        }

        // Mendapatkan komponen AudioSource dari objek yang sama dengan skrip ini
        soundSource = GetComponent<AudioSource>();

        // Periksa apakah soundSource null setelah mencoba mengambil komponen AudioSource
        if (soundSource == null)
        {
            // Jika soundSource null, munculkan pesan kesalahan
            Debug.LogError("AudioSource component is missing!");
        }

        //--- Tween


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

    public void ClickAnimation(Transform targetTransform)
    {
        // Menggunakan DOTween untuk melakukan animasi scaling objek
        targetTransform.DOScale(tweenTargetScale, tweenScaleDuration)
            .SetEase(tweenEaseType) // Memberikan efek easing ke animasi (opsional)
            .OnComplete(() => {
                // Ketika animasi pertama selesai, memulai animasi kembali dari targetScale ke skala awal
                targetTransform.DOScale(initialScale, tweenScaleDuration)
                    .SetEase(Ease.OutQuad); // Memberikan efek easing ke animasi kembali
            });
    }
}

