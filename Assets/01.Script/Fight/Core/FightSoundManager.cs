using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightSoundManager : MonoBehaviour
{
    [SerializeField] // BGM 출력 전용 오디오 소스
    private AudioSource BGM;

    [SerializeField] // Effect 출력 전용 오디오 소스
    private AudioSource Effect;

    [SerializeField] // 배경음악, 효과음 등등 오디오 소스 배열
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
