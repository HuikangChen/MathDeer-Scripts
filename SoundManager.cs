using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager singleton;

    public AudioSource mainSource;
    public AudioSource playerSource;
    public AudioSource effectSource;
    public AudioSource uiSource;
    public AudioClip mainBG;
    public AudioClip click;
    public AudioClip jump;
    public AudioClip dash;
    public AudioClip impact;
    public AudioClip coin;
    public AudioClip wrong;
    public AudioClip correct;
    public AudioClip pickup;

    private void Awake()
    {
        singleton = this;
    }

    public void PlayMainAudio(AudioClip clip)
    {
        mainSource.clip = clip;
        mainSource.Play();
    }

    public void PlayAudioPlayer(AudioClip clip)
    {
        playerSource.clip = clip;
        playerSource.Play();
    }
    public void PlayAudioEffect(AudioClip clip)
    {
        effectSource.clip = clip;
        effectSource.Play();
    }

    public void ClickSound()
    {
        uiSource.clip = click;
        uiSource.Play();
    }
}
