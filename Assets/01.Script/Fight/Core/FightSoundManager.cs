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
        BGM = 0,
        GameOver,
        PlayerMove,
        EnemyMove,
        ButtonClick,
        PlayerClick,
        TurnEndButton,
        AttackEffect,
    };

    SoundsIndexNumber soundsIndexNumber = SoundsIndexNumber.BGM;

    private void Awake()
    {

    }

    void Update()
    {
        
    }

    public void BGMSet(int SoundsNum)
    {
        switch (SoundsNum)
        {
            case (int)SoundsIndexNumber.BGM :
                BGM.clip = Sounds[SoundsNum];
                BGM.Play();
                Debug.Log("f");
                break;  

            case (int)SoundsIndexNumber.GameOver :

                break;
        }
    }
}
