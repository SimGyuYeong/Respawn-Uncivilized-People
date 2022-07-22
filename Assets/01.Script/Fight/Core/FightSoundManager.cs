using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightSoundManager : MonoBehaviour
{
    [SerializeField] // BGM ��� ���� ����� �ҽ�
    private AudioSource BGM;

    [SerializeField] // Effect ��� ���� ����� �ҽ�
    private AudioSource Effect;

    [SerializeField] // �������, ȿ���� ��� ����� �ҽ� �迭
    private AudioClip[] Sounds;

    public void PlayBGM(int bgmNum)
    {
        BGM.clip = Sounds[bgmNum];
        BGM.Play();
    }

    public void PauseBGM()
    {
        BGM.Pause();
    }

    public void PlayEffect(int effectNum)
    {
        Effect.clip = Sounds[effectNum];
        Effect.Play();
    }

    public void PauseEffect()
    {
        Effect.Pause();
    }
}
