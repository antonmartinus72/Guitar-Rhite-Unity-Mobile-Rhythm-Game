using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIMAnimationEvent : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] AudioClip bellClip;
    [SerializeField] AudioClip gameResultClip;

    AudioSource bellNormalTone;
    AudioSource bellHighTone;
    AudioSource bellHigherTone;
    AudioSource gameResultSfx;
    void Awake()
    {
        bellNormalTone = gameObject.AddComponent<AudioSource>();
        bellHighTone = gameObject.AddComponent<AudioSource>();
        bellHigherTone = gameObject.AddComponent<AudioSource>();
        gameResultSfx = gameObject.AddComponent<AudioSource>();

        bellNormalTone.clip = bellClip;
        bellHighTone.clip = bellClip;
        bellHigherTone.clip = bellClip;
        gameResultSfx.clip = gameResultClip;

        bellHighTone.pitch = 1.5f;
        bellHigherTone.pitch = 2f;

    }

    public void PlayBellNormalTone() 
    {
        bellNormalTone.Play();
    }

    public void PlayBellHighTone()
    {
        bellHighTone.Play();
    }

    public void PlayBellHigherTone()
    {
        bellHigherTone.Play();
    }

    public void PlayGameResultSfx()
    {
        gameResultSfx.Play();
    }

}
