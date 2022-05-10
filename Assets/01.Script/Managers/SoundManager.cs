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
    /// ���� �÷��� �Լ�
    /// </summary>
    /// <param name="clip">����� Ŭ��</param>
    /// <param name="isLoop">�ݺ� ����</param>
    public void PlaySound(AudioClip clip, bool isLoop = false)
    {
        _audioSource.Stop();
        _audioSource.clip = clip;
        _audioSource.loop = isLoop;
        _audioSource.Play();
    }


    /// <summary>
    /// ���� ���� �Լ�
    /// </summary>
    public void StopSound()
    {
        _audioSource.Stop();
    }
 }
