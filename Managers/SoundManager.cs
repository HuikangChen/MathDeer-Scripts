using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all the bg sounds and fx
/// Conains all the audio sources that plays the audio clips
/// </summary>

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance { get { return instance; } }

    //All the audio sources
    public AudioSource mainSource;
    public AudioSource playerSource;
    public AudioSource effectSource;
    public AudioSource uiSource;

    //All the sound fx in the game
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
        //Our singleton pattern
        if (instance != null && instance != this)
        {
            // destroy the gameobject if an instance of this exist already
            Destroy(gameObject);
        }
        else
        {
            //Set our instance to this object/instance
            instance = this;
        }
    }

    //Play a audioclip through the main audio source
    public void PlayMainAudio(AudioClip clip)
    {
        mainSource.clip = clip;
        mainSource.Play();
    }

    //Play a audioclip through the player audio source
    public void PlayAudioPlayer(AudioClip clip)
    {
        playerSource.clip = clip;
        playerSource.Play();
    }

    //Play a audioclip through the effect audio source
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
