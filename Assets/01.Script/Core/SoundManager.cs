using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 사운드 플레이 함수
    /// </summary>
    /// <param name="clip">오디오 클립</param>
    /// <param name="isLoop">반복 여부</param>
    public void PlaySound(AudioClip clip, bool isLoop = false)
    {
        _audioSource.Stop();
        _audioSource.clip = clip;
        _audioSource.loop = isLoop;
        _audioSource.Play();
    }


    /// <summary>
    /// 사운드 정지 함수
    /// </summary>
    public void StopSound()
    {
        _audioSource.Stop();
    }
 }
