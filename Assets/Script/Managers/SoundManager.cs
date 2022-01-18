using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Music
{
    public string name;
    public AudioClip audio;
}

[System.Serializable]
public class EffectMusic
{
    public string name;
    public AudioClip audio;
}

public class SoundManager : MonoBehaviour
{
    [SerializeField] Music[] BGM;
    [SerializeField] EffectMusic[] EffectMusics;
    [SerializeField] AudioSource audioPlayer;


    public void PlayingMusic(int num, float volume)
    {
        audioPlayer.clip = BGM[num].audio;
        audioPlayer.loop = true;
        audioPlayer.volume = volume;
        audioPlayer.Play();
    }

    public void PlayingEffectMusic(int num, float volume)
    {
        audioPlayer.clip = EffectMusics[num].audio;
        audioPlayer.loop = false;
        audioPlayer.volume = volume;
        audioPlayer.Play();
    }

    public void PauseMusic()
    {
        audioPlayer.Pause();
    }

    public void DeleteMusic()
    {
        audioPlayer.clip = null;
    }
}
