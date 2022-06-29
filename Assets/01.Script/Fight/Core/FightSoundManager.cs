using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightSoundManager : MonoBehaviour
{
    [SerializeField] // BGM ��� ���� ����� �ҽ�
    private AudioSource BGM;

    [SerializeField] // Effect ��� ���� ����� �ҽ�
    private AudioSource Effect1;

    [SerializeField] // Effect ��� ���� ���� ����� �ҽ�
    private AudioSource Effect2;

    [SerializeField] // �������, ȿ���� ��� ����� �ҽ� �迭
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
