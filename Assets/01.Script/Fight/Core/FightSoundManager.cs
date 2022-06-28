using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightSoundManager : MonoBehaviour
{
    [SerializeField] // BGM 출력 전용 오디오 소스
    private AudioSource BGM;

    [SerializeField] // Effect 출력 전용 오디오 소스
    private AudioSource Effect1;

    [SerializeField] // Effect 출력 전용 서브 오디오 소스
    private AudioSource Effect2;

    [SerializeField] // 배경음악, 효과음 등등 오디오 소스 배열
    private AudioClip[] Sounds;

    enum SoundsIndexNumber 
    {
        BGM,
        GameOver,
        PlayerMove,
        EnemyMove,
        ButtonClick,
        PlayerClick,
        TurnEndButton,
        AttackEffect,
        Effect2
    };

    private void Awake()
    {

    }

    void Update()
    {
        
    }

    public void BGMSet()
    {
        BGM.clip = Sounds[0];
    }
}
