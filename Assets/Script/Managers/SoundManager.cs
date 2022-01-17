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

public class SoundManager : MonoBehaviour
{
    [SerializeField] Music[] music;
    [SerializeField] AudioSource audioPlayer;

    public void PlayingMusic(int num)
    {
        audioPlayer.clip = music[num].audio;
        audioPlayer.Play();
    }

    public void PauseMusic()
    {
        audioPlayer.Pause();
    }
}
